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
                    args.FallbackValue = GetFallbackValue(args);
                }
                else
                {
                    args.HasFallbackValue = HasFallbackValue(args);
                }

                if (args.HasFallbackValue)
                {
                    args.AbortPipeline();
                }
            }
        }

        protected abstract bool IsEnabledForField(Field field);

        protected abstract string GetFallbackValue(FieldFallbackPipelineArgs args);

        protected virtual bool HasFallbackValue(FieldFallbackPipelineArgs args)
        {
            // standard values comes before fallback...
            // therefore, if we have standard values, we can't be falling back
            if (args.Field.ContainsStandardValueSafe())
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
