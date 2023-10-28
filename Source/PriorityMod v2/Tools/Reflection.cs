using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace PriorityMod.Tools
{
    public static class Reflection
    {

        public static Type Type(String type)
        {
            return AccessTools.TypeByName(type);
        }

        public static ConstructorInfo Constructor(String type)
        {
            return AccessTools.GetDeclaredConstructors(Type(type)).Take(1).Last();
        }

        public static MethodBase Method(String type, String method, Type[] args = null, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredMethod(Type(type), method, args);
            return AccessTools.Method(Type(type), method, args);
        }

        public static FieldInfo Field(String type, String field, bool declared = true)
        {
            if(declared)
                return AccessTools.DeclaredField(Type(type), field);
            return AccessTools.Field(Type(type), field);
        }

        public static PropertyInfo Property(String type, String property, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredProperty(Type(type), property);
            return AccessTools.Property(Type(type), property);
        }

    }
}
