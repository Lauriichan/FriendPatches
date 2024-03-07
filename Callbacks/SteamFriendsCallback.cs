using FriendPatches.Core;
using GameNetcodeStuff;
using Steamworks;

namespace FriendPatches.Callbacks
{
    public static class SteamFriendsCallback
    {

        private static bool applied = false;

        public static void Apply()
        {
            if (FriendPatchesSettings.UsernameFix)
            {
                applied = true;
                SteamFriends.OnPersonaStateChange += PersonaChanged;
            }
        }

        public static void Unapply()
        {
            if (applied)
            {
                applied = false;
                SteamFriends.OnPersonaStateChange -= PersonaChanged;
            }
        }

        private static void PersonaChanged(Friend friend)
        {
            if (GameNetworkManager.Instance.currentLobby == null || StartOfRound.Instance == null)
            {
                return;
            }
            SteamId friendId = friend.Id;
            if (!GameNetworkManager.Instance.steamIdsInLobby.Contains(friendId))
            {
                return;
            }
            FriendPatchesPlugin.Log.LogInfo(string.Format("Updating friend ({0}): {1}", friend.Id, friend.Name));
            PlayerControllerB self = StartOfRound.Instance.allPlayerScripts[StartOfRound.Instance.thisClientPlayerId];
            ulong friendIdLong = friend.Id;
            for (int i = 0;  i < StartOfRound.Instance.allPlayerScripts.Length; i++)
            {
                PlayerControllerB controller = StartOfRound.Instance.allPlayerScripts[i];
                if (controller == null || !controller.isPlayerControlled || controller.playerSteamId != friendIdLong)
                {
                    continue;
                }
                string friendName = friend.Name;
                controller.playerUsername = friendName;
                if (controller.usernameBillboardText != null)
                {
                    controller.usernameBillboardText.text = friendName;
                }
                string friendName2 = friendName;
                int numberOfDuplicateNamesInLobby = GetNumberOfDuplicateNamesInLobby(self);
                if (numberOfDuplicateNamesInLobby > 0)
                {
                    friendName2 = string.Format("{0}{1}", friendName, numberOfDuplicateNamesInLobby);
                }
                if (self.quickMenuManager != null)
                {
                    self.quickMenuManager.AddUserToPlayerList(friendIdLong, friendName2, i);
                }
                if (StartOfRound.Instance.mapScreen == null)
                {
                    TransformAndName transform = StartOfRound.Instance.mapScreen.radarTargets[i];
                    if (transform != null)
                    {
                        transform.name = friendName2;
                    }
                }
                break;
            }
        }
        private static int GetNumberOfDuplicateNamesInLobby(PlayerControllerB self)
        {
            int num = 0;
            PlayerControllerB current;
            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
            {
                current = StartOfRound.Instance.allPlayerScripts[i];
                if (current != null && (current.isPlayerControlled || current.isPlayerDead) && !(current == self) && current.playerUsername == self.playerUsername)
                {
                    num++;
                }
            }
            for (int j = 0; j < StartOfRound.Instance.allPlayerScripts.Length; j++)
            {
                current = StartOfRound.Instance.allPlayerScripts[j];
                if (current != null && (current.isPlayerControlled || current.isPlayerDead) && !(current == self) && current.playerUsername == string.Format("{0}{1}", current.playerUsername, num))
                {
                    num++;
                }
            }
            return num;
        }

    }
}
