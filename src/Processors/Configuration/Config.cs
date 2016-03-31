using Sitecore.Configuration;

namespace FieldFallback.Processors.Configuration
{
    public class Config
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
    }
}