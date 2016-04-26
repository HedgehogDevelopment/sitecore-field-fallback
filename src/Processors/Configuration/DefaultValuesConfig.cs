using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace FieldFallback.Processors.Configuration
{
    public class DefaultValuesConfig
    {

        /// <summary>
        ///     Gets the default template location.
        /// </summary>
        ///
        /// <value>
        ///     The default template location that is retrieved from the config file.
        /// </value>

        public static string DefaultTemplateLocation
        {
            get { return Settings.GetSetting("Fallback.Default.Template.Location"); }
        }

        /// <summary>
        ///     Gets the default item location.
        /// </summary>
        ///
        /// <value>
        ///     The default item location that is retrieved from the config file.
        /// </value>

        public static string DefaultItemLocation
        {
            get { return Settings.GetSetting("Fallback.Default.Item.Location"); }
        }

        /// <summary>
        ///     Gets the database name from the event config file
        ///     to compare the item that we are working with to.
        ///     We only work with the item if it is in the database
        ///     that is set in the config file
        /// </summary>
        ///
        /// <value>
        ///     The database set in the config file to work with 
        ///     the items.
        /// </value>

        public static string Database
        {
            get { return Settings.GetSetting("Fallback.Default.Database"); }
        }
        /// <summary>
        ///     Gets the content item suffix that will be used when creating
        ///     the content item based off of the template.
        /// </summary>
        ///
        /// <value>
        ///     The content item suffix appended to the Template Name.
        /// </value>

        public static string ContentItemSuffix
        {
            get { return Settings.GetSetting("Fallback.Default.ContentItemSuffix"); }
        }

        /// <summary>
        ///     Gets the Item ID of the fallback defaults folder from the
        ///     config file.
        /// </summary>
        ///
        /// <value>
        ///     The Item ID of the fallback defaults folder located under
        ///     .
        /// </value>

        public static string FallbackDefaultsFolderID
        {
            get { return Settings.GetSetting("Fallback.Default.Folder.ID"); }
        }
    }
}