using HarmonyLib;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PriorityMod.PatchesV6.Compat
{
    public static class CentricWorkPatches
    {

        public static void Apply(Harmony harmony)
        {
            if (!ModsConfig.IsActive("sirreality.PawnCentricWorkPriorities"))
            {
                return;
            }
            harmony.Patch(Reflection.Property("WorkUI", "MaxWorkPriority").GetGetMethod(), transpiler: PatchHelper.Method(() => MaxWorkPriorityTranspiler(null, null)));
            harmony.Patch(Reflection.Method("WorkUI", "ColorOfPriority"), transpiler: PatchHelper.Method(() => PriorityPatches.PriorityColorTranspiler(null, null)));
        }

        private static IEnumerable<CodeInstruction> MaxWorkPriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")));
            list.Add(new CodeInstruction(OpCodes.Ret));
            return list;
        }

    }
}
