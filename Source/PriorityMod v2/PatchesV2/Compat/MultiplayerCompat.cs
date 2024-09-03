using HarmonyLib;
using Multiplayer.API;
using PriorityMod.Core;
using PriorityMod.Settings;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using Verse;

namespace PriorityMod.PatchesV2.Compat
{
    public static class MultiplayerCompat
    {
        private static readonly Dictionary<string, Action> watchActions = new Dictionary<string, Action>();

        private static Action beginWatchAction = () => { };
        private static Action endWatchAction = () => { };

        private static Func<bool> IsInMultiplayerFunc = () => false;
        private static Func<bool> IsHostingFunc = () => false;

        private static int maximumPriority;
        private static int defaultPriority;

        public static void Initialize(Harmony harmony)
        {
            if (!ModsConfig.IsActive("rwmt.multiplayer"))
            {
                return;
            }
            RunMultiplayerSetup(harmony);
        }

        public static bool IsInMultiplayer()
        {
            return IsInMultiplayerFunc.Invoke();
        }

        public static bool CanChangeSettings()
        {
            return !IsInMultiplayerFunc.Invoke() || IsHostingFunc.Invoke();
        }

        public static int GetMaximumPriority() 
        { 
            return maximumPriority; 
        }

        public static void SetMaximumPriority(int value) 
        { 
            maximumPriority = value;
        }

        public static int GetDefaultPriority() 
        {  
            return defaultPriority;
        }

        public static void SetDefaultPriority(int value) 
        { 
            defaultPriority = value; 
        }

        public static void BeginWatch()
        {
            if (!IsInMultiplayerFunc.Invoke())
            {
                return;
            }
            beginWatchAction.Invoke();
        }

        public static void Watch(string id)
        {
            if (!IsInMultiplayerFunc.Invoke())
            {
                return;
            }
            if (watchActions.TryGetValue(id, out var action))
            {
                action.Invoke();
            }
        }

        public static void EndWatch()
        {
            if (!IsInMultiplayerFunc.Invoke())
            {
                return;
            }
            endWatchAction.Invoke();
        }

        /*
            Private
        */

        private static void RunMultiplayerSetup(Harmony harmony)
        {
            if (!MP.enabled)
            {
                // If MP API is not initialized we can't do anything :/
                Log.Warning("[PriorityMaster - Multiplayer Compat] Multiplayer mod installed, but API not initialized!");
                return;
            }

            AddModToIgnoredConfigList();

            IsInMultiplayerFunc = () => MP.IsInMultiplayer;
            IsHostingFunc = () => MP.IsHosting;

            beginWatchAction = () => MP.WatchBegin();
            endWatchAction = () => MP.WatchEnd();

            MP.RegisterSyncMethod(typeof(MultiplayerCompat), nameof(SetMaximumPriority)).SetHostOnly().SetPostInvoke((a, b) =>
            {
                DrawingTools.UpdateColors();
            });
            MP.RegisterSyncMethod(typeof(MultiplayerCompat), nameof(SetDefaultPriority)).SetHostOnly();

            harmony.Patch(Reflection.Method("Multiplayer.Client.HostUtil", "HostServer"), postfix: PatchHelper.Method(() => InitializeSettings()));
            harmony.Patch(Reflection.Method("Multiplayer.Client.Comp.MultiplayerGameComp", "ExposeData"), postfix: PatchHelper.Method(() => SyncSettings()));

            Log.Message("[PriorityMaster - Multiplayer Compat] Setup was successful");
        }

        private static void AddModToIgnoredConfigList()
        {
            var ignoredField = Reflection.Field("Multiplayer.Client.JoinData", "ignoredConfigsModIds", true);
            if (ignoredField == null)
            {
                Log.Warning("[PriorityMaster - Multiplayer Compat] Couldn't add mod to ignored mod configs for syncing - couldn't find field 'ignoredConfigsModIds");
                return;
            }
            if (!ignoredField.IsStatic)
            {
                Log.Warning("[PriorityMaster - Multiplayer Compat] Couldn't add mod to ignored mod configs for syncing - field is not static");
                return;
            }
            var value = ignoredField.GetValue(null);
            if (!(value is string[] configs))
            {
                Log.Warning("[PriorityMaster - Multiplayer Compat] Couldn't add mod to ignored mod configs for syncing  - target field did not return string array type - actual value (" + value.ToStringSafe() + ") of type " + ignoredField.FieldType.ToStringSafe());
                return;
            }
            ignoredField.SetValue(null, configs.AddToArray(PriorityMaster.MOD_ID.ToLower()));
        }

        /*
            Patches
        */

        private static void InitializeSettings()
        {
            maximumPriority = PriorityMaster.settings.GetUserMaxPriority();
            defaultPriority = PriorityMaster.settings.GetUserDefPriority();
        }

        private static void SyncSettings()
        {
            Scribe_Values.Look(ref maximumPriority, PriorityMaster.NAMESPACED_KEY + ".maximum_priority", PrioritySettings.VALUE_MAX_PRIORITY_DEFAULT);
            Scribe_Values.Look(ref defaultPriority, PriorityMaster.NAMESPACED_KEY + ".default_priority", PrioritySettings.VALUE_DEF_PRIORITY_DEFAULT);
        }

    }
}
