using HarmonyLib;
using Multiplayer.API;
using PriorityMod.Core;
using PriorityMod.Settings;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using Verse;

namespace PriorityMod.PatchesV4.Multiplayer
{
    public static class MultiplayerCompat
    {

        private static IMPWatcher watcher = new NOPWatcher();

        public static void Initialize(PrioritySettings settings)
        {
            if (!ModsConfig.IsActive("rwmt.multiplayer"))
            {
                return;
            }
            RunMultiplayerSetup(settings);
        }

        private static void RunMultiplayerSetup(PrioritySettings settings)
        {
            if (!MP.enabled)
            {
                // If MP API is not initialized we can't do anything :/
                Log.Warning("[PriorityMaster - Multiplayer Patches] Multiplayer mod installed, but API not initialized!");
                return;
            }
            AddModToIgnoredConfigList();

            MPWatcherImpl watcherImpl = new MPWatcherImpl();

            watcherImpl.CreateField("max_priority", settings, "maxPriority").SetHostOnly().InGameLoop().PostApply((a, b) =>
            {
                settings.buffer.setChangeMax = true;
                settings.GetMaxPriority();
                DrawingTools.UpdateColors();
            });
            watcherImpl.CreateField("default_priority", settings, "defPriority").SetHostOnly().InGameLoop().PostApply((a, b) =>
            {
                settings.buffer.setChangeDef = true;
                settings.GetDefPriority();
            });

            MP.RegisterSyncWorker<PrioritySettings>(SyncSettingsWorker, typeof(PrioritySettings));

            watcher = watcherImpl;
            Log.Message("[PriorityMaster - Multiplayer Patches] Successfully applied");
        }

        private static void AddModToIgnoredConfigList()
        {
            var ignoredField = Reflection.Field("Multiplayer.Client.JoinData", "ignoredConfigsModIds", true);
            if (ignoredField == null)
            {
                Log.Warning("[PriorityMaster - Multiplayer Patches] Couldn't add mod to ignored mod configs for syncing - couldn't find field 'ignoredConfigsModIds");
                return;
            }
            if (!ignoredField.IsStatic)
            {
                Log.Warning("[PriorityMaster - Multiplayer Patches] Couldn't add mod to ignored mod configs for syncing - field is not static");
                return;
            }
            var value = ignoredField.GetValue(null);
            if (!(value is string[] configs))
            {
                Log.Warning("[PriorityMaster - Multiplayer Patches] Couldn't add mod to ignored mod configs for syncing  - target field did not return string array type - actual value (" + value.ToStringSafe() + ") of type " + ignoredField.FieldType.ToStringSafe());
                return;
            }
            ignoredField.SetValue(null, configs.AddToArray(PriorityMaster.MOD_ID.ToLower()));
        }

        private static void SyncSettingsWorker(SyncWorker worker, ref PrioritySettings settings)
        {
            if (worker.isWriting)
            {
                worker.Write<int>(settings.GetMaxPriority());
                worker.Write<int>(settings.GetDefPriority());
            } else
            {
                settings.SetMaxPriority(worker.Read<int>());
                settings.SetDefPriority(worker.Read<int>());
                DrawingTools.UpdateColors();
            }
        }

        public static IMPWatcher GetWatcher()
        {
            return watcher;
        }

    }

    internal class MPWatcherImpl : IMPWatcher
    {

        private readonly Dictionary<string, ISyncRef> sync = new Dictionary<string, ISyncRef>();

        public ISyncField CreateField(string id, Type type, string field)
        {
            if (sync.ContainsKey(id))
            {
                throw new ArgumentException($"Id ${id} is already in use");
            }
            ISyncField syncField = MP.RegisterSyncField(type, field);
            sync.Add(id, new SyncFieldRef(this, null, syncField));
            return syncField;
        }
        public ISyncField CreateField(string id, object instance, string field)
        {
            if (sync.ContainsKey(id))
            {
                throw new ArgumentException($"Id ${id} is already in use");
            }
            ISyncField syncField = MP.RegisterSyncField(instance.GetType(), field);
            sync.Add(id, new SyncFieldRef(this, instance, syncField));
            return syncField;
        }

        public bool CanChangeSettings()
        {
            return !MP.IsInMultiplayer || MP.IsHosting;
        }

        public bool ShouldWatch()
        {
            return MP.IsInMultiplayer;
        }

        public void BeginWatch()
        {
            if (!ShouldWatch())
            {
                return;
            }
            MP.WatchBegin();
        }

        public void EndWatch()
        {
            if (!ShouldWatch())
            {
                return;
            }
            MP.WatchEnd();
        }

        public ISyncRef Get(string id)
        {
            if (sync.TryGetValue(id, out ISyncRef syncField))
            {
                return syncField;
            }
            return null;
        }
    }

    internal class SyncFieldRef : ISyncRef
    {

        private readonly MPWatcherImpl parent;

        private readonly object instance;
        private readonly ISyncField field;

        public SyncFieldRef(MPWatcherImpl parent, object instance, ISyncField field) {
            this.parent = parent;
            this.instance = instance;
            this.field = field;
        }

        public void Watch()
        {
            if (!parent.ShouldWatch())
            {
                return;
            }
            field.Watch(instance);
        }
    }
}
