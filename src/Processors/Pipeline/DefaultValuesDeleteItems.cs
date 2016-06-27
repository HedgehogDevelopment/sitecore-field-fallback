using System.Collections.Generic;
using System.Linq;
using FieldFallback.Processors.Configuration;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Proxies;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace FieldFallback.Processors.Pipeline
{
    /// <summary>
    ///     This Pipeline Deletes Default Content Item.
    /// </summary>
    public class DefaultValuesDeleteItems
    {

        /// <summary>
        ///     Process the delete items described by args.
        /// </summary>
        /// <param name="args"> The arguments. </param>
        public void ProcessDeleteItems(ClientPipelineArgs args)
        {
            Item itemToDelete = GetItemToDelete(args);

            if (itemToDelete == null)
            {
                return;
            }

            Assert.IsNotNull(itemToDelete, "Template Item Deleting");
            ItemLink[] referrerItemLinks = Globals.LinkDatabase.GetReferrers(itemToDelete);
            if (referrerItemLinks.Count() > 2)
            {
                Assert.IsFalse(false, "Default Template Item Delete has child items", args);
                args.AbortPipeline();
            }

            // As a note we are not expecting more than 2 items
            foreach (Item referrerItem in referrerItemLinks.Select(referrer => referrer.GetSourceItem()))
            {
                if (referrerItem == null)
                {
                    Assert.IsNull(referrerItem, "Referrer Item", args);
                    args.AbortPipeline();
                }

                // If the referrer item is in the Defaults path, we need to delete it
                if (referrerItem.Paths.Path.StartsWith(DefaultValuesConfig.DefaultItemLocation))
                {
                    RecycleOrDeleteItem(referrerItem);
                }
            }
        }

        /// <summary>
        ///     Recyle or delete item.
        /// </summary>
        /// </remarks>
        /// <param name="referrerItem"> The referrer item. </param>
        private void RecycleOrDeleteItem(Item referrerItem)
        {
            if (Settings.RecycleBinActive)
            {
                Log.Audit(this, "Recycle item: {0}", AuditFormatter.FormatItem(referrerItem));
                referrerItem.Recycle();
            }
            else
            {
                Log.Audit(this, "Delete item: {0}", AuditFormatter.FormatItem(referrerItem));
                referrerItem.Delete();
            }
        }

        /// <summary>
        ///     Query if 'args' is fallback default template.
        /// </summary>
        /// <param name="args"> 
        ///     The arguments that were
        ///     passed in by the client. 
        /// </param>
        /// <returns>
        ///     true if fallback default template, false if not.
        /// </returns>
        public Item GetItemToDelete(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            List<Item> items = GetItems(args) as List<Item>;

            return items.FirstOrDefault(current => current.Paths.Path
                                .StartsWith(DefaultValuesConfig.DefaultTemplateLocation));
        }

        /// <summary>
        ///     Gets the items in this collection.
        /// </summary>
        /// <param name="args"> 
        ///     The arguments that were
        ///     passed in by the client. 
        /// </param>
        ///
        /// <returns>
        ///     An enumerator that allows foreach to be used to get 
        ///     the items in this collection.
        /// </returns>

        private static IEnumerable<Item> GetItems(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Database database = Factory.GetDatabase(args.Parameters["database"]);
            Assert.IsNotNull(database, typeof(Database), "Name: {0}", args.Parameters["database"]);

            ListString listString = new ListString(args.Parameters["items"], '|');

            return listString.Select(id => database.GetItem(id)).ToList();
        }
    }
}