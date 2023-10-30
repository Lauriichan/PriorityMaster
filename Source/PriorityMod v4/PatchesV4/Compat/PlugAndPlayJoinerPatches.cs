using HarmonyLib;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PriorityMod.PatchesV4.Compat
{
    public static class PlugAndPlayJoinerPatches
    {

        public static void Apply(Harmony harmony)
        {
            if (!ModsConfig.IsActive("telardo.plugandplayjoiner"))
            {
                return;
            }
            PatchSettings.Disable_EnableAndInitialize_Patch();

            HarmonyMethod defaultPriorityTranspiler = PatchHelper.Method(() => PriorityPatches.DefaultPriorityTranspiler(null, null));
            harmony.Patch(Reflection.Method("PlugAndPlayJoiner.WorkPriorityInitTranspiler", "GetAlterPriority"), transpiler: PatchHelper.Method(() => GetAlterPrioDefaultPriorityTranspiler(null, null)));
            harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSetting", "PriorityByWorkTypeDefName").GetGetMethod(), transpiler: defaultPriorityTranspiler);
            harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSetting", "ProfessionalWorkPriorities").GetGetMethod(), transpiler: defaultPriorityTranspiler);

            harmony.Patch(Reflection.Method("PlugAndPlayJoiner.PlugAndPlayJoinerModHandler", "DoSettingsWindowContents"), transpiler: PatchHelper.Method(() => GetAlterPrioMaxPriorityTranspiler(null, null)));

        }

        public static IEnumerable<CodeInstruction> GetAlterPrioMaxPriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            { 
                if (list.Count != 0)
                {
                    CodeInstruction last = list.ElementAt(list.Count - 1);
                    if (last.opcode == OpCodes.Ldloc_S && instruction.opcode == OpCodes.Ldc_I4_4)
                    {
                        list.RemoveAt(list.Count - 1);
                        CodeInstruction newInstr = (new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")));
                        instruction.MoveLabelsTo(newInstr);
                        list.Add(newInstr);
                        list.Add(last);
                        continue;
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
