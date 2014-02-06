using System.Collections.Generic;
using System.Linq;
using FieldFallback.Processors.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;

namespace FieldFallback.Processors.Caching
{
    public class FallbackValuesCache : FieldFallback.Caching.FallbackValuesCache
    {
        protected bool ClearChildEntries { get; set; }

        public FallbackValuesCache(string sizeString, string clearChildren)
            : base(sizeString)
        {
            ClearChildEntries = bool.Parse(clearChildren);
        }

        public override void RemoveItem(Item item)
        {
            // if we remove a specific version of an item, we may need
            // to clear all child cache entries since they may be
            // showing a value from the deleted version
            if (ClearChildEntries)
            {
                base.RemoveTree(item);
            }
            else
            {
                // Just remove this specific item from the cache
                base.RemoveItem(item);
            }
        }

        public override void RemoveItemField(Item item, ID fieldID)
        {
            // With the LateralFieldFallbackProcessor, we need to clear that cache of a field that references this field too.
            FallbackItem fieldItem = item;
            
            if (fieldItem != null && fieldItem.HasLateralFallback)
            {
                List<ID> fieldsThatFallbackToMe = fieldItem.GetFieldsThatFallbackTo(item.Fields[fieldID]).ToList();
                foreach (ID v in fieldsThatFallbackToMe)
                {
                    Remove(GetFieldKey(item, v));
                }
            }
            

            if (ClearChildEntries)
            {
                // we need to clear all child items that have this field
                // get cache keys for all items in this subtree
                List<string> keys = base.InnerCache.GetCacheKeys(GetGeneralItemKeyPrefix(item)).Cast<string>().ToList();
                string sfieldID = fieldID.ToString();
                foreach (string key in keys)
                {
                    // if this key contains the field, then clear it
                    if (key.Contains(sfieldID))
                    {
                        base.Remove(key);
                    }
                }
            }
            else
            {
                base.Remove(GetFieldKey(item, fieldID));
            }
        }

        
    }
}
