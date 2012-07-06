using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace FieldFallback.Caching
{
    public interface IFallbackValueCache
    {
        void AddFallbackValues(Item item, Field field, string value);

        string GetFallbackValue(Item item, Field field);

        void RemoveItem(Item item);

        void RemoveItemField(Item item, ID fieldID);

        /// <summary>
        /// Removes a tree of items from the cache.
        /// </summary>
        /// <param name="rootItem">The root item.</param>
        void RemoveTree(Item rootItem);
    }

    
}