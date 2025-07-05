using HarmonyLib;
using System;
using System.Collections.Generic;
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

        public static List<ConstructorInfo> Constructors(String type)
        {
            return AccessTools.GetDeclaredConstructors(Type(type));
        }

        public static List<ConstructorInfo> Constructors(Type type)
        {
            return AccessTools.GetDeclaredConstructors(type);
        }

        public static MethodBase Method(String type, String method, Type[] args = null, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredMethod(Type(type), method, args);
            return AccessTools.Method(Type(type), method, args);
        }

        public static MethodBase Method(Type type, String method, Type[] args = null, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredMethod(type, method, args);
            return AccessTools.Method(type, method, args);
        }

        public static FieldInfo Field(String type, String field, bool declared = true)
        {
            if(declared)
                return AccessTools.DeclaredField(Type(type), field);
            return AccessTools.Field(Type(type), field);
        }

        public static FieldInfo Field(Type type, String field, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredField(type, field);
            return AccessTools.Field(type, field);
        }
        public static PropertyInfo Property(String type, String property, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredProperty(Type(type), property);
            return AccessTools.Property(Type(type), property);
        }
        public static PropertyInfo Property(Type type, String property, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredProperty(type, property);
            return AccessTools.Property(type, property);
        }

    }
}
