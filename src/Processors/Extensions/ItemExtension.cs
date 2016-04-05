using FieldFallback.Processors.Configuration;
using Sitecore;
using Sitecore.Data.Items;


namespace FieldFallback.Processors.Extensions
{
    public static class ItemExtension
    {
        public static string GetTemplatedContentItem(this TemplateItem item)
        {
            // Path of content item dynamically created by 
            // FieldFallback Processor Event
            string itemPath = string.Format("{0}{1}{2}",
                GetParentContentPath(item),
                item.Name,
                Config.ContentItemSuffix);
            //Item createdItem = templateItem.Database.GetItem(itemPath);

            return itemPath;
        }

        /// <summary>
        ///     Get the parent of the content item path
        ///     that has to be created based off of the template.
        /// </summary>
        /// <param name="templateItem">The template item.</param>
        /// <returns>
        ///     The parent content path of the item that
        ///     is going to be created.
        /// </returns>
        public static string GetParentContentPath(this TemplateItem item)
        {
            Item workingItem = (Item) item;
            string itemPath =
                workingItem.Paths.ParentPath
                    //Remove the template path
                    .Substring(Config.DefaultTemplateLocation.Length)
                    //Insert the item location
                    .Insert(0, Config.DefaultItemLocation);

            return itemPath;
        }

        /// <summary>
        ///     Query if 'item' is template.
        /// </summary>
        /// <param name="item">
        ///     The Sitecore item to evaluate
        ///     if it is a template.
        /// </param>
        /// <returns>
        ///     true if the item is a template, false if it is not.
        /// </returns>
        public static bool IsTemplate(this Item item)
        {
            return item.TemplateID.ToString() == TemplateIDs.Template.ToString();
        }

        public static bool IsInConfigDatabase(this Item item)
        {
            if (Config.Database == item.Database.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsInItemTemplatePath(this Item item)
        {
            if (item.Paths.FullPath.ToLower()
                .Contains(Config.DefaultTemplateLocation.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}