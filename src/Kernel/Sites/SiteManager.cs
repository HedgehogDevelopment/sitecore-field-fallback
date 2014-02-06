using FieldFallback.Configuration;
using Sitecore.Sites;
using Sitecore.Web;

namespace FieldFallback.Sites
{
    internal class SiteManager
    {
        private const string SITE_ENABLE_FALLBACK_ATTRIBUTE = "enableFallback";
        private const string SITE_ENABLED_VALUE = "true";

        /// <summary>
        /// Enables the site.
        /// </summary>
        /// <param name="siteName">Name of the site.</param>
        public void EnableSite(string siteName)
        {
            SiteInfo site = Sitecore.Configuration.Factory.GetSiteInfo(siteName);
            if (site != null)
            {
                site.Properties.Add(SITE_ENABLE_FALLBACK_ATTRIBUTE, SITE_ENABLED_VALUE);
            }
        }

        /// <summary>
        /// Determines whether [is fallback enabled] [the specified site].
        /// </summary>
        /// <param name="site">The site.</param>
        /// <returns>
        ///   <c>true</c> if [is fallback enabled] [the specified site]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFallbackEnabled(SiteInfo site)
        {
            return  site != null &&
                    site.Properties[SITE_ENABLE_FALLBACK_ATTRIBUTE] != null &&
                    site.Properties[SITE_ENABLE_FALLBACK_ATTRIBUTE].Equals(SITE_ENABLED_VALUE);
        }

        /// <summary>
        /// Determines whether [is fallback enabled for display mode] [the specified site context].
        /// </summary>
        /// <param name="siteContext">The site context.</param>
        /// <returns>
        ///   <c>true</c> if [is fallback enabled for display mode] [the specified site context]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFallbackEnabledForDisplayMode(SiteContext siteContext)
        {
            return siteContext != null &&
                        (
                            siteContext.DisplayMode == DisplayMode.Normal ||
                            (siteContext.DisplayMode == DisplayMode.Preview && Config.PreviewEnabled) ||
                            (siteContext.DisplayMode == DisplayMode.Edit && Config.EditEnabled)
                        );
        }
    }
}