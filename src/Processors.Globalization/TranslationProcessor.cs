using System;
using System.Collections.Generic;
using System.Linq;
using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Globalization.Data.Fields;
using FieldFallback.Processors.Translation;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;

namespace FieldFallback.Processors
{
    public class TranslationProcessor : FieldFallbackProcessor
    {
        private string _masterLanguageName;
        private Language _masterLanguage;

        /// <summary>
        /// The translation service to use. This is set via Sitecore reflection.
        /// </summary>
        public ITranslationService TranslationService { get; set; }

        /// <summary>
        /// The master language to use as the source for translation
        /// </summary>
        protected Language MasterLanguage
        {
            get
            {
                return _masterLanguage;
            }
        }

        protected IEnumerable<string> SupportedLanguages { get; private set; }

        public TranslationProcessor(string masterLanguageName, string supportedLanguages)
        {
            _masterLanguageName = masterLanguageName;
            _masterLanguage = InitMasterLanguage();
            SupportedLanguages = supportedLanguages.Split(new[] { '|', ' ', ',' });
        }

        protected override bool IsEnabledForField(Field field)
        {
            Assert.IsNotNull(field, "Field is null");

            TemplateFallbackFieldItem fallbackField = field;
            return field.Language.Name != _masterLanguageName 
                    && SupportedLanguages.Contains(field.Language.CultureInfo.TwoLetterISOLanguageName)
                    && (fallbackField != null && fallbackField.EnableLanguageTranslation);
        }

        protected override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            Assert.IsNotNull(args.Field, "Field is null");

            Item masterItem = GetMasterItem(args.Field);
            string fieldValue = null;

            if (masterItem != null)
            {
                // Get the value of the field, from the master item, so we can translate it
                string translateMe = masterItem.Fields[args.Field.ID].GetValueSafe(true, false, false);

                // Call the translation service
                fieldValue = TranslationService.Translate(translateMe, _masterLanguage.CultureInfo, args.Field.Language.CultureInfo);
            }

            return fieldValue;
        }

        /// <summary>
        /// Tries to load the Language via the passed in name
        /// </summary>
        /// <returns></returns>
        private Language InitMasterLanguage()
        {
            Language masterLanguage;
            if (!Language.TryParse(_masterLanguageName, out masterLanguage))
            {
                throw new Exception(string.Format("Unable to parse the MasterLanguageName '{0}'", _masterLanguageName));
            }
            return masterLanguage;
        }

        /// <summary>
        /// Gets the item in the master language.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        private Item GetMasterItem(Field field)
        {
            Item item = field.Item;
            Database database = field.Database;

            Item fallbackItem = database.GetItem(item.ID, _masterLanguage);
            return fallbackItem;
        }
    }
}