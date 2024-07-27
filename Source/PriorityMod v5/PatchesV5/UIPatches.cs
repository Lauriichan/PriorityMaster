using HarmonyLib;
using PriorityMod.Tools;
using Verse;

namespace PriorityMod.PatchesV5
{
    public static class UIPatches
    {

        public static void Apply(Harmony harmony)
        {
            harmony.Patch(Reflection.Method("Widgets", "ResetSliderDraggingIDs"), prefix: PatchHelper.Method(() => OnResetPrefix()));
        }

        private static void OnResetPrefix()
        {
            WidgetUtil.ResetDragId();
        }

    }
}
