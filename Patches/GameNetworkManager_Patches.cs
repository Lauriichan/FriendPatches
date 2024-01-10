using FriendPatches.Core;
using FriendPatches.Tools;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FriendPatches.Patches
{
    public static class GameNetworkManager_Patches
    {
        public static void Apply(Harmony harmony)
        {
            if (FriendPatchesSettings.SetPlayedWithOnSteam)
            {
                harmony.Patch(Reflection.Method(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyMemberJoined"), transpiler: PatchHelper.Method(() => PlayerJoined(null, null)));
            }
        }

        public static void HandleFriendJoin(List<SteamId> steamIdsInLobby, SteamId id)
        {
            if (steamIdsInLobby == null || steamIdsInLobby.Contains(id))
            {
                return;
            }
            steamIdsInLobby.Add(id);
            SteamFriends.SetPlayedWith(id);
        }

        private static IEnumerable<CodeInstruction> PlayerJoined(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            CodeMatcher matcher = new CodeMatcher(instructions, generator);
            // Add HandleFriendJoin patch
            matcher.MatchForward(true,
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "steamIdsInLobby"),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldloc_3),
                new CodeMatch(OpCodes.Ldelema),
                new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "Id" && ((FieldInfo)i.operand).DeclaringType == typeof(Friend)),
                new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "Add" && ((MethodInfo)i.operand).DeclaringType.Name.StartsWith("List")));
            if (!matcher.ReportFailure(MethodBase.GetCurrentMethod(), FriendPatchesPlugin.Log.LogFatal))
            {
                matcher.SetInstruction(new CodeInstruction(OpCodes.Call, Reflection.Method(typeof(GameNetworkManager_Patches), "HandleFriendJoin")));
            }
            return matcher.End().InstructionEnumeration();
        }

    }
}
