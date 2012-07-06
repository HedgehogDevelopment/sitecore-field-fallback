using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.StringExtensions;

namespace FieldFallback.Processors.Globalization
{
    public class Config
    {
        public static string MasterLanguageName
        {
            get { return Settings.GetSetting("Fallback.MasterLanguage"); }
        }

        public static string EnforceVersionPresenceTemplates
        {
            get
            {
                return Settings.GetSetting("Fallback.EnforceVersionPresenceTemplates");
            }
        }

        public static Language MasterLanguage
        {
            get
            {
                Language parsedLang;
                var configLangName = MasterLanguageName;
                if (!configLangName.IsNullOrEmpty() && Language.TryParse(configLangName, out parsedLang))
                {
                    return parsedLang;
                }

                return null;
            }
        }

        public static IEnumerable<ID> EnforcedVersionTemplateIDs
        {
            get
            {
                var templateIds = Sitecore.MainUtil.RemoveEmptyStrings(Config.EnforceVersionPresenceTemplates.ToLower().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                return from templateId in templateIds where ID.IsID(templateId) select ID.Parse(templateId);
            }
        }
    }
}
