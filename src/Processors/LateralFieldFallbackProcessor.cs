using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace FieldFallback.Processors
{
    public class LateralFieldFallbackProcessor : FieldFallbackProcessor
    {
        public override bool IsEnabledForField(Field field)
        {
            if (field.ID == FallbackItem.FallbackFields)
            {
                return false;
            }

            FallbackItem item = field.Item;
            return item.DoesFieldHaveLateralFallback(field);
        }

        public override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            Assert.IsNotNull(args.Field, "Field is null");
            FallbackItem item = args.Field.Item;

            // Get the fields that this field should fallback to
            foreach (Field f in item.GetFallbackFields(args.Field))
            {
                // get the value of the field
                // Standard Values are an acceptable value
                string val = f.GetValueSafe(true, false, false);
                if (val != null)
                {
                    // we found our value, don't execute any other processors.
                    args.AbortPipeline();

                    return val;
                }
            }
            return null;
        }
    }
}
