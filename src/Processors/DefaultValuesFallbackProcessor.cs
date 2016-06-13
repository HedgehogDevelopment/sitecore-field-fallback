using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Data.Items;
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

        protected Item GetFallbackItem(Field field)
        {
            using (new SecurityDisabler())
            {
                TemplateItem currentTemplate = field.Database.GetTemplate(field.Item.TemplateID);

                //Get the item from the database so we have the correct item
                DefaultValuesItem contentPath = new DefaultValuesItem(field.Item);
                
                return field.Database.GetItem(contentPath.GetFullContentItemPath(currentTemplate));
            }
        }
    }
}