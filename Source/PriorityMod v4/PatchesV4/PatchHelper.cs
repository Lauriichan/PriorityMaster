using HarmonyLib;
using System;
using System.Linq.Expressions;

namespace PriorityMod.PatchesV4
{
    public static class PatchHelper
    {
        public static HarmonyMethod Method(Expression<Action> action)
        {
            return new HarmonyMethod(SymbolExtensions.GetMethodInfo(action))
            {
                priority = 10000
            };
        }
        public static HarmonyMethod Method<A>(Expression<Action<A>> action)
        {
            return new HarmonyMethod(SymbolExtensions.GetMethodInfo(action))
            {
                priority = 10000
            };
        }
        public static HarmonyMethod Method<A, B>(Expression<Action<A, B>> action)
        {
            return new HarmonyMethod(SymbolExtensions.GetMethodInfo(action))
            {
                priority = 10000
            };
        }
    }
}
