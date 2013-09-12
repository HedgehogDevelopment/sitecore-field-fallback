using System.Linq;
using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Data.Fields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace FieldFallback.Processors
{
    public class AncestorFallbackProcessor : FieldFallbackProcessor
    {
        protected override bool IsEnabledForField(Field field)
        {
            TemplateFallbackFieldItem fallbackField = field;
            return (fallbackField != null && fallbackField.EnableAncestorFallback);
        }

        protected override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            Assert.IsNotNull(args.Field, "Field is null");
            Item fallbackItem = GetFallbackItem(args.Field);

            // if we have an ancestor with the field...
            if (fallbackItem != null && fallbackItem.Fields[args.Field.ID] != null)
            {
                // get the value of the ancestor item.
                // Standard Values are an acceptable value!
                return fallbackItem.Fields[args.Field.ID].GetValueSafe(true, false, false);
            }
            return null;
        }

        /// <summary>
        /// Gets the nearest set ancestor that has the field with a value
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        private Item GetFallbackItem(Field field)
        {
            using (new SecurityDisabler())
            {
                return field.Item.Axes.GetAncestors().Where(ancestor => DoesItemHaveFieldWithValue(ancestor, field.ID, true)).LastOrDefault();
            }
        }
    }
}
