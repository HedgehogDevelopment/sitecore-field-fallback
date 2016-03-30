using Sitecore.Configuration;

namespace FieldFallback.Configuration
{
    public static class Config
    {
        public static bool ProcessSystemFields
        {
            get
            {
                return Settings.GetBoolSetting("Fallback.ProcessSystemFields", false);
            }
        }

        public static string IgnoredFields
        {
            get
            {
                return Settings.GetSetting("Fallback.IgnoredFields");
            }
        }

        public static bool PreviewEnabled
        {
            get
            {
                return Settings.GetBoolSetting("Fallback.EnablePreview", true);
            }
        }

        public static bool EditEnabled
        {
            get
            {
                return Settings.GetBoolSetting("Fallback.EnablePageEditor", true);
            }
        }

        /// <summary>
        ///     Gets the default template location.
        /// </summary>
        ///
        /// <value>
        ///     The default template location that is retrieved from the config file.
        /// </value>

        public static string DefaultTemplateLocation
        {
            get { return Settings.GetSetting("Fallback.Default.Teamplate.Location"); }
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