﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using FieldFallback.Caching;
using FieldFallback.Configuration;
using FieldFallback.Logging;
using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Sites;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Engines.DataCommands;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;

namespace FieldFallback.Data
{
    public class FallbackValuesProvider : StandardValuesProvider
    {
        private SiteManager _siteManager;
        private IFallbackSupportCache _supportCache;

        /// <summary>
        /// Gets or sets the supported content paths.
        /// <para>Set via Config file reflection</para>
        /// </summary>
        /// <value>
        /// The supported content paths.
        /// </value>
        public string SupportedContentPaths { get; set; }

        /// <summary>
        /// Gets or sets the cache manager.
        /// <para>Set via Config file reflection</para>
        /// </summary>
        /// <value>The cache manager.</value>
        public IFallbackValueCache Cache { get; set; }

        /// <summary>
        /// Gets or sets the skip item cache.
        /// <para>Set via Config file reflection</para>
        /// </summary>
        /// <value>
        /// The skip item cache.
        /// </value>
        public ISkipItemCache SkipItemCache { get; set; }

        /// <summary>
        /// The logger.
        /// This will get instantiated via Sitecore configuration file reflection
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Supported database names.
        /// </summary>
        /// <value>The supported databases.</value>
        protected IEnumerable<string> SupportedDatabaseNames { get; private set; }

        /// <summary>
        /// Gets or sets the supported sites.
        /// </summary>
        /// <value>
        /// The supported sites.
        /// </value>
        protected IEnumerable<string> SupportedSiteNames { get; private set; }

        public FallbackValuesProvider(string databases, string sites)
        {
            Assert.IsNotNullOrEmpty(databases, "databases param cannot be null or empty");
            Assert.IsNotNullOrEmpty(sites, "databases param cannot be null or empty");

            SupportedDatabaseNames = databases.Split(new[] { '|', ' ', ',' });
            SupportedSiteNames = sites.Split(new[] { '|', ' ', ',' });

            _siteManager = new SiteManager();
            _supportCache = new FallbackSupportCache();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            if (!Settings.ConfigurationIsSet)
            {
                return;
            }

            EnableDatabases();

            EnableSites();
        }

        /// <summary>
        /// Gets the standard value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public sealed override string GetStandardValue(Field field)
        {
            Assert.ArgumentNotNull(field, "field");

            // normal standard values take priority over fallback
            string value = base.GetStandardValue(field);
            if (value != null)
            {
                return value;
            }

            // if fallback isn't supported for this field, then return the default standard values
            if (!IsFallbackSupported(field))
            {
                return value;
            }

            // lets try to get a fallback value for this field...
            return GetFallbackValue(field) ?? value;
        }

        /// <summary>
        /// Gets the fallback value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public virtual string GetFallbackValue(Field field)
        {
            Assert.ArgumentNotNull(field, "field");

            Item item = field.Item;
            Assert.ArgumentNotNull(item, "item");

            Debug(">> GetFallbackValue - s:{0} db:{1} i:{2} f:{3}", Sitecore.Context.GetSiteName(), item.Database.Name, item.ID, field.Name);
            Logger.PushIndent();

            string value = null;

            try
            {
                // test cache first
                value = Cache.GetFallbackValue(item, field);

                // we have a cached value.. return it.
                if (value != null)
                {
                    Debug("Value found in the cache");
                    return value;
                }

                // Call the fieldFallback pipeline
                var pipelineArgs = new FieldFallbackPipelineArgs(FieldFallbackPipelineArgs.Methods.Execute, field);
                Sitecore.Pipelines.CorePipeline.Run("fieldFallback", pipelineArgs);

                // if the args has a value, then one of the processors fell back to something
                if (pipelineArgs.HasFallbackValue)
                {
                    Debug("Fallback Found");

                    // put it in the cache
                    Cache.AddFallbackValues(item, field, pipelineArgs.FallbackValue);
                    return pipelineArgs.FallbackValue;
                }

                Debug("No fallback");
                return null;
            }
            finally
            {
                Logger.PopIndent();
                Debug("<< GetFallbackValue");
            }
        }

        public virtual bool FieldContainsFallbackValue(Field field, Language language)
        {
            // if fallback isn't supported for this field, then we wouldn't have a fallback value
            if (!IsFallbackSupported(field))
            {
                return false;
            }

            // If the field has some value stored in it, it isn't falling back
            // The abstract FieldFallbackProcessor does have this logic, but calling it here can bypass a lot of calls
            if (field.HasValueSafe() || field.ContainsStandardValue)
            {
                return false;
            }

            var pipelineArgs = new FieldFallbackPipelineArgs(Pipelines.FieldFallbackPipeline.FieldFallbackPipelineArgs.Methods.Query, field);
            Sitecore.Pipelines.CorePipeline.Run("fieldFallback", pipelineArgs);
            return pipelineArgs.HasFallbackValue;
        }

        private bool IsFallbackSupported(Field field)
        {
            if (FieldFallback.Data.FallbackDisabler.CurrentValue == FallbackStates.Disabled)
            {
                //Debug("@ Fallback Disabled by Disabler [{0}:{1}]", field.Item.Name, field.Name);
                return false;
            }

            Assert.ArgumentNotNull(field, "field");

            if (IsIgnoredField(field))
            {
                //Debug("@ Field {0} is ignored", field.Name);
                return false;
            }

            // Check the cache to see if this field is supported
            if (_supportCache.GetFallbackSupport(field).HasValue)
            {
                //Debug("@ IsFallbackSupported (cache hit - {0}) ", SupportCache.GetFallbackSupport(field).Value);
                return _supportCache.GetFallbackSupport(field).Value;
            }

            Item item = field.Item;
            Assert.ArgumentNotNull(item, "item");

            // see if we know this item should be skipped
            if (SkipItemCache.IsItemSkipped(item))
            {
                //Debug("@ IsFallbackSupported (SkipItemCache cache hit) ");
                return false;
            }

            bool isSupported = true;

            Debug(">> IsFallbackSupported - s:{0} db:{1} i:{2} f:{3}", Sitecore.Context.GetSiteName(), item.Database.Name, item.ID, field.Name);
            Logger.PushIndent();

            if (!_siteManager.IsFallbackEnabledForDisplayMode(Sitecore.Context.Site))
            {
                Debug("Fallback is not enabled for current page mode");
                isSupported = false;
            }
            else if (!_siteManager.IsFallbackEnabled(Sitecore.Context.Site.SiteInfo))
            {
                Debug("Fallback is not enabled for site {0}", Sitecore.Context.Site.Name);
                isSupported = false;
            }
            else if (!IsItemInSupportedDatabase(item))
            {
                Debug("Item database '{0}' not valid.", item.Database.Name);
                isSupported = false;

                // lets cache this item as skipped to prevent future checks on it
                SkipItemCache.SetSkippedItem(item);
            }
            else if (!IsItemInSupportedContentPath(item)) // it must be under /sitecore/content
            {
                Debug("Item {0} is in an invalid path", item.Name);
                isSupported = false;

                // lets cache this item as skipped to prevent future checks on it
                SkipItemCache.SetSkippedItem(item);
            }

            _supportCache.SetFallbackSupport(field, isSupported);
            Logger.PopIndent();
            Debug("<< IsFallbackSupported: {0}", isSupported);
            return isSupported;
        }

