using HarmonyLib;
using Multiplayer.API;
using PriorityMod.Core;
using PriorityMod.Settings;
using PriorityMod.Tools;
using Verse;

namespace PriorityMod.PatchesV3.Compat
{
    public static class MultiplayerPatches
    {
        public static void Apply()
        {
            if (!ModsConfig.IsActive("rwmt.multiplayer"))
            {
                return;
            }
            if (!MP.enabled)
            {
                // If MP API is not initialized we can't do anything :/
                return;
            }
            AddModToIgnoredConfigList();

            MP.RegisterSyncField(typeof(PrioritySettings), "maxPriority").SetHostOnly();
            MP.RegisterSyncField(typeof(PrioritySettings), "defPriority").SetHostOnly();
        }

        private static void AddModToIgnoredConfigList()
        {
            var ignoredField = Reflection.Field("Multiplayer.Client.JoinData", "ignoredConfigsModIds", true);
            if (ignoredField == null) {
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
    }
}
