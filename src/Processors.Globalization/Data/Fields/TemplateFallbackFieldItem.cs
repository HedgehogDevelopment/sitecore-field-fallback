using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace FieldFallback.Processors.Globalization.Data.Fields
{
    internal class TemplateFallbackFieldItem : TemplateFieldItem
    {
        public static readonly ID EnableLanguageFallbackFieldID = new ID("{76F75F49-BC51-44C2-9085-13BC162FD9A4}");

        private TemplateFallbackFieldItem(Item innerItem, TemplateSectionItem owner)
            : base(innerItem, owner)
        {
           
        }

        public static implicit operator TemplateFallbackFieldItem(Item item)
        {
            if (item == null)
            {
                return null;
            }
            if (item.TemplateID != TemplateIDs.TemplateField)
            {
                Error.Raise("Item \"" + item.Paths.Path + "\" is not a template field.");
            }
            return new TemplateFallbackFieldItem(item, item.Parent);
        }

        public static implicit operator TemplateFallbackFieldItem(Field field)
        {
            if (field == null)
            {
                return null;
            }
            
            Item item = null;
            using (new SecurityDisabler())
            {
                item = field.Database.GetItem(field.ID);
            }

            if (item == null)
            {
                return null;
            }

            if (item.TemplateID != TemplateIDs.TemplateField)
            {
                Error.Raise("Item \"" + item.Paths.Path + "\" is not a template field.");
            }
            return new TemplateFallbackFieldItem(item, item.Parent);
        }
                
        /// <summary>
        /// Gets a value indicating whether [enable language fallback].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable language fallback]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLanguageFallback
        {
            get
            {
                return InnerItem.Fields[EnableLanguageFallbackFieldID] != null && ((CheckboxField)InnerItem.Fields[EnableLanguageFallbackFieldID]).Checked;
            }
        }
    }
}
