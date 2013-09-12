using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft;

namespace FieldFallback.Processors.Translation
{
    /// <summary>
    /// Translates text via the Microsoft Azure Translation Service
    /// https://datamarket.azure.com/dataset/bing/microsofttranslator
    /// </summary>
    public class MicrosoftTranslator : ITranslationService
    {
        private const string SERVICE_URL = "https://api.datamarket.azure.com/Bing/MicrosoftTranslator/v1/";

        private string _username, _password;

        public MicrosoftTranslator(string accountKey)
        {
            _username = accountKey;
            _password = accountKey;

#if DEBUG
            // Fiddler debugging of https... only for debug
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(
                delegate
                {
                    return true;
                });
#endif
        }

        public string Translate(string translateMe, CultureInfo fromCultureInfo, CultureInfo toCultureInfo)
        {
            if (fromCultureInfo == null)
            {
                throw new ArgumentNullException("fromCultureInfo is null");
            }
            if (toCultureInfo == null)
            {
                throw new ArgumentNullException("toCultureInfo is null");
            }

            if (string.IsNullOrEmpty(translateMe))
            {
                return translateMe;
            }

            string sourceLanguage = fromCultureInfo.TwoLetterISOLanguageName;
            string targetLanguage = toCultureInfo.TwoLetterISOLanguageName;

            TranslatorContainer tc = new TranslatorContainer(new System.Uri(SERVICE_URL));
            tc.Credentials = new System.Net.NetworkCredential(_username, _password);

            var translationQuery = tc.Translate(translateMe, targetLanguage, sourceLanguage);

            List<Microsoft.Translation> translationResults = translationQuery.Execute().ToList();

            // Verify there was a result 
            if (translationResults.Count() <= 0)
            {
                return null;
            }

            // In case there were multiple results, pick the first one 
            Microsoft.Translation translationResult = translationResults.First();

            return translationResult.Text;
        }
    }
}
