using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FriendPatches.Tools
{
    public static class Reflection
    {
        public static Type Type(string type)
        {
            return AccessTools.TypeByName(type);
        }

        public static List<ConstructorInfo> Constructors(string type)
        {
            return AccessTools.GetDeclaredConstructors(Type(type));
        }

        public static List<ConstructorInfo> Constructors(Type type)
        {
            return AccessTools.GetDeclaredConstructors(type);
        }

        public static MethodBase Method(string type, string method, Type[] args = null, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredMethod(Type(type), method, args);
            return AccessTools.Method(Type(type), method, args);
        }

        public static MethodBase Method(Type type, string method, Type[] args = null, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredMethod(type, method, args);
            return AccessTools.Method(type, method, args);
        }

        public static FieldInfo Field(string type, string field, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredField(Type(type), field);
            return AccessTools.Field(Type(type), field);
        }

        public static FieldInfo Field(Type type, string field, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredField(type, field);
            return AccessTools.Field(type, field);
        }
        public static PropertyInfo Property(string type, string property, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredProperty(Type(type), property);
            return AccessTools.Property(Type(type), property);
        }
        public static PropertyInfo Property(Type type, string property, bool declared = true)
        {
            if (declared)
                return AccessTools.DeclaredProperty(type, property);
            return AccessTools.Property(type, property);
        }

    }
}
