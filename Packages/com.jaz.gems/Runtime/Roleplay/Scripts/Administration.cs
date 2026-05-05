// Jaz's Gems — Administration
// Purpose: Admin UI for assigning roles to players via dropdowns
// Used by: Roleplay system

using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Roleplay
    {
        public class Administration : EmeraldBehaviour
        {
            protected override string LogName => "Gems.Roleplay.Administration";
            protected override string LogColor => "#c205e8";

            [SerializeField] RoleManager roleManager;
            [SerializeField] RoleplayData data;
            [SerializeField] TMP_Dropdown playerDropdown;
            [SerializeField] TMP_Dropdown roleDropdown;

            string selectedPlayerName;
            string selectedRoleId;
            string[] playerNames = new string[0];
            string[] roleIds = new string[0];

            bool rolesLoaded;

            void Start()
            {
                LogInfo("[Start]: Administration initialized, waiting for role data...");
                SendCustomEventDelayedSeconds(nameof(_TryLoadRoles), 2f);
            }

            public void _TryLoadRoles()
            {
                if (rolesLoaded) return;

                DataList keys = data.RoleIds;
                if (keys == null || keys.Count == 0)
                {
                    LogInfo("[TryLoadRoles]: Role data not available yet, retrying...");
                    SendCustomEventDelayedSeconds(nameof(_TryLoadRoles), 2f);
                    return;
                }

                rolesLoaded = true;
                RefreshRoleDropdown();
                LogInfo("[TryLoadRoles]: Role dropdown populated");
            }

            public override void OnPlayerJoined(VRCPlayerApi player)
            {
                if (!Networking.IsInstanceOwner) return;

                LogInfo($"[OnPlayerJoined]: {player.displayName}");
                RefreshPlayerDropdown();
            }

            public override void OnPlayerLeft(VRCPlayerApi player)
            {
                if (!Networking.IsInstanceOwner) return;

                LogInfo($"[OnPlayerLeft]: {player.displayName}");
                RefreshPlayerDropdown();
            }

            [NetworkCallable]
            public void OnPlayerSelected(int index, string label)
            {
                if (!Networking.IsInstanceOwner) return;

                if (index < 0 || index >= playerNames.Length)
                {
                    LogWarn($"[OnPlayerSelected]: Index {index} out of range");
                    return;
                }

                selectedPlayerName = playerNames[index];
                LogInfo($"[OnPlayerSelected]: Selected player '{selectedPlayerName}'");
            }

            [NetworkCallable]
            public void OnRoleSelected(int index, string label)
            {
                if (!Networking.IsInstanceOwner) return;

                if (index < 0 || index >= roleIds.Length)
                {
                    LogWarn($"[OnRoleSelected]: Index {index} out of range");
                    return;
                }

                selectedRoleId = roleIds[index];
                LogInfo($"[OnRoleSelected]: Selected role '{selectedRoleId}'");
            }

            [NetworkCallable]
            public void OnAssignClicked()
            {
                if (!Networking.IsInstanceOwner) return;

                if (string.IsNullOrEmpty(selectedPlayerName))
                {
                    LogWarn("[OnAssignClicked]: No player selected");
                    return;
                }

                if (string.IsNullOrEmpty(selectedRoleId))
                {
                    LogWarn("[OnAssignClicked]: No role selected");
                    return;
                }

                LogInfo($"[OnAssignClicked]: Assigning role '{selectedRoleId}' to '{selectedPlayerName}'");
                roleManager._AssignRole(selectedPlayerName, selectedRoleId);
            }

            void RefreshPlayerDropdown()
            {
                DataList keys = roleManager.PlayerNames;
                playerNames = new string[keys.Count];

                playerDropdown.ClearOptions();

                for (int i = 0; i < keys.Count; i++)
                {
                    playerNames[i] = keys[i].String;
                }
                playerDropdown.AddOptions(playerNames);
                playerDropdown.RefreshShownValue();
                selectedPlayerName = playerNames.Length > 0 ? playerNames[0] : null;

                LogInfo($"[RefreshPlayerDropdown]: {playerNames.Length} players listed");
            }

            void RefreshRoleDropdown()
            {
                DataList keys = data.RoleIds;
                DataDictionary roles = data.Roles;
                roleIds = new string[keys.Count];

                roleDropdown.ClearOptions();

                string[] roleNames = new string[keys.Count];
                for (int i = 0; i < keys.Count; i++)
                {
                    roleIds[i] = keys[i].String;
                    roleNames[i] = roles[roleIds[i]].DataDictionary["roleName"].String;
                }

                roleDropdown.AddOptions(roleNames);
                roleDropdown.RefreshShownValue();
                selectedRoleId = roleIds.Length > 0 ? roleIds[0] : null;

                LogInfo($"[RefreshRoleDropdown]: {roleIds.Length} roles listed");
            }
        }
    }
}
