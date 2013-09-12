using System.Globalization;

namespace FieldFallback.Processors.Translation
{
    public interface ITranslationService
    {
        /// <summary>
        /// Translates the specified text to the specified culture.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns></returns>
        string Translate(string text, CultureInfo fromCultureInfo, CultureInfo toCultureInfo);
    }
}