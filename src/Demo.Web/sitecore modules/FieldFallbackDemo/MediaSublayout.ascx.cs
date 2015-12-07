using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using ImageField = Sitecore.Data.Fields.ImageField;

namespace FieldFallback.Demo.Web.sitecore_modules.FieldFallbackDemo
{
    public partial class MediaSublayout : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Item currentItem = Sitecore.Context.Item;

            if (currentItem.Fields["Image"] != null && !string.IsNullOrEmpty(currentItem.Fields["Image"].Value))
            {
                ImageField currentImageField = currentItem.Fields["Image"];
                Item imageItem = currentImageField.MediaItem;
                litImageTitle.Text = imageItem.Fields["Title"].Value;
            }

        }
    }
}