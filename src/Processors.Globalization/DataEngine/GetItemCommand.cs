using System;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;

namespace FieldFallback.Processors.Globalization.DataEngine
{
   public class GetItemCommand : Sitecore.Data.Engines.DataCommands.GetItemCommand
   {
      protected override Sitecore.Data.Engines.DataCommands.GetItemCommand CreateInstance()
      {
         return new GetItemCommand();
      }

      protected override Item DoExecute()
      {
         var item = base.DoExecute();

         if (   item != null &&
                !item.Name.StartsWith("__") &&
                !string.IsNullOrEmpty(item.Language.Name) &&
                VersionPresenceEnforced &&
                IsMadeFromEnforcedTemplate(item))
         {
             return IsEmptyVersion(item) ? null : item;
         }
         return item;
      }

      protected virtual bool IsMadeFromEnforcedTemplate(Item item)
      {
         var template = TemplateManager.GetTemplate(item.TemplateID, item.Database);
         return Config.EnforcedVersionTemplateIDs.Any(template.DescendsFromOrEquals);
      }

      protected bool IsEmptyVersion(Item item)
      {
         return item == null || item.Versions.Count == 0;
      }

      internal static bool VersionPresenceEnforced
      {
          get
          {
              return Context.Site != null &&
                     Context.Site.SiteInfo.Properties["enforceVersionPresence"] != null &&
                     Context.Site.SiteInfo.Properties["enforceVersionPresence"].Equals("true", StringComparison.InvariantCultureIgnoreCase);
          }
      }
   }
}
