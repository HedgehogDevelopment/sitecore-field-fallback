using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace FieldFallback.Processors.Data.Fields
{
    internal class TemplateFallbackFieldItem : TemplateFieldItem
    {
        public static readonly ID EnableAncestorFallbackFieldID = new ID("{556130F8-FC4E-4CED-9B90-CB6D51E4175F}");
        public static readonly ID DefaultFieldValue = new ID("{D2F2D7EB-B1D4-4DD4-B94E-A3707455DDAB}");

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
        /// Gets a value indicating whether [enable ancestor fallback].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable ancestor fallback]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableAncestorFallback
        {
            get
            {
                return InnerItem.Fields[EnableAncestorFallbackFieldID] != null && ((CheckboxField)InnerItem.Fields[EnableAncestorFallbackFieldID]).Checked;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether [enable default value fallback].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable default value fallback]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDefaultValueFallback
        {
            get
            {
                // if we have some value we want to default to, then it is enabled
                return DefaultFallbackValue != null;
            }
        }

        /// <summary>
        /// Gets the default fallback value.
        /// </summary>
        /// <value>The default fallback value.</value>
        public string DefaultFallbackValue
        {
            get
            {
                return InnerItem.Fields[DefaultFieldValue].GetValueSafe(false, false);
            }
        }
    }
}
