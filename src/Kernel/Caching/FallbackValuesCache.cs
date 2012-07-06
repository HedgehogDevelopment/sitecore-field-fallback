using Sitecore;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace FieldFallback.Caching
{
    /// <summary>
    /// A basic implementation of an IFallbackValuesCache.
    /// This will get/insert values. It will also clear values for fields on Save or Delete.
    /// Specific FieldFallbackProcessors may require custom cache logic.
    /// </summary>
    public class FallbackValuesCache : CustomCache, IFallbackValueCache
    {
        public FallbackValuesCache(string sizeString)
            : base("Field Fallback Values", StringUtil.ParseSizeString(sizeString))
        {
        }

        public virtual void AddFallbackValues(Item item, Field field, string value)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(field, "field");
            Assert.ArgumentNotNull(value, "value");

            SetString(GetFieldKey(item, field.ID), value);
        }

        public virtual string GetFallbackValue(Item item, Field field)
        {
            Assert.ArgumentNotNull(item, "item");

            return GetString(GetFieldKey(item, field.ID));
        }

        public virtual void RemoveItem(Item item)
        {
            base.RemovePrefix(GetSpecificItemKeyPrefix(item));
        }

        /// <summary>
        /// Removes a tree of items from the cache.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        public virtual void RemoveTree(Item rootItem)
        {
            string generalKey = GetGeneralItemKeyPrefix(rootItem);
            base.RemovePrefix(generalKey);
        }

        public virtual void RemoveItemField(Item item, ID fieldID)
        {
            base.Remove(GetFieldKey(item, fieldID));
        }

        /// <summary>
        /// Gets the key for the item without any version/language.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected virtual string GetGeneralItemKeyPrefix(Item item)
        {
            // item.Paths.Path is fairly expensive, but we need it here to support clearing trees
            return string.Format("sitecore://{0}/{1}", item.Database.Name, item.Paths.Path);
        }

        protected virtual string GetSpecificItemKeyPrefix(Item item)
        {
            return string.Format("{0}?lang={1}&ver={2}", GetGeneralItemKeyPrefix(item), item.Language.Name, item.Version.Number);
        }

        protected virtual string GetFieldKey(Item item, ID fieldID)
        {
            return string.Format("{0}|{1}", GetSpecificItemKeyPrefix(item), fieldID);
        }
    }
}
