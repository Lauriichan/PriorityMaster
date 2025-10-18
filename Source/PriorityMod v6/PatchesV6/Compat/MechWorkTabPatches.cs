using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Verse;
using PriorityMod.Tools;

namespace PriorityMod.PatchesV6.Compat
{
    public static class MechWorkTabPatches
    {

        public static void Apply(Harmony harmony)
        {
            if (!ModsConfig.IsActive("SpaceMoth.MechTab"))
            {
                return;
            }
            harmony.Patch(Reflection.Method("SM_MechTab.WidgetMechWork", "DrawMechWorkBoxFor"), transpiler: PatchHelper.Method(() => PriorityValueTranspiler(null, null)));
            harmony.Patch(Reflection.Method("SM_MechTab.WidgetMechWork", "TipForPawnWorker"), transpiler: PatchHelper.Method(() => TipForPawnWorkerTranspiler(null, null)));
            harmony.Patch(Reflection.Method("SM_MechTab.SM_PawnColumnWorker_WorkPriority", "HeaderClicked"), transpiler: PatchHelper.Method(() => PriorityRedirectTranspiler(null, null)));
        }
        public static IEnumerable<CodeInstruction> TipForPawnWorkerTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            bool done = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!done && instruction.opcode == OpCodes.Call)
                {
                    MethodInfo info = instruction.operand as MethodInfo;
                    if (info.Name == "Translate" && info.DeclaringType.Name == "Translator")
                    {
                        list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("DrawingTools", "GetTaggedStringFromPriorityString")).MoveLabelsFrom(instruction));
                        done = true;
                        continue;
                    }
                }
                list.Add(instruction);
            }
            return list;
        }

        private static IEnumerable<CodeInstruction> PriorityValueTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            CodeInstruction last = null;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_4 || instruction.opcode == OpCodes.Ldc_I4_3)
                {
                    last = instruction;
                }
                else if (last != null && instruction.opcode != OpCodes.Call)
                {
                    list.Pop();
                    if (last.opcode == OpCodes.Ldc_I4_4)
                    {
                        list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")).MoveLabelsFrom(last));
                    }
                    else if (last.opcode == OpCodes.Ldc_I4_3)
                    {
                        list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetDefaultPriority")).MoveLabelsFrom(last));
                    }
                    last = null;
                }
                else
                {
                    last = null;
                }
                list.Add(instruction);
            }
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
                        if (field.Name.Equals("maxPriority"))
                        {
                            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method(typeof(PatchHook), "GetMaximumPriority")).MoveLabelsFrom(instruction));
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
