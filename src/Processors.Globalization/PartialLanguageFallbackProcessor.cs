using System.Collections.Generic;
using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Globalization.Data.Fields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;

namespace FieldFallback.Processors.Globalization
{
    public class PartialLanguageFallbackProcessor : FieldFallbackProcessor
    {
        public override bool IsEnabledForField(Field field)
        {
            TemplateFallbackFieldItem fallbackField = field;
            return (fallbackField != null && fallbackField.EnableLanguageFallback);
        }

        public override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            Assert.IsNotNull(args.Field, "Field is null");

            if (IsEnabledForField(args.Field))
            {
                Item fallbackItem = GetFallbackItem(args.Field);

                if (fallbackItem != null)
                {
                    // stop processing subsequent fallback checks
                    args.AbortPipeline();

                    // Get field's value from the fallback item
                    return fallbackItem.Fields[args.Field.ID].GetValueSafe(true, true, false);
                }
            }
            return null;
        }
        
        /// <summary>
        /// Gets the fallback item that has a version with a value for the field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        private Item GetFallbackItem(Field field)
        {
            Item item = field.Item;
            Database database = field.Database;
            Language language = field.Language;
            
            Item fallbackItem = null;

            // get the fallback languages 
            IEnumerable<Language> fallbackLanguages = language.GetFallbackLanguages(database);
            
            foreach (Language fallbackLanguage in fallbackLanguages)
            {
                fallbackItem = database.GetItem(item.ID, fallbackLanguage);

                // first fallback item in the chain of fallback languages with a value wins
                if (fallbackItem != null && fallbackItem.Versions.Count > 0 && DoesItemHaveFieldWithValue(fallbackItem, field.ID, true))
                {
                    break;
                }
            }

            return fallbackItem;
        }
    }
}
