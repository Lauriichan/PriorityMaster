using HarmonyLib;
using PriorityMod.Core;
using PriorityMod.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PriorityMod.PatchesV6
{
    public static class PriorityPatches
    {

        public static void Apply(Harmony harmony)
        {
            HarmonyMethod defaultPriorityTranspiler = PatchHelper.Method(() => DefaultPriorityTranspiler(null, null));
            if (PatchSettings.Patch_GetPriority)
            {
                harmony.Patch(Reflection.Method("Pawn_WorkSettings", "GetPriority"), transpiler: defaultPriorityTranspiler);
            }
            if (PatchSettings.Patch_EnableAndInitialize)
            {
                harmony.Patch(Reflection.Method("Pawn_WorkSettings", "EnableAndInitialize"), transpiler: defaultPriorityTranspiler);
            }

            harmony.Patch(Reflection.Method("Autotests_ColonyMaker", "MakeColonists"), transpiler: defaultPriorityTranspiler);
            harmony.Patch(Reflection.Method("GameInitData", "PrepForMapGen"), transpiler: defaultPriorityTranspiler);

            harmony.Patch(Reflection.Method("Pawn_WorkSettings", "SetPriority"), transpiler: PatchHelper.Method(() => MaximumPriorityTranspiler(null, null)));
            harmony.Patch(Reflection.Method("PawnColumnWorker_WorkPriority", "HeaderClicked"), transpiler: PatchHelper.Method(() => PriorityTranspiler(null, null)));

            harmony.Patch(Reflection.Method("WidgetsWork", "ColorOfPriority"), transpiler: PatchHelper.Method(() => PriorityColorTranspiler(null, null)));
            harmony.Patch(Reflection.Method("WidgetsWork", "TipForPawnWorker"), transpiler: PatchHelper.Method(() => TipForPawnWorkerTranspiler(null, null)));
            harmony.Patch(Reflection.Method("WidgetsWork", "DrawWorkBoxFor"), transpiler: PatchHelper.Method(() => DrawWorkBoxTranspiler(null, null)));

            harmony.Patch(Reflection.Constructors("Pawn_WorkSettings").First(), prefix: PatchHelper.Method<Object>((obj) => PriorityPatches.PatchPriorityFields(ref obj)));

        }

        /*
         * 
         * 
         * 
         * 
         */

        public static IEnumerable<CodeInstruction> MaximumPriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_4)
                {
                    list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")));
                    continue;
                }
                list.Add(instruction);
            }
            return list;
        }

        public static IEnumerable<CodeInstruction> DefaultPriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_3)
                {
                    list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetDefaultPriority")));
                    continue;
                }
                list.Add(instruction);
            }
            return list;
        }

        public static IEnumerable<CodeInstruction> PriorityTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_4)
                {
                    list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")));
                    continue;
                }
                if (instruction.opcode == OpCodes.Ldc_I4_3)
                {
                    list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetDefaultPriority")));
                    continue;
                }
                list.Add(instruction);
            }
            return list;
        }

        /*
         * 
         * 
         * 
         * 
         */

        public static IEnumerable<CodeInstruction> PriorityColorTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            list.Add(new CodeInstruction(OpCodes.Ldarg_0));
            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("DrawingTools", "GetColorFromPriority", new Type[] { typeof(int) })));
            list.Add(new CodeInstruction(OpCodes.Ret));
            return list;
        }
        public static IEnumerable<CodeInstruction> TipForPawnWorkerTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            bool done = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!done && instruction.opcode == OpCodes.Call)
                {
                    if (((MethodInfo)instruction.operand).Name == "Translate")
                    {
                        list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("DrawingTools", "GetTaggedStringFromPriorityString")));
                        done = true;
                        continue;
                    }
                }
                list.Add(instruction);
            }
            return list;
        }
        public static IEnumerable<CodeInstruction> DrawWorkBoxTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            int index = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                index++;
                if (index == 116 || index == 138)
                {
                    list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method("PatchHook", "GetMaximumPriority")));
                    continue;
                }
                list.Add(instruction);
            }
            return list;
        }

        /*
         * 
         * 
         * 
         * 
         */

        private static readonly FieldInfo LOWEST_PRIORITY_FIELD = Reflection.Field("Pawn_WorkSettings", "LowestPriority", true);
        private static readonly FieldInfo DEFAULT_PRIORITY_FIELD = Reflection.Field("Pawn_WorkSettings", "DefaultPriority", true);

        private static void PatchPriorityFields(ref Object __instance)
        {
            LOWEST_PRIORITY_FIELD.SetValue(__instance, PriorityMaster.settings.GetMaxPriority());
            DEFAULT_PRIORITY_FIELD.SetValue(__instance, PriorityMaster.settings.GetDefPriority());
        }

    }


}
