
namespace FieldFallback.Data
{
    public class FallbackValuesManager
    {
        /// <summary>
        /// Gets the FallbackValuesProvider.
        /// </summary>
        public static FallbackValuesProvider Provider
        {
            get
            {
                return Sitecore.Data.StandardValuesManager.Provider as FallbackValuesProvider;
            }
        }
    }
}
