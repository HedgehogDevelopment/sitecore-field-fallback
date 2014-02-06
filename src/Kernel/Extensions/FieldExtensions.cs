using Sitecore.Data.Fields;
using FieldFallback.Data;

namespace FieldFallback
{
    public static class FieldExtensions
    {
        public static bool IsSystem(this Field field)
        {
            // If the field doesn't have a definition, then it might not be 
            // published and we can throw ourself in a loop. 
            // To be safe, if it is an unpublished field assume it is a system field
            return field.Definition != null && field.Name.StartsWith("__");
        }

        /// <summary>
        /// The proper way to check if a field has the standard value when fallback is instaled.
        /// It is also the safe way to call Field.ContainsStandardValue that gets around Sitecore bug #368493
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if [has value safe] [the specified field]; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsStandardValueSafe(this Field field)
        {
            // safely call Field.GetValue with fallback disabled
         //   FixContainsStandardValue(field);
            return field.ContainsStandardValue;
        }

        /// <summary>
        /// A safe way to call Field.HasValue that gets around Sitecore bug #368493
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>
        ///   <c>true</c> if [has value safe] [the specified field]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasValueSafe(this Field field)
        {
            // field.HasValue calls Field.GetValue(false, false)
            // The GetValue(false,false) method internally will cause the ContainsStandardValue property to break
            // Since GetValue(false, false) isn't using standard values it won't cause any deep diving into the fallback provider
            bool hasValue = field.HasValue;

            // that previous call just broke the ContainsStandardValue property...
            FixContainsStandardValue(field);

            return hasValue;
        }

        /// <summary>
        /// A safe way to call Field.GetValue that gets around Sitecore bug #368493
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="allowStandardValue">if set to <c>true</c> [allow standard value].</param>
        /// <returns></returns>
        public static string GetValueSafe(this Field field, bool allowStandardValue)
        {
            return GetValueSafe(field, allowStandardValue, true, false);
        }

        /// <summary>
        /// A safe way to call Field.GetValue that gets around Sitecore bug #368493
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="allowStandardValue">if set to <c>true</c> [allow standard value].</param>
        /// <param name="allowDefaultValue">if set to <c>true</c> [allow default value].</param>
        /// <returns></returns>
        public static string GetValueSafe(this Field field, bool allowStandardValue, bool allowDefaultValue)
        {
            return GetValueSafe(field, allowStandardValue, allowDefaultValue, true);
        }

        /// <summary>
        /// A safe way to call Field.GetValue that gets around Sitecore bug #368493
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="allowStandardValue">if set to <c>true</c> [allow standard value].</param>
        /// <param name="allowDefaultValue">if set to <c>true</c> [allow default value].</param>
        /// <param name="allowFallbackValue">if set to <c>true</c> [allow fallback value].</param>
        /// <returns></returns>
        public static string GetValueSafe(this Field field, bool allowStandardValue, bool allowDefaultValue, bool allowFallbackValue)
        {
            string val = null;
            
            // disable fallback?
            using (new FallbackDisabler(allowFallbackValue ? FallbackStates.Default : FallbackStates.Disabled ))
            {
                // get the raw value of the item
                val = field.GetValue(allowStandardValue, allowDefaultValue);
            }

            // if allowStandard Value was false and allowDefault was true
            //   then that previous call just broke the ContainsStandardValue property...
            if (!allowStandardValue && allowDefaultValue)
            {
                // that previous call just broke the ContainsStandardValue property...
                FixContainsStandardValue(field);
            }
            return val;
        }

        /// <summary>
        /// Fixes the contains standard value bug #368493.
        /// </summary>
        /// <see cref="https://support.sitecore.net/helpdesk/defects/ViewDefect.aspx?DefectId=368493"/>
        /// <param name="field">The field.</param>
        private static void FixContainsStandardValue(Field field)
        {
            // disable fallback and get the value again to fix the bug
            using (new FieldFallback.Data.FallbackDisabler())
            {
                // just call GetValue... we can disregard the result
                field.GetValue(true, false);
            }
        }
    }
}
