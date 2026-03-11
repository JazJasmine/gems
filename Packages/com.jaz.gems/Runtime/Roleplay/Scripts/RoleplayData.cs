
using Gems;
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
            [SerializeField] VRCUrl scenarioUrl;
            DataDictionary roleplayData;

            void Start()
            {
                VRCStringDownloader.LoadUrl(scenarioUrl, (IUdonEventReceiver)this);
            }

            public override void OnStringLoadSuccess(IVRCStringDownload result)
            {
                if (VRCJson.TryDeserializeFromJson(result.Result, out DataToken json))
                {
                    LogInfo($"Successfully deserialized as a dictionary with {json.DataDictionary.Count} items.");
                    roleplayData = json.DataDictionary;
                }
            }

            public DataDictionary Roles
            {
                get => roleplayData["roles"].DataDictionary;
            }

            public DataList RoleIds
            {
                get => roleplayData["roles"].DataDictionary.GetKeys();
            }

            public DataDictionary TaskSlots
            {
                get => roleplayData["taskSlotsByRole"].DataDictionary;
            }

            public DataDictionary TaskPool
            {
                get => roleplayData["taskPoolsByRole"].DataDictionary;
            }

            public DataList UrgentTasks
            {
                get => roleplayData["fallbackTask"].DataList;
            }

            public DataDictionary FallbackTask
            {
                get => roleplayData["fallbackTask"].DataDictionary;
            }
        }
    }
}