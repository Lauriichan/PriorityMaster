using HarmonyLib;
using PriorityMod.Core;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PriorityMod.PatchesV4
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
