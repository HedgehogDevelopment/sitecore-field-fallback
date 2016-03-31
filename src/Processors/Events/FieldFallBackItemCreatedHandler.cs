using System;
using FieldFallback.Processors.Configuration;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.SecurityModel;

namespace FieldFallback.Processors.Events
{
    public class FieldFallBackItemCreatedHandler
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            SitecoreEventArgs eventArgs
                = args as SitecoreEventArgs;

            Assert.IsNotNull(eventArgs, "eventArgs");

            if (eventArgs == null)
            {
                return;
            }

            Item item = eventArgs.Parameters[0] as Item;


            if (!isTemplate(item))
            {
                return;
            }

            //If our new item is in our default template location,
            // we are creating a template. Since we are creating a template,
            // we need to create a default item to match it.

            if (item.Paths.FullPath.ToLower().StartsWith(Config.DefaultTemplateLocation.ToLower()))
            {
                //Create a new item in the item location based off of the template
                //that was just created. 
                using (new SecurityDisabler())
                {
                    //Get the database context
                    Database context = item.Database;

                    //Get the parent item from the config folder
                    Item defaultParentItem = context.Items.GetItem(Config.DefaultItemLocation.ToLower());

                    defaultParentItem.Editing.BeginEdit();

                    //Create a new item based off of the template just created
                    string itemName = item.Name + " Defaults";

                    TemplateItem template = item.Database.GetTemplate(item.ID);
                    Item newItem = defaultParentItem.Add(itemName, template);

                    defaultParentItem.Editing.EndEdit();
                }
            }
        }

        private bool isTemplate(Item item)
        {
            if (item.TemplateID.ToString() == TemplateIDs.Template.ToString())
            {
                return true;
            }
            return false;
        }
    }
}