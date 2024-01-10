﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FriendPatches.Tools
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
