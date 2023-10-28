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
            Log.Message("GetAlterPriority");
            harmony.Patch(Reflection.Method("PlugAndPlayJoiner.WorkPriorityInitTranspiler", "GetAlterPriority"), transpiler: defaultPriorityTranspiler);
            Log.Message("PriorityByWorkTypeDefName");
            harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSettings", "PriorityByWorkTypeDefName").GetGetMethod(), transpiler: defaultPriorityTranspiler);
            Log.Message("ProfessionalWorkPriorities");
            harmony.Patch(Reflection.Property("PlugAndPlayJoiner.PlugAndPlayJoinerModSettings", "ProfessionalWorkPriorities").GetGetMethod(), transpiler: defaultPriorityTranspiler);

            Log.Message("DoSettingsWindowContents");
            harmony.Patch(Reflection.Method("PlugAndPlayJoiner.PlugAndPlayJoinerModHandler", "DoSettingsWindowContents"), transpiler: PatchHelper.Method(() => GetAlterPrioDefaultPriorityTranspiler(null, null)));

        }

        public static IEnumerable<CodeInstruction> GetAlterPrioDefaultPriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            { 
                if (list.Count != 0)
                {
                    CodeInstruction last = list.ElementAt(list.Count - 1);
                    if (last.opcode == OpCodes.Ldloc_S && instruction.opcode == OpCodes.Ldc_I4_4)
                    {
                        list.Add(new CodeInstruction(OpCodes.Callvirt, Reflection.Method("PatchHook", "GetMaximumPriority")));
                        continue;
                    }
                }
                list.Add(instruction);
            }
            StringBuilder builder = new StringBuilder();
            int idx = 0;
            foreach (CodeInstruction ins in list)
            {
                builder.AppendLine((idx++) + ": " + ins.ToString());
            }
            Log.Error(builder.ToString());
            return list;
        }

    }
}
