using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;

namespace Gems
{
    namespace Rooms
    {

        [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
        public class PrivateRoom : EmeraldBehaviour
        {
            [SerializeField] int roomNumber;

            [UdonSynced, FieldChangeCallback(nameof(IsLocked))] bool isLocked;
            [UdonSynced, FieldChangeCallback(nameof(IsOccupied))] bool isOccupied;

            [UdonSynced, FieldChangeCallback(nameof(_PlayerListJson))] string _playerListJson;
            DataList playerList = new DataList();
            VRCPlayerApi localPlayer;

            [SerializeField] Transform enterSpawn;
            [SerializeField] Transform exitSpawn;
            [SerializeField] UI.BoxToggle lockToggle;

            [SerializeField] MeshRenderer doorNumber;
            [SerializeField] Material openMaterial;
            [SerializeField] Material occupiedMaterial;
            [SerializeField] Material lockedMaterial;

            private void Start()
            {
                localPlayer = Networking.LocalPlayer;
                IsLocked = false;
                IsOccupied = false;
            }

            public override void OnPlayerTriggerEnter(VRCPlayerApi player)
            {
                if (!Networking.IsOwner(localPlayer, gameObject)) return;

                playerList.Add(player.playerId);
                if (VRCJson.TrySerializeToJson(playerList, JsonExportType.Minify, out DataToken result))
                {
                    _PlayerListJson = result.String;
                }

                IsOccupied = true;
                RequestSerialization();
            }

            public override void OnPlayerTriggerExit(VRCPlayerApi player)
            {
                if (!Networking.IsOwner(localPlayer, gameObject)) return;
                if (!Contains(playerList, player.playerId)) return;
                Remove(playerList, player.playerId);

                if (VRCJson.TrySerializeToJson(playerList, JsonExportType.Minify, out DataToken result))
                {
                    _PlayerListJson = result.String;
                }

                // Room is empty now
                if (playerList.Count <= 0)
                {
                    IsOccupied = false;
                    IsLocked = false;
                }
                RequestSerialization();
            }

            public override void OnPlayerLeft(VRCPlayerApi player)
            {
                if (!Networking.IsOwner(localPlayer, gameObject)) return;

                if (!Contains(playerList, player.playerId)) return;
                Remove(playerList, player.playerId);

                if (VRCJson.TrySerializeToJson(playerList, JsonExportType.Minify, out DataToken result))
                {
                    _PlayerListJson = result.String;
                }
                // Room is empty now
                if (playerList.Count <= 0)
                {
                    IsOccupied = false;
                    IsLocked = false;
                }
                RequestSerialization();
            }

            public void _Enter()
            {
                if (IsLocked) return;

                localPlayer.TeleportTo(enterSpawn.position, enterSpawn.rotation);
            }

            public void _Exit()
            {
                localPlayer.TeleportTo(exitSpawn.position, exitSpawn.rotation);
            }

            [NetworkCallable]
            public void _LockRoomRequest(bool state)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "LockRoom", state);
            }

            [NetworkCallable]
            public void _PrivacyShutterRequest(bool state)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ToggleShutter", state);
            }

            void UpdateLockIndicator()
            {
                if (IsLocked)
                {
                    doorNumber.material = lockedMaterial;
                    return;
                }

                if (IsOccupied)
                {
                    doorNumber.material = occupiedMaterial;
                    return;
                }

                doorNumber.material = openMaterial;
            }

            [NetworkCallable]
            public void LockRoom(bool state)
            {
                IsLocked = state;
                RequestSerialization();
            }

            bool Contains(DataList playerIds, int searchId)
            {
                foreach (var id in playerIds.ToArray())
                {
                    var number = id.Number;

                    if (searchId == number)
                    {
                        return true;
                    }
                }

                return false;
            }

            void Remove(DataList playerIds, int deleteId)
            {
                for (int i = 0; i < playerIds.Count; i++)
                {
                    if (playerIds[i].Number == deleteId)
                    {
                        playerIds.RemoveAt(i);
                    }
                }
            }

            public bool IsLocked
            {
                get => isLocked;
                set
                {
                    isLocked = value;
                    lockToggle.State = value;
                    UpdateLockIndicator();
                }
            }

            public bool IsOccupied
            {
                get => isOccupied;
                set
                {
                    isOccupied = value;
                    UpdateLockIndicator();
                }
            }

            public string _PlayerListJson
            {
                get => _playerListJson;
                set
                {
                    _playerListJson = value;

                    if (VRCJson.TryDeserializeFromJson(_playerListJson, out DataToken result))
                    {
                        playerList = result.DataList;
                    }
                }
            }
        }
    }
}