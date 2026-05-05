// Jaz's Gems — TaskManager
// Purpose: Placeholder for future task assignment and distribution logic
// Used by: Roleplay system (not yet implemented)

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Roleplay
    {
        public class TaskManager : EmeraldBehaviour
        {
            protected override string LogName => "Gems.Roleplay.TaskManager";
            protected override string LogColor => "#c205e8";

            [SerializeField]
            RoleplayData data;
        }
    }
}
