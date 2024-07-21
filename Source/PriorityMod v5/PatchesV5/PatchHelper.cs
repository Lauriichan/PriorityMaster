using HarmonyLib;
using System;
using System.Linq.Expressions;

namespace PriorityMod.PatchesV5
{
    public static class PatchHelper
    {
        public static HarmonyMethod Method(Expression<Action> action, bool debug = false)
        {
            return new HarmonyMethod(SymbolExtensions.GetMethodInfo(action), debug: debug)
            {
                priority = 10000
            };
        }
        public static HarmonyMethod Method<A>(Expression<Action<A>> action, bool debug = false)
        {
            return new HarmonyMethod(SymbolExtensions.GetMethodInfo(action), debug: debug)
            {
                priority = 10000
            };
        }
        public static HarmonyMethod Method<A, B>(Expression<Action<A, B>> action, bool debug = false)
        {
            return new HarmonyMethod(SymbolExtensions.GetMethodInfo(action), debug: debug)
            {
                priority = 10000
            };
        }
    }
}
