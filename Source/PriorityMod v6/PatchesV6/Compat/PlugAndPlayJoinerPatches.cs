using HarmonyLib;
using PriorityMod.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace PriorityMod.PatchesV6.Compat
{
    public static class PlugAndPlayJoinerPatches
    {

        public static void Apply(Harmony harmony)
        {
            bool continued = false;
            if (!ModsConfig.IsActive("telardo.plugandplayjoiner") && !(continued = ModsConfig.IsActive("Mlie.PlugAndPlayJoiner")))
            {
                return;
            }
            PatchSettings.Disable_EnableAndInitialize_Patch();

            HarmonyMethod defaultPriorityTranspiler = PatchHelper.Method(() => PriorityPatches.DefaultPriorityTranspiler(null, null));
            if (continued)
            {
                harmony.Patch(Reflection.Method("PlugAndPlayJoiner.Pawn_WorkSettings_EnableAndInitialize", "GetAlterPriority"), transpiler: PatchHelper.Method(() => GetAlterPrioDefaultPriorityTranspiler(null, null)));
                harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSetting", "PriorityByWorkTypeDefName").GetGetMethod(), transpiler: defaultPriorityTranspiler);
                harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSetting", "ProfessionalWorkPriorities").GetGetMethod(), transpiler: defaultPriorityTranspiler);

                harmony.Patch(Reflection.Method("PlugAndPlayJoiner.PlugAndPlayJoinerModHandler", "DoSettingsWindowContents"), transpiler: PatchHelper.Method(() => GetAlterPrioMaxPriorityTranspiler(null, null)));
            } else
            {
                harmony.Patch(Reflection.Method("PlugAndPlayJoiner.WorkPriorityInitTranspiler", "GetAlterPriority"), transpiler: PatchHelper.Method(() => GetAlterPrioDefaultPriorityTranspiler(null, null)));
                harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSetting", "PriorityByWorkTypeDefName").GetGetMethod(), transpiler: defaultPriorityTranspiler);
                harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSetting", "ProfessionalWorkPriorities").GetGetMethod(), transpiler: defaultPriorityTranspiler);

                harmony.Patch(Reflection.Method("PlugAndPlayJoiner.PlugAndPlayJoinerModHandler", "DoSettingsWindowContents"), transpiler: PatchHelper.Method(() => GetAlterPrioMaxPriorityTranspiler(null, null)));
            }
        }

        public static IEnumerable<CodeInstruction> GetAlterPrioMaxPriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            { 
                if (list.Count > 1)
                {
                    CodeInstruction last1 = list.ElementAt(list.Count - 2);
                    CodeInstruction last2 = list.ElementAt(list.Count - 1);
                    if (last1.opcode == OpCodes.Ldloc_S && last2.opcode == OpCodes.Ldc_I4_4 && instruction.opcode == OpCodes.Ble_S)
                    {
                        list.RemoveAt(list.Count - 1);
                        CodeInstruction newInstr = (new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")));
                        last2.MoveLabelsTo(newInstr);
                        list.Add(newInstr);
                    }
                }
                list.Add(instruction);
            }
            return list;
        }

        public static IEnumerable<CodeInstruction> GetAlterPrioDefaultPriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                if (list.Count != 0)
                {
                    CodeInstruction last = list.ElementAt(list.Count - 1);
                    if (last.opcode == OpCodes.Ldc_I4_3 && (instruction.opcode == OpCodes.Ret || instruction.opcode == OpCodes.Stloc_0))
                    {
                        list.RemoveAt(list.Count - 1);
                        CodeInstruction newInstr = new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetDefaultPriority"));
                        last.MoveLabelsTo(newInstr);
                        list.Add(newInstr);
                    }
                }
                list.Add(instruction);
            }
            return list;
        }

    }
}
