using HarmonyLib;
using PriorityMod.Core;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PriorityMod.PatchesV3
{
    public static class UIPatches
    {

        public static void Apply(Harmony harmony)
        {
            harmony.Patch(Reflection.Method("UnityGUIBugsFixer", "OnGUI"), postfix: PatchHelper.Method(() => OnGUIPostfix()));
        }

        private static void OnGUIPostfix()
        {
            WidgetUtil.ResetDragId();
        }

    }
}
