using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace FieldFallback.Processors.Globalization.Data.Items
{
    public class LanguageItem : CustomItemBase
    {
        public static readonly ID FallbackLanguageField = new ID("{892975AC-496F-4AC9-8826-087095C68E1D}");

        private LanguageItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator LanguageItem(Item item)
        {
            if (item == null)
            {
                return null;
            }

            return new LanguageItem(item);
        }

        /// <summary>
        /// Gets the fallback language.
        /// </summary>
        public Language FallbackLanguage
        {
            get
            {
                // Get the field value. 
                // The fallback language is stored as plain text not a link to an item
                var fallbackLangName = InnerItem[FallbackLanguageField];
                Language fallbackLang;
                return Language.TryParse(fallbackLangName, out fallbackLang) ? fallbackLang : null;
            }
        }

    }
}