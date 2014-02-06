using Sitecore.Data.Items;

namespace FieldFallback.Caching
{
    public interface ISkipItemCache
    {
        bool IsItemSkipped(Item item);

        void SetSkippedItem(Item item);

        void UnSkipItem(Item item);
    }
}