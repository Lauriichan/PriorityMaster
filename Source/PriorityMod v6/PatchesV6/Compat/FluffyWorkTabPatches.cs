using HarmonyLib;
using PriorityMod.Extensions;
using PriorityMod.Tools;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PriorityMod.PatchesV6.Compat
{
    public static class FluffyWorkTabPatches
    {

        private static FieldInfo workgiver;
        private static PropertyInfo priorities;

        public static void Apply(Harmony harmony)
        {
            if (!ModsConfig.IsAnyActiveOrEmpty(new string[] { "Fluffy.WorkTab", "arof.fluffy.worktab", "arof.fluffy.worktab.continued" }))
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

            workgiver = Reflection.Field("WorkTab.WorkPriority", "workgiver");
            priorities = Reflection.Property("WorkTab.WorkPriority", "Priorities");
            harmony.Patch(Reflection.Method("WorkTab.WorkPriority", "ExposeData"), transpiler: PatchHelper.Method(() => ExposeDataTranspiler(null, null)));
        }

        public static string PatchedPriorityLabel(int priority)
        {
            return DrawingTools.GetTaggedStringFromPriorityString("Priority" +  priority).Colorize(DrawingTools.GetColorFromPriority(priority));
        }

        public static void DoInfoLabel(Listing_Standard listing)
        {
            listing.Label("For priority settings, please check out the configuration of PriorityMaster.");
        }

        public static void PatchedExposeData(object self)
        {
            WorkGiverDef workGiverDef = (WorkGiverDef) workgiver.GetValue(self);
            try
            {
                Scribe_Defs.Look<WorkGiverDef>(ref workGiverDef, "Workgiver");
            }
            catch (Exception e)
            {
                Log.Warning("WorkTab :: failed to load priorities. Did you disable a mod? If so, this message can safely be ignored." + e.Message + "\n\n" + e.StackTrace);
            }
            workgiver.SetValue(self, workGiverDef);
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                int[] _priority_array = priorities.GetValue(self) as int[];
                string _priorities = string.Join("_", _priority_array.Select((int i) => i.ToString()).ToArray<string>());
                Scribe_Values.Look<string>(ref _priorities, "PM_Priorities", null, false);
            }
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                string _pm_priorities = "";
                Scribe_Values.Look<string>(ref _pm_priorities, "PM_Priorities", null, false);
                int[] _priority_array;
                if (_pm_priorities == null || _pm_priorities.Length == 0)
                {
                    _pm_priorities = "";
                    Scribe_Values.Look<string>(ref _pm_priorities, "Priorities", null, false);
                    _priority_array = (from prio in _pm_priorities.ToCharArray() select int.Parse(prio.ToString())).ToArray<int>();
                } else
                {
                    _priority_array = (from prio in _pm_priorities.Split('_') select int.Parse(prio)).ToArray<int>();
                }
                priorities.SetValue(self, _priority_array);
            }
        }

        private static IEnumerable<CodeInstruction> ExposeDataTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            list.Add(new CodeInstruction(OpCodes.Ldarg_0));
            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method(typeof(FluffyWorkTabPatches), "PatchedExposeData", new Type[] { typeof(object) })));
            list.Add(new CodeInstruction(OpCodes.Ret));
            return list;
        }

        private static IEnumerable<CodeInstruction> OptionsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {

            OpCodeMatcher matcher = new OpCodeMatcher(new OpCode[]
            {
                OpCodes.Dup,
                OpCodes.Ldstr,
                OpCodes.Call,
                OpCodes.Call,
                OpCodes.Ldsflda,
                OpCodes.Ldsflda,
                OpCodes.Ldc_R4,
            });

            List<CodeInstruction> output = new List<CodeInstruction>();
            List<CodeInstruction> list = new List<CodeInstruction>(instructions);

            bool infoLabelAdded = false;

            int idx = -1, lastMatchEnd = -1;
            while ((idx = matcher.Match(list, idx + 1)) != -1)
            {
                if (lastMatchEnd != -1)
                {
                    output.AppendList(list, lastMatchEnd, idx);
                } else
                {
                    output.AppendList(list, 0, idx);
                }
                switch (list[idx + 1].operand)
                {
                    case "WorkTab.MaxPriority":
                    case "WorkTab.DefaultPriority":
                        if (!infoLabelAdded)
                        {
                            infoLabelAdded = true;
                            output.Add(new CodeInstruction(OpCodes.Dup));
                            output.Add(new CodeInstruction(OpCodes.Callvirt, Reflection.Method(typeof(FluffyWorkTabPatches), "DoInfoLabel", new Type[] { typeof(Listing_Standard) })));
                        }
                        lastMatchEnd = idx + matcher.Count() + 7;
                        continue;
                    default:
                        output.AppendList(list, idx, idx + matcher.Count());
                        break;
                }
                lastMatchEnd = idx + matcher.Count();
            }
            output.AppendList(list, lastMatchEnd);
            return output;
        }

        private static IEnumerable<CodeInstruction> LabelTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> list = new List<CodeInstruction>();
            list.Add(new CodeInstruction(OpCodes.Ldarg_0));
            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method(typeof(FluffyWorkTabPatches), "PatchedPriorityLabel", new Type[] { typeof(int) })));
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
                            list.Add(new CodeInstruction(OpCodes.Call, Reflection.Method(typeof(PatchHook), "GetDefaultPriority")).MoveLabelsFrom(instruction));
                            continue;
                        }
                        else if (field.Name.Equals("maxPriority"))
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
