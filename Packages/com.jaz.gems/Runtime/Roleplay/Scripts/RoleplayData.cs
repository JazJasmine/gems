// Jaz's Gems — RoleplayData
// Purpose: Loads and provides structured access to roleplay scenario JSON data
// Used by: Roleplay system (RoleManager, Roleplayer, Administration)

using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace Gems
{
    namespace Roleplay
    {
        public class RoleplayData : EmeraldBehaviour
        {
            protected override string LogName => "Gems.Roleplay.RoleplayData";
            protected override string LogColor => "#05e81b";

            [SerializeField] VRCUrl scenarioUrl;

            DataDictionary roleplayData;

            public bool IsLoaded => roleplayData != null;

            void Start()
            {
                LogInfo($"[Start]: Loading scenario data from URL...");
                VRCStringDownloader.LoadUrl(scenarioUrl, (IUdonEventReceiver)this);
            }

            public override void OnStringLoadSuccess(IVRCStringDownload result)
            {
                if (VRCJson.TryDeserializeFromJson(result.Result, out DataToken json))
                {
                    roleplayData = json.DataDictionary;
                    LogInfo($"[OnStringLoadSuccess]: Data loaded.");
                }
                else
                {
                    LogError($"[OnStringLoadSuccess]: Failed to deserialize JSON. {json}");
                }
            }

            public override void OnStringLoadError(IVRCStringDownload result)
            {
                LogError($"[OnStringLoadError]: Failed to load data: {result.Error}");
            }

            public DataDictionary RoleById(string id)
            {
                if (roleplayData == null) return null;
                return roleplayData["roles"].DataDictionary[id].DataDictionary;
            }

            public DataDictionary Roles
            {
                get
                {
                    if (roleplayData == null) return null;
                    return roleplayData["roles"].DataDictionary;
                }
            }

            public DataList RoleIds
            {
                get
                {
                    if (roleplayData == null) return null;
                    return roleplayData["roles"].DataDictionary.GetKeys();
                }
            }

            public DataDictionary TaskSlots
            {
                get
                {
                    if (roleplayData == null) return null;
                    return roleplayData["taskSlotsByRole"].DataDictionary;
                }
            }

            public DataDictionary TaskPool
            {
                get
                {
                    if (roleplayData == null) return null;
                    return roleplayData["taskPoolsByRole"].DataDictionary;
                }
            }

            public DataList UrgentTasks
            {
                get
                {
                    if (roleplayData == null) return null;
                    return roleplayData["urgentTasks"].DataList;
                }
            }

            public DataDictionary FallbackTask
            {
                get
                {
                    if (roleplayData == null) return null;
                    return roleplayData["fallbackTask"].DataDictionary;
                }
            }
        }
    }
}