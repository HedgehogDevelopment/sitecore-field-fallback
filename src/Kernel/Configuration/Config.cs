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
    }
}
