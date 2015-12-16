using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using FieldFallback.Data;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web;

namespace FieldFallback.Processors.Data.Items
{
    internal class FallbackItem : CustomItemBase
    {
        public static readonly ID FallbackFields = new ID("{8A826BE2-C878-49DA-A0F7-32DB6718E2FB}");

        private FallbackItem(Item innerItem) : base(innerItem)
        {
            Initialize();
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
                return FallbackDefinition.Keys.Count > 0;
            }
        }

        /// <summary>
        /// Gets the fallback definition.
        /// </summary>
        /// <value>The fallback definition.</value>
        protected Dictionary<ID, Setting> FallbackDefinition
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the fallback fields for the given field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public Setting GetFallbackFields(Field field)
        {
            if (DoesFieldHaveLateralFallback(field))
            {
                return FallbackDefinition[field.ID];
            }
            return null;
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
                if (FallbackDefinition[key].SourceFields.Any(f => f.ID == field.ID))
                {
                    yield return key;
                }
            }
        }

        private void Initialize()
        {
            var _fallbackDefinition = new Dictionary<ID, Setting>();

            // don't use fallback on this configuration field
            using (new FallbackDisabler())
            {
                // try to get this fields as an XML field
                // Original version of this field was key/value pair
                // It was converted to XML to support advanced options
                bool isFieldXml = false;
                if (IsXml(InnerItem.Fields[FallbackFields]))
                {
                    XmlField testXmlField = InnerItem.Fields[FallbackFields];
                    if (testXmlField != null && testXmlField.Xml != null)
                    {
                        ParseXmlConfiguration(testXmlField, _fallbackDefinition);
                        isFieldXml = true;
                    }
                }
                
                if(!isFieldXml)
                {
                    ParseNameValueConfiguration(InnerItem[FallbackFields], _fallbackDefinition);
                }
                
            }
            FallbackDefinition = _fallbackDefinition;
        }

        private bool IsXml(Field testField)
        {
            try
            {
                string test = string.Empty;
                if (testField != null)
                {
                    test = testField.Value;
                }
                XmlDocument testXmlDocument = new XmlDocument();
                testXmlDocument.LoadXml(test); //will throw XmlException if test != XML
                return true;//testField is XML
            }
            catch (XmlException)
            {
                //Error trying to get field as XML
                //Must be a key/value pair
            }

            return false;

        }

        private void ParseNameValueConfiguration(string oldConfigurationValue, Dictionary<ID, Setting> settings)
        {
            NameValueCollection fallbackConfiguration = WebUtil.ParseUrlParameters(oldConfigurationValue);
            foreach (string fieldName in fallbackConfiguration.AllKeys)
            {
                Field mainField = InnerItem.Fields[fieldName];

                if (mainField != null)
                {
                    List<Field> fallbackFields = new List<Field>();
                    List<string> fallbackFieldNames = fallbackConfiguration.GetValues(fieldName).FirstOrDefault().Split(new char[] { '|', ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (string fallbackFieldName in fallbackFieldNames)
                    {
                        Field f = InnerItem.Fields[fallbackFieldName];
                        if (f != null)
                        {
                            fallbackFields.Add(f);
                        }
                    }

                    Setting setting = new Setting();
                    setting.SourceFields = fallbackFields;

                    settings.Add(mainField.ID, setting);
                }
            }
        }

        private void ParseXmlConfiguration(XmlField fallbackField, Dictionary<ID, Setting> settings)
        {
            // complex structure...
            // TODO: Create a nice Sitecore field editing interface for this

            /// <fallback>
            ///     <setting target="{id}" source="{id}|{id}|{id}..." enableEllipsis="true|false" clipAt="{number chars}" />
            ///     <setting target="{id}" source="{id}|{id}|{id}..." enableEllipsis="true|false" clipAt="{number chars}" />
            ///     ...
            /// </fallback>

            foreach (XmlNode child in fallbackField.Xml.FirstChild.ChildNodes)
            {
                XmlAttribute target = child.Attributes["target"];
                XmlAttribute source = child.Attributes["source"];
                XmlAttribute enableEllipsis = child.Attributes["enableEllipsis"];
                XmlAttribute clipAt = child.Attributes["clipAt"];

                /// TODO: Guard against improper input.. move to factory
                Setting setting = new Setting();

                if (enableEllipsis != null)
                {
                    setting.UseEllipsis = bool.Parse(enableEllipsis.Value);
                }

                if (clipAt != null)
                {
                    int i;
                    if (int.TryParse(clipAt.Value, out i) && i > 0)
                    {
                        setting.ClipAt = i;
                    }
                }

                List<Field> fallbackFields = new List<Field>();
                List<string> fallbackFieldNames = source.Value.Split(new char[] { '|', ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string fallbackFieldName in fallbackFieldNames)
                {
                    Field f = InnerItem.Fields[fallbackFieldName];
                    if (f != null)
                    {
                        fallbackFields.Add(f);
                    }
                }

                setting.SourceFields = fallbackFields;

                ID targetID = new Sitecore.Data.ID(target.Value);

                settings.Add(targetID, setting);

            }
        }

        internal class Setting
        {
            public List<Sitecore.Data.Fields.Field> SourceFields { get; set; }

            public bool TruncateText
            {
                get
                {
                    return ClipAt.HasValue;
                }
            }

            public bool UseEllipsis { get; set; }

            public int? ClipAt { get; set; }
        }
    }
}

