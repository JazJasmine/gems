// Jaz's Gems — Roleplayer
// Purpose: Per-player synced role state holder; stores assigned role and hydrated role data
// Used by: Roleplay system (RoleManager assigns roles to this)

using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Roleplay
    {
        [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
        public class Roleplayer : EmeraldBehaviour
        {
            protected override string LogName => "Gems.Roleplay.Roleplayer";
            protected override string LogColor => "#05b8e8";

            VRCPlayerApi owner;

            [SerializeField] RoleplayData data;

            // Roleplay Data
            [UdonSynced, FieldChangeCallback(nameof(RoleId))] string roleId;
            string roleName;
            string description;
            string vibe;
            string behavior;
            string rules;

            // Internals
            DataList activeTasks = new DataList();
            DataList recentlyDone = new DataList();


            void Start()
            {
                owner = Networking.GetOwner(gameObject);
            }

            [NetworkCallable]
            public void AssignRole(string id)
            {
                LogInfo($"[AssignRole|{owner.displayName}]: {name} (id={id})");

                if (!Networking.IsOwner(gameObject))
                {
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
                }

                RoleId = id;

                RequestSerialization();
            }

            public string RoleId
            {
                get => roleId;
                set
                {
                    roleId = value;

                    DataDictionary role = data.RoleById(roleId);
                    if (role == null) return;

                    roleName = role["roleName"].String;
                    description = role["description"].String;
                    vibe = role["vibe"].String;
                    behavior = role["behavior"].String;
                    rules = role["rules"].String;

                }
            }
        }
    }
}
