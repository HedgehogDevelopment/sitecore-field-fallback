using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace FieldFallback.Pipelines.FieldFallbackPipeline
{
    public abstract class FieldFallbackProcessor
    {
        public void Process(FieldFallbackPipelineArgs args)
        {
            // If the processor isn't enabled for this field, then don't even run the processor.
            if (IsEnabledForField(args.Field))
            {
                args.FallbackValue = GetFallbackValue(args);

                if (args.HasFallbackValue)
                {
                    args.AbortPipeline();
                }
            }
        }

        protected abstract bool IsEnabledForField(Field field);

        protected abstract string GetFallbackValue(FieldFallbackPipelineArgs args);

        /// <summary>
        /// Does the item have the specified field with a value?
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fieldID">The field ID.</param>
        /// <param name="allowStandardValue">if set to <c>true</c> [allow standard value].</param>
        /// <returns></returns>
        protected bool DoesItemHaveFieldWithValue(Item item, ID fieldID, bool allowStandardValue = false)
        {
            // Check if the field exists on the item
            // AND
            // if the field HasValue
            //! HasValue is much different than !string.IsNullOrEmpty(item[fieldID])
            //!     HasValue excludes standard values when checking.... 
            Field field = item.Fields[fieldID];
            if (field == null)
            {
                return false;
            }

            string val = field.GetValueSafe(allowStandardValue, false, false);
            return val != null;
        }
    }
}
