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
                if (args.Method == FieldFallbackPipelineArgs.Methods.Execute)
                {
                    string fallbackValue = GetFallbackValue(args);
                    args.FallbackValue = fallbackValue;
                }
                else
                {
                    bool hasValue = HasFallbackValue(args);
                    args.HasFallbackValue = hasValue;
                }
            }
        }

        public abstract bool IsEnabledForField(Field field);

        public abstract string GetFallbackValue(FieldFallbackPipelineArgs args);

        public virtual bool HasFallbackValue(FieldFallbackPipelineArgs args)
        {
            // standard values comes before fallback...
            // therefore, if we have standard values, we can't be falling back
            if (args.Field.ContainsStandardValue)
            {
                return false;
            }

            // if the item has a value for the field, then we aren't falling back
            if (DoesItemHaveFieldWithValue(args.Field.Item, args.Field.ID))
            {
                return false;
            }

            // otherwise, if the value of the field is the same as the calculated fallback value then we are indeed falling back
            if (args.Field.Value == GetFallbackValue(args))
            {
                args.AbortPipeline();
                return true;
            }

            return false;
        }

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
