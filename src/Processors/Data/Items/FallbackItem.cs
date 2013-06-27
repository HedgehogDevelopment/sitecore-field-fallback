using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FieldFallback.Data;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web;

namespace FieldFallback.Processors.Data.Items
{
    internal class FallbackItem : CustomItemBase
    {
        public static readonly ID FallbackFields = new ID("{8A826BE2-C878-49DA-A0F7-32DB6718E2FB}");

        private NameValueCollection _fallbackConfiguration;
        private Dictionary<ID, List<Field>> _fallbackDefinition;

        private FallbackItem(Item innerItem) : base(innerItem)
        {
          
        }
        
        public static implicit operator FallbackItem(Item item)
        {
            if (item == null)
            {
                return null;
            }

            return new FallbackItem(item);
        }
        
        /// <summary>
        /// Gets a value indicating whether this item has lateral fallback configured.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has lateral fallback; otherwise, <c>false</c>.
        /// </value>
        public bool HasLateralFallback
        {
            get
            {
                return FallbackConfiguration.HasKeys();
            }
        }

        /// <summary>
        /// Gets the fallback configuration from the "FallbackFields" field.
        /// </summary>
        /// <value>The fallback configuration.</value>
        protected NameValueCollection FallbackConfiguration
        {
            get
            {
                if (_fallbackConfiguration == null)
                {
                    // don't use fallback on this configuration field
                    using (new FallbackDisabler())
                    {
                        _fallbackConfiguration = WebUtil.ParseUrlParameters(InnerItem[FallbackFields]);
                    }
                }
                return _fallbackConfiguration ?? new NameValueCollection(0);
            }
        }

        /// <summary>
        /// Gets the fallback definition.
        /// </summary>
        /// <value>The fallback definition.</value>
        protected Dictionary<ID, List<Field>> FallbackDefinition
        {
            get
            {
                if (_fallbackDefinition == null)
                {
                    _fallbackDefinition = InitializeLateralFallbackConfiguration();
                }
                return _fallbackDefinition;
            }
        }

        /// <summary>
        /// Gets the fallback fields for the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public List<Field> GetFallbackFields(Field field)
        {
            if (DoesFieldHaveLateralFallback(field))
            {
                return FallbackDefinition[field.ID];
            }
            return new List<Field>(0);
        }

        /// <summary>
        /// Doeses the field have lateral fallback configured.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public bool DoesFieldHaveLateralFallback(Field field)
        {
            return FallbackDefinition.ContainsKey(field.ID);
        }

        /// <summary>
        /// Gets the fields that fallback to the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public IEnumerable<ID> GetFieldsThatFallbackTo(Field field)
        {
            foreach (ID key in FallbackDefinition.Keys)
            {
                if (FallbackDefinition[key].Any(f => f.ID == field.ID))
                {
                    yield return key;
                }
            }
        }

        /// <summary>
        /// Converts the NameValueCollection into Dictionary<ID, List<Field>> 
        /// where the key is the ID of the field to recieve the fallback value
        /// and the values are the list of fields to check for a fallback value
        /// </summary>
        /// <returns></returns>
        private Dictionary<ID, List<Field>> InitializeLateralFallbackConfiguration()
        {
            Dictionary<ID, List<Field>> dic; 
            if (!HasLateralFallback)
            {
                dic = new Dictionary<ID, List<Field>>(0);
            }
            else
            {
                dic = new Dictionary<ID, List<Field>>();
                foreach (string fieldName in FallbackConfiguration.AllKeys)
                {
                    Field mainField = InnerItem.Fields[fieldName];

                    if (mainField != null)
                    {
                        List<Field> fallbackFields = new List<Field>();
                        List<string> fallbackFieldNames = FallbackConfiguration.GetValues(fieldName).FirstOrDefault().Split(new char[]{'|', ',', ';'}, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (string fallbackFieldName in fallbackFieldNames)
                        {
                            Field f = InnerItem.Fields[fallbackFieldName];
                            if (f != null)
                            {
                                fallbackFields.Add(f);
                            }
                        }

                        dic.Add(mainField.ID, fallbackFields);
                    }
                }
            }
            return dic;
        }
    }
}

