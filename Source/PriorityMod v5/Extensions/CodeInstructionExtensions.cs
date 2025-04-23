using HarmonyLib;

namespace PriorityMod.Extensions
{
    public static class CodeInstructionExtensions
    {
        public static CodeInstruction WithLabelsFrom(this CodeInstruction inst, CodeInstruction from)
        {
            foreach (var label in from.labels)
                inst.labels.Add(label);
            return inst;
        }
    }
}