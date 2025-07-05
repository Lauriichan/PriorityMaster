using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PriorityMod.Tools
{
    public class OpCodeMatcher
    {

        private readonly List<OpCode> opcodes = new List<OpCode>();

        public OpCodeMatcher(OpCode[] codes) { 
            opcodes.AddRange(codes);
        }

        public int Count()
        {
            return opcodes.Count;
        }

        public int Match(List<CodeInstruction> instructions, int startIndex) {
            if (startIndex <= -1)
            {
                startIndex = 0;
            }
            bool success;
            for (int i = startIndex; i + opcodes.Count < instructions.Count; i++)
            {
                int tmpIdx = 0;
                success = true;
                foreach (OpCode opcode in opcodes)
                {
                    if (instructions[i + (tmpIdx++)].opcode != opcode)
                    {
                        success = false;
                        break;
                    }
                }
                if (success)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
