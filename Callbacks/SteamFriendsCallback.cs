using FriendPatches.Core;
using GameNetcodeStuff;
using Steamworks;

namespace FriendPatches.Callbacks
{
    public static class SteamFriendsCallback
    {

        public static void Apply()
        {
            SteamFriends.OnPersonaStateChange += PersonaChanged;
        }

        public static void Unapply()
        {
            SteamFriends.OnPersonaStateChange -= PersonaChanged;
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
                controller.usernameBillboardText.text = friendName;
                string friendName2 = friendName;
                int numberOfDuplicateNamesInLobby = GetNumberOfDuplicateNamesInLobby(self);
                if (numberOfDuplicateNamesInLobby > 0)
                {
                    friendName2 = string.Format("{0}{1}", friendName, numberOfDuplicateNamesInLobby);
                }
                self.quickMenuManager.AddUserToPlayerList(friendIdLong, friendName2, i);
                StartOfRound.Instance.mapScreen.radarTargets[i].name = friendName2;
                break;
            }
        }

        private static int GetNumberOfDuplicateNamesInLobby(PlayerControllerB self)
        {
            int num = 0;
            for (int i = 0; i < StartOfRound.Instance.allPlayerScripts.Length; i++)
            {
                if ((StartOfRound.Instance.allPlayerScripts[i].isPlayerControlled || StartOfRound.Instance.allPlayerScripts[i].isPlayerDead) && !(StartOfRound.Instance.allPlayerScripts[i] == self) && StartOfRound.Instance.allPlayerScripts[i].playerUsername == self.playerUsername)
                {
                    num++;
                }
            }
            for (int j = 0; j < StartOfRound.Instance.allPlayerScripts.Length; j++)
            {
                if ((StartOfRound.Instance.allPlayerScripts[j].isPlayerControlled || StartOfRound.Instance.allPlayerScripts[j].isPlayerDead) && !(StartOfRound.Instance.allPlayerScripts[j] == self) && StartOfRound.Instance.allPlayerScripts[j].playerUsername == string.Format("{0}{1}", StartOfRound.Instance.allPlayerScripts[j].playerUsername, num))
                {
                    num++;
                }
            }
            return num;
        }

    }
}
