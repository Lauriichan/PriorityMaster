using HarmonyLib;
using PriorityMod.Tools;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PriorityMod.PatchesV5.Compat
{
    public static class FluffyWorkTabPatches
    {

        public static void Apply(Harmony harmony)
        {
            if (!ModsConfig.IsActive("Fluffy.WorkTab"))
            {
                return;
            }

            harmony.Patch(Reflection.Method("WorkTab.Settings", "DoWindowContents"), transpiler: PatchHelper.Method(() => OptionsTranspiler(null, null)));

            harmony.Patch(Reflection.Method("WorkTab.DrawUtilities", "ColorOfPriority"), transpiler: PatchHelper.Method(() => PriorityPatches.PriorityColorTranspiler(null, null)));
            harmony.Patch(Reflection.Method("WorkTab.DrawUtilities", "PriorityLabel"), transpiler: PatchHelper.Method(() => LabelTranspiler(null, null)));

            HarmonyMethod redirectTranspiler = PatchHelper.Method(() => PriorityRedirectTranspiler(null, null));
            harmony.Patch(Reflection.Constructors("WorkTab.WorkPriority").Last(), transpiler: redirectTranspiler);
            harmony.Patch(Reflection.Method("WorkTab.PawnColumnWorker_WorkTabLabel", "Decrement"), transpiler: redirectTranspiler);
            harmony.Patch(Reflection.Method("WorkTab.PawnColumnWorker_WorkTabLabel", "Increment"), transpiler: redirectTranspiler);
            harmony.Patch(Reflection.Method("WorkTab.PriorityTracker", "SetPriority", new Type[] { typeof(WorkGiverDef), typeof(int), typeof(int), typeof(bool) }), transpiler: redirectTranspiler);
            harmony.Patch(Reflection.Method("WorkTab.PawnColumnWorker_WorkGiver", "HandleInteractionsToggle"), transpiler: redirectTranspiler);
            harmony.Patch(Reflection.Method("WorkTab.PawnColumnWorker_WorkType", "HandleInteractionsToggle"), transpiler: redirectTranspiler);
        }

        public static string PatchedPriorityLabel(int priority)
        {
            return ("Priority" + priority).Translate().Colorize(DrawingTools.GetColorFromPriority(priority));
        }

        private static IEnumerable<CodeInstruction> OptionsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            int idx = 0;
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions) {
                if (idx++ > 3 && idx <= 30)
                {
                    continue;
                }
                list.Add(instruction);
            }
            return list;
        }

        private static IEnumerable<CodeInstruction> LabelTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            list.Add(new CodeInstruction(OpCodes.Ldarg_0));
            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PriorityMod.PatchesV4.Compat.FluffyWorkTabPatches", "PatchedPriorityLabel", new Type[] { typeof(int) })));
            list.Add(new CodeInstruction(OpCodes.Ret));
            return list;
        }

        private static IEnumerable<CodeInstruction> PriorityRedirectTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldsfld)
                {
                    FieldInfo field = (FieldInfo)instruction.operand;
                    if (field != null)
                    {
                        if (field.Name.Equals("defaultPriority"))
                        {
                            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetDefaultPriority")));
                            continue;
                        }
                        else if (field.Name.Equals("maxPriority"))
                        {
                            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")));
                            continue;
                        }
                    }
                }
                list.Add(instruction);
            }
            return list;
        }

    }
}
