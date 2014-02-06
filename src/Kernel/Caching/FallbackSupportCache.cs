using System.Collections.Generic;
using Sitecore.Data.Fields;

namespace FieldFallback.Caching
{
    public class FallbackSupportCache : IFallbackSupportCache
    {
        private const string CACHE_KEY = "FallbackSupportCache";

        public bool? GetFallbackSupport(Sitecore.Data.Fields.Field field)
        {
            bool? isSupported = null;
            Dictionary<string, bool> cache = GetInternalCache();

            if (cache != null)
            {
                string cckey = GetFieldCacheKey(field);
                if (cache.ContainsKey(cckey))
                {
                    isSupported = cache[cckey];
                }
            }

            return isSupported;
        }

        public void SetFallbackSupport(Sitecore.Data.Fields.Field field, bool isSupported)
        {
            Dictionary<string, bool> cache = GetInternalCache();
            if (cache != null)
            {
                string cckey = GetFieldCacheKey(field);
                cache[cckey] = isSupported;
            }
        }

        private Dictionary<string, bool> GetInternalCache()
        {
            if (System.Web.HttpContext.Current != null)
            {
                Dictionary<string, bool> cache = (Dictionary<string, bool>)System.Web.HttpContext.Current.Items[CACHE_KEY];

                if (cache == null)
                {
                    cache = new Dictionary<string, bool>();
                    System.Web.HttpContext.Current.Items.Add(CACHE_KEY, cache);
                }
                return cache;
            }
            return null;
        }

        private string GetFieldCacheKey(Field field)
        {
            return string.Concat(field.Item.ID.ToString(), ":", field.ID.ToString());
        }
    }
}