using FieldFallback.Processors.Configuration;
using Sitecore;
using Sitecore.Data.Items;

namespace FieldFallback.Processors.Data.Items
{
    public static class DefaultValuesItem
    {
        public static string GetFullContentItemPath(TemplateItem item)
        {
            // Path of content item dynamically created by 
            // FieldFallback Processor Event
            string itemPath = string.Format("{0}/{1}{2}",
                GetDefaultContentPath(item),
                item.Name,
                DefaultValuesConfig.ContentItemSuffix);

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
        public static string GetDefaultContentPath(Item item)
        {
            string itemPath =
                item.Paths.ParentPath
                    //Remove the template path
                    .Substring(DefaultValuesConfig.DefaultTemplateLocation.Length)
                    //Insert the item location
                    .Insert(0, DefaultValuesConfig.DefaultItemLocation);

            return itemPath;
        }

        /// <summary>
        ///     Query if 'item' is TemplateItem or a TemplateFolder .
        /// </summary>
        /// <param name="item">
        ///     The Sitecore item to evaluate
        ///     if it is a template.
        /// </param>
        /// <returns>
        ///     true if the item is a TemplateItem or TemplateFolder, 
        ///     false if it is not.
        /// </returns>
        public static bool IsTemplate(Item item)
        {
            //Check if item it a TemplateItem
            return item.TemplateID.ToString() == TemplateIDs.Template.ToString();
        }

        public static bool IsFolderTemplate(Item item)
        {
            //Check if item it a TemplateFolder
            return item.TemplateID.ToString() == TemplateIDs.TemplateFolder.ToString();
        }

        public static bool IsInConfigDatabase(Item item)
        {
            return DefaultValuesConfig.Database == item.Database.Name;
        }

        public static bool IsInItemTemplatePath(Item item)
        {
            return item.Paths.FullPath.ToLower()
                .Contains(DefaultValuesConfig.DefaultTemplateLocation.ToLower());
        }
    }
}