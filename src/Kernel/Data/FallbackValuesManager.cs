using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

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
                ProviderHelper<StandardValuesProvider, StandardValuesProviderCollection> providerHelper = 
                    ServiceLocator.ServiceProvider.GetService<ProviderHelper<StandardValuesProvider, StandardValuesProviderCollection>>();

                return providerHelper.Provider as FallbackValuesProvider;
            }
        }
    }
}
