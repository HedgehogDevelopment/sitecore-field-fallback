using Sitecore;
using Sitecore.Caching;
using Sitecore.Data.Items;

namespace FieldFallback.Caching
{
    public class SkipItemCache : CustomCache, ISkipItemCache
    {
        public SkipItemCache(string sizeString)
            : base("Field Fallback Skipped Items", StringUtil.ParseSizeString(sizeString))
        {
        }

        public bool IsItemSkipped(Item item)
        {
            return GetString(GetSkippedItemKey(item)) != null;
        }

        public void SetSkippedItem(Item item)
        {
            SetString(GetSkippedItemKey(item), "1");
        }

        public void UnSkipItem(Item item)
        {
            base.Remove(GetSkippedItemKey(item));
        }

        private string GetSkippedItemKey(Item item)
        {
            // don't use the item's path as path is more expensive than getting an ID
            return string.Format("skippedItem://{0}/{1}", item.Database.Name, item.ID.ToString());
        }
    }
}