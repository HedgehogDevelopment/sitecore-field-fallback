using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Extensions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace FieldFallback.Processors
{
    public class DefaultValuesFallbackProcessor : FieldFallbackProcessor
    {
        protected override bool IsEnabledForField(Field field)
        {
            //This processor is always enabled because it is processed
            // for all fields that are part of the Item being created.
            return true;
        }

        protected override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            Assert.IsNotNull(args.Field, "Field is null");
            Item fallbackItem = GetFallbackItem(args.Field);

            // if we have an Default Item with the field...
            if (fallbackItem != null && fallbackItem.Fields[args.Field.ID] != null)
            {
                // get the value of the default item.
                return fallbackItem.Fields[args.Field.ID].GetValueSafe(false, false, false);
            }
            return null;
        }

        private Item GetFallbackItem(Field field)
        {
            using (new SecurityDisabler())
            {
                TemplateItem currentTemplate = field.Database.GetTemplate(field.Item.TemplateID);
                return field.Database.GetItem(currentTemplate.GetFullContentItemPath());
            }
        }
    }
}