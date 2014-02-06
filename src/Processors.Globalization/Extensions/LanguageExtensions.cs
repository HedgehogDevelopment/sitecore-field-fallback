using System.Collections.Generic;
using FieldFallback.Processors.Globalization;
using FieldFallback.Processors.Globalization.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;

namespace FieldFallback
{
    public static class LanguageExtensions
    {
        /// <summary>
        /// Given a language, get the chain of languages to check for a value on
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public static IEnumerable<Language> GetFallbackLanguages(this Language language, Database database)
        {
            // Get the item for this language
            LanguageItem sourceLangItem = GetLanguageDefinitionItem(language, database);

            if (sourceLangItem != null)
            {
                // get the configured fallback language for this language
                Language fallbackLang = sourceLangItem.FallbackLanguage;

                while (fallbackLang != null && !fallbackLang.Name.IsNullOrEmpty()) // build up the fallback chain
                {
                    yield return fallbackLang;
                    sourceLangItem = GetLanguageDefinitionItem(fallbackLang, database);
                    fallbackLang = sourceLangItem.FallbackLanguage;
                }
            }
        }

        /// <summary>
        /// Gets the item representing the given Language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        private static Item GetLanguageDefinitionItem(Language language, Database database)
        {
            ID sourceLanguageItemId = LanguageManager.GetLanguageItemId(language, database);
            return ID.IsNullOrEmpty(sourceLanguageItemId) ? null : ItemManager.GetItem(sourceLanguageItemId, Config.MasterLanguage, Version.Latest, database, SecurityCheck.Disable);
        }
    }
}
