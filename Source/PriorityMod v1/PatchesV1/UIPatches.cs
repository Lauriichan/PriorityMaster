using Harmony;
using PriorityMod.Core;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PriorityMod.PatchesV1
{
    public static class UIPatches
    {

        public static void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(Reflection.Method("UnityGUIBugsFixer", "OnGUI"), postfix: PatchHelper.Method(() => OnGUIPostfix()));
        }

        private static void OnGUIPostfix()
        {
            WidgetUtil.ResetDragId();
        }

    }
}