        private bool IsItemInSupportedDatabase(Item item)
        {
            if (item == null)
            {
                return false;
            }

            // Is the database the item is in within the supported databases?
            return SupportedDatabaseNames.Contains(item.Database.Name, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsItemInSupportedContentPath(Item item)
        {
            // get the path once!
            // Each call to `item.Paths.Path` will walk up the tree
            string itemPath = item.Paths.Path.ToLower();

            // `item.Paths.IsContentItem` is what we can use, but again, this will walk up the tree each time
            bool isContentItem = itemPath.StartsWith("/sitecore/content/", StringComparison.OrdinalIgnoreCase);

            // the item must be a content item.
            if (!isContentItem)
            {
                return false;
            }

            // if there is no configured value...
            if (string.IsNullOrEmpty(SupportedContentPaths))
            {
                return isContentItem;
            }

            // there could be multiple values specified
            IEnumerable<string> paths = SupportedContentPaths.Split(new[] { '|', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // but there isn't...
            if (paths.Count() <= 0)
            {
                return isContentItem;
            }

            // see if the item's path starts with a configured value
            return paths.Any(path => itemPath.StartsWith(path.ToLower()));
        }

        private bool IsIgnoredField(Field field)
        {
            // ignoring __Source field that is a part of Item Cloning functionality
            // if this field is processed, the SV provider goes into infinite loop
            if (field.ID.Equals(Sitecore.FieldIDs.Source))
            {
                return true;
            }

            // if configuration is set to process system fields - return false
            if ((!Config.ProcessSystemFields) && field.IsSystem())
            {
                return true;
            }

            return Config.IgnoredFields.Contains(field.ID.ToString());
        }

        private void EnableDatabases()
        {
            foreach (var dbName in SupportedDatabaseNames)
            {
                Database database = Factory.GetDatabase(dbName);

                if (database == null)
                {
                    continue;
                }

                InitializeEventHandlers(database);
            }
        }

        private void EnableSites()
        {
            // add properties to each supported site definition
            foreach (string siteName in SupportedSiteNames)
            {
                Logger.Info("FallbackProvider enabled for the '{0}' site.", siteName);
                _siteManager.EnableSite(siteName);
            }
        }

        /// <summary>
        /// Writes the specified message to the ILogger.Debug method.
        /// <para>This method is only called when the DEBUG compilation symbol is defined.</para>
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        [Conditional("DEBUG")]
        private void Debug(string message, params object[] args)
        {
            Logger.Debug(message, args);
        }

        /// <summary>
        /// Initializes the event handlers.
        /// </summary>
        /// <param name="database">The database.</param>
        private void InitializeEventHandlers(Database database)
        {
            Logger.Info("Instatiating event handlers for database: {0}", database.Name);
            Sitecore.Data.Engines.DataEngine dataEngine = database.Engines.DataEngine;

            // Hook into the Delete/Remove/Saved events to clear caches
            dataEngine.DeletedItem += DataEngine_DeletedItem;
            dataEngine.RemovedVersion += DataEngine_RemoveVersion;

            /* 
             * When creating an item from a branch template we get fields with empty values (not null)
             * So, we need to bail out if we are editing, since when an item is created from a branch template
             * we are in Editing mode. (Item.Editing.IsEditing)
             * - However, when saving an item this now causes fallback values to get saved on the item
             * - item.Editing.IsEditing also breaks validators
             * 
             * What we do is on the 'pre' events we disable fallback then on the 'post' event we re-enable them
             */

            dataEngine.CopyingItem += DataEngine_CopyingItem;
            dataEngine.CopiedItem += DataEngine_CopiedItem;
            dataEngine.SavingItem += DataEngine_SavingItem;
            dataEngine.SavedItem += DataEngine_SavedItem;
            dataEngine.CreatingItem += DataEngine_CreatingItem;
            dataEngine.CreatedItem += DataEngine_CreatedItem;
            dataEngine.AddingFromTemplate += DataEngine_AddingFromTemplate;
            dataEngine.AddedFromTemplate += DataEngine_AddedFromTemplate;
        }

        private void DataEngine_RemoveVersion(object sender, ExecutedEventArgs<RemoveVersionCommand> e)
        {
            Cache.RemoveItem(e.Command.Item);
        }

        private void DataEngine_DeletedItem(object sender, ExecutedEventArgs<DeleteItemCommand> e)
        {
            Cache.RemoveTree(e.Command.Item);
            SkipItemCache.UnSkipItem(e.Command.Item);
        }

        private void DataEngine_CopyingItem(object sender, ExecutingEventArgs<CopyItemCommand> e)
        {
            Debug(">> Copying item '{0}'", e.Command.Source.Name);
            Logger.PushIndent();
            FieldFallback.Data.FallbackDisabler.Enter(FallbackStates.Disabled);
        }

        private void DataEngine_CopiedItem(object sender, ExecutedEventArgs<CopyItemCommand> e)
        {
            //! Sitecore Bug 369065 - When an item is duplicated (copy/pasted) in Sitecore
            //          Sitecore raises the events in this order Copying (source) > Created (new) > Copied (source). 
            //          It doesn't raise the Creating (new) Event
            if (FieldFallback.Data.FallbackDisabler.GetStack(false) != null && FieldFallback.Data.FallbackDisabler.GetStack(false).Count > 0)
            {
                FieldFallback.Data.FallbackDisabler.Exit();
            }
            else
            {
                Debug("! Sitecore Bug 369065. Creating Item wasn't raised and we exited out of order.");
            }

            Logger.PopIndent();
            Debug("<< Copied item '{0}'", e.Command.Source.Name);
        }

        private void DataEngine_SavingItem(object sender, ExecutingEventArgs<SaveItemCommand> e)
        {
            Debug(">> Saving item '{0}'", e.Command.Item.Name);
            Logger.PushIndent();
            FieldFallback.Data.FallbackDisabler.Enter(FallbackStates.Disabled);
        }

        private void DataEngine_SavedItem(object sender, ExecutedEventArgs<SaveItemCommand> e)
        {
            if (e.Command.Changes.HasFieldsChanged)
            {
                foreach (FieldChange change in e.Command.Changes.FieldChanges)
                {
                    Cache.RemoveItemField(e.Command.Item, change.FieldID);
                }
            }

            Logger.PopIndent();
            Debug("<< Saved item '{0}'", e.Command.Item.Name);
            FieldFallback.Data.FallbackDisabler.Exit();
        }

        private void DataEngine_CreatingItem(object sender, ExecutingEventArgs<CreateItemCommand> e)
        {
            Debug(">> Creating item '{0}'", e.Command.ItemName);
            Logger.PushIndent();
            FieldFallback.Data.FallbackDisabler.Enter(FallbackStates.Disabled);
        }

        private void DataEngine_CreatedItem(object sender, ExecutedEventArgs<CreateItemCommand> e)
        {
            Logger.PopIndent();
            Debug("<< Created item '{0}'", e.Command.ItemName);
            FieldFallback.Data.FallbackDisabler.Exit();
        }

        private void DataEngine_AddingFromTemplate(object sender, ExecutingEventArgs<AddFromTemplateCommand> e)
        {
            Debug(">> Adding From Template item '{0}'", e.Command.ItemName);
            Logger.PushIndent();
            FieldFallback.Data.FallbackDisabler.Enter(FallbackStates.Disabled);
        }

        private void DataEngine_AddedFromTemplate(object sender, ExecutedEventArgs<AddFromTemplateCommand> e)
        {
            Logger.PopIndent();
            Debug("<< Added From Template item '{0}'", e.Command.ItemName);
            FieldFallback.Data.FallbackDisabler.Exit();
        }
    }
}
