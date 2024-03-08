using FriendPatches.Core;
using GameNetcodeStuff;
using Steamworks;
using System.Threading.Tasks;

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
            Task.Run(() => DoNameUpdate(friendId, friend.Name));
        }

        private static void DoNameUpdate(SteamId friendId, string friendName)
        {
            StartOfRound round = StartOfRound.Instance;
            FriendPatchesPlugin.Log.LogInfo(string.Format("Updating friend ({0}): {1}", friendId, friendName));
            PlayerControllerB self = round.allPlayerScripts[round.thisClientPlayerId];
            ulong friendIdLong = friendId;
            for (int i = 0; i < round.allPlayerScripts.Length; i++)
            {
                PlayerControllerB controller = round.allPlayerScripts[i];
                if (controller == null || !controller.isPlayerControlled || controller.playerSteamId != friendIdLong)
                {
                    continue;
                }
                controller.playerUsername = friendName;
                if (controller.usernameBillboardText != null)
                {
                    controller.usernameBillboardText.text = friendName;
                }
                string friendName2 = friendName;
                int numberOfDuplicateNamesInLobby = round.GetNumberOfDuplicateNamesInLobby(self);
                if (numberOfDuplicateNamesInLobby > 0)
                {
                    friendName2 = string.Format("{0}{1}", friendName, numberOfDuplicateNamesInLobby);
                }
                if (self.quickMenuManager != null)
                {
                    self.quickMenuManager.AddUserToPlayerList(friendIdLong, friendName2, i);
                }
                if (round.mapScreen == null)
                {
                    TransformAndName transform = round.mapScreen.radarTargets[i];
                    if (transform != null)
                    {
                        transform.name = friendName2;
                    }
                }
                break;
            }
        }

        private static int GetNumberOfDuplicateNamesInLobby(this StartOfRound round, PlayerControllerB self)
        {
            int num = 0;
            PlayerControllerB current;
            for (int i = 0; i < round.allPlayerScripts.Length; i++)
            {
                current = round.allPlayerScripts[i];
                if (current != null && (current.isPlayerControlled || current.isPlayerDead) && !(current == self) && current.playerUsername == self.playerUsername)
                {
                    num++;
                }
            }
            for (int j = 0; j < round.allPlayerScripts.Length; j++)
            {
                current = round.allPlayerScripts[j];
                if (current != null && (current.isPlayerControlled || current.isPlayerDead) && !(current == self) && current.playerUsername == string.Format("{0}{1}", current.playerUsername, num))
                {
                    num++;
                }
            }
            return num;
        }

    }
}
