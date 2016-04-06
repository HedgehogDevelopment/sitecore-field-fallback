using System;
using System.Threading;
using FieldFallback.Processors.Configuration;
using FieldFallback.Processors.Extensions;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.SecurityModel;

namespace FieldFallback.Processors.Events
{
    /// <summary>
    ///     A field fall back item created handler.
    ///     This handler functions to create items based off
    ///     of templates that are created. It is documented
    ///     throughout. However, one key point is item:created
    ///     was chosen over item:saved specifically. Also note
    ///     that there is a background thread that runs on a
    ///     delay to create the item to ensure the SaveUI
    ///     Pipeline has time to process saving the TemplateItem
    /// </summary>
    public class FieldFallBackItemCreatedHandler
    {
        /// <summary>
        ///     Raises the item saved event.
        ///     This event will create an item based off of the
        ///     template in the same location in the content tree
        ///     as it is in the template tree.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="args">     Event information to send to registered event handlers. </param>
        public void OnItemSaved(object sender, EventArgs args)
        {
            // Get the item created event arguments.
            // We are using the item:Created event over the item:Saved event
            // The purpose of using the create over the save event is the 
            // complexity of the SaveUI Pipeline may cause adverse affects
            // to creating the defaults for the dynamic template creation
            ItemCreatedEventArgs eventArgs =
                Event.ExtractParameter(args, 0) as ItemCreatedEventArgs;

            Assert.IsNotNull(eventArgs, "eventArgs");

            if (eventArgs == null)
            {
                return;
            }

            Item item = eventArgs.Item;
            Assert.IsNotNull(item, "item");

            // Validate that the item is in the database
            // that is configured 
            if (!item.IsInConfigDatabase())
            {
                return;
            }

            // Check to see that the item is a template
            // If it is not a template, we do not need to 
            // process it.
            if (!(item.IsTemplate() || item.IsFolderTemplate()))
            {
                return;
            }

            //If our new item is in our default template location,
            // we are creating a template. Since we are creating a template,
            // we need to create a default item to match it.
            if (item.IsInItemTemplatePath())
            {
                // We are creating the new item in a new thread
                // This allows the Template time create and save
                // to the database.
                // It is important that the thread be ran in the
                // background AND we allow the Thread to sleep
                // giving time for the current process to complete.
                Thread createdThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Thread.Sleep(2000);

                    CreateContentItem(item);
                });

                createdThread.Start();
            }
        }

        /// <summary>
        ///     Creates content item based off of the template.
        /// </summary>
        /// <param name="item">
        ///     The TemplateItem that was just created.
        /// </param>
        private void CreateContentItem(Item item)
        {
            //Create a new item in the item location based off of the template
            //that was just created. 
            using (new SecurityDisabler())
            {
                //Get the database context
                Database context = item.Database;

                //Get the parent item of the item we need to create
                Item parentItem = context.Items.GetItem(item.GetDefaultContentPath());

                if (parentItem == null)
                {
                    Assert.IsNull(parentItem, "Fallback Item");
                }

                parentItem.Editing.BeginEdit();

                string newItemName = item.Name + Config.ContentItemSuffix;

                // Create a new item based off of the template 
                // just created     
                if (item.IsTemplate())
                {
                    //Get the template to base the template 
                    TemplateItem template = item.Database.GetTemplate(item.ID);
                    parentItem.Add(newItemName, template);
                }
                else if (item.IsFolderTemplate())
                {
                    //Get the base folder template
                    TemplateItem template = item.Database.GetItem(Config.FallbackDefaultsFolderID);
                    parentItem.Add(item.Name, template);
                }

                parentItem.Editing.EndEdit();
            }
        }
    }
}