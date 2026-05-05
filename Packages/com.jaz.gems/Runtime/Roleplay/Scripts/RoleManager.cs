// Jaz's Gems — RoleManager
// Purpose: Manages player-to-role assignments; instance owner authoritative
// Used by: Roleplay system (Administration UI)

using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Roleplay
    {
        public class RoleManager : EmeraldBehaviour
        {
            protected override string LogName => "Gems.Roleplay.RoleManager";
            protected override string LogColor => "#e8a205";

            [SerializeField] Roleplayer referenceRoleplayer;
            [SerializeField] RoleplayData data;

            DataDictionary roleplayerByPlayer = new DataDictionary();
            DataDictionary roleByPlayer = new DataDictionary();

            public DataList PlayerNames => roleByPlayer.GetKeys();

            public string GetPlayerRole(string playerName)
            {
                if (!roleByPlayer.ContainsKey(playerName)) return null;
                return roleByPlayer[playerName].String;
            }

            public override void OnPlayerJoined(VRCPlayerApi player)
            {
                if (!Networking.IsInstanceOwner) return;

                LogInfo($"[OnPlayerJoined]: Registering {player.displayName}");
                roleplayerByPlayer.Add(player.displayName, GetByPlayer(player));
                roleByPlayer.Add(player.displayName, "unassigned");
            }

            public override void OnPlayerLeft(VRCPlayerApi player)
            {
                if (!Networking.IsInstanceOwner) return;

                LogInfo($"[OnPlayerLeft]: Removing data for {player.displayName}");
                roleplayerByPlayer.Remove(player.displayName);
                roleByPlayer.Remove(player.displayName);
            }

            public void _AssignRole(string playerName, string roleId)
            {
                if (!Networking.IsInstanceOwner) return;

                LogInfo($"[AssignRole]: player={playerName}, roleId={roleId}");

                if (!data.Roles.ContainsKey(roleId))
                {
                    LogWarn($"[AssignRole]: Role '{roleId}' not found");
                    return;
                }

                if (!roleplayerByPlayer.ContainsKey(playerName) || !roleByPlayer.ContainsKey(playerName))
                {
                    LogWarn($"[AssignRole]: Player '{playerName}' not in dictionaries, roleplayerByPlayer={roleplayerByPlayer.ContainsKey(playerName)}, roleByPlayer={roleByPlayer.ContainsKey(playerName)}");
                    return;
                }

                roleByPlayer[playerName] = roleId;

                GetByPlayer(playerName).SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "AssignRole",
                    roleId);

                LogInfo($"[AssignRole]: Dispatched role '{roleId}' to {playerName} via network call");
            }

            Roleplayer GetByPlayer(VRCPlayerApi player)
            {
                if (roleplayerByPlayer.ContainsKey(player.displayName))
                {
                    LogInfo($"[GetByPlayer]: Cache hit for {player.displayName}");
                    return ((Roleplayer)roleplayerByPlayer[player.displayName].Reference);
                }

                LogInfo($"[GetByPlayer]: Cache miss for {player.displayName}. Searching Player Objects");
                return (Roleplayer)Networking.FindComponentInPlayerObjects(player, referenceRoleplayer);
            }

            Roleplayer GetByPlayer(string playerName)
            {
                if (!roleplayerByPlayer.ContainsKey(playerName))
                {
                    LogWarn($"[GetByPlayer]: Player '{playerName}' not found in cache");
                    return null;
                }
                return ((Roleplayer)roleplayerByPlayer[playerName].Reference);
            }

        }
    }
}
