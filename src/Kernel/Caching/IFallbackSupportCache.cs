using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace FieldFallback.Caching
{
    public interface IFallbackSupportCache
    {
        /// <summary>
        /// Does the field support fallback?
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        bool? GetFallbackSupport(Field field);

        /// <summary>
        /// Sets the fallback support.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="isSupported">if set to <c>true</c> [is supported].</param>
        /// <returns></returns>
        void SetFallbackSupport(Field field, bool isSupported);
    }
}