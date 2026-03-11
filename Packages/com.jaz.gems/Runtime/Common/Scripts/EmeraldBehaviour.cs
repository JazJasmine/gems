
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    // Base Class for my scripts to make common functions available
    public class EmeraldBehaviour : UdonSharpBehaviour
    {
        #region Logging
        protected void LogInfo(string msg)
        {
            Debug.Log($"(INFO) [<color={LogColor}>{LogName}</color>]: {msg}\n\n<color=#242424>[EmeraldBehaviour]</color>");
        }

        protected void LogWarn(string msg)
        {
            Debug.LogWarning($"<color=#fce303>(WARN)</color> [<color={LogColor}>{LogName}</color>]: {msg}\n\n<color=#242424>[EmeraldBehaviour]</color>");
        }

        protected void LogError(string msg)
        {
            Debug.LogError($"<color=#db041a>(ERROR)</color> [<color={LogColor}>{LogName}</color>]: {msg}\n\n<color=#242424>[EmeraldBehaviour]</color>");
        }

        private const string DEFAULT_COLOR = "#05e81b";
        private const string DEFAULT_NAME = "Gems.CoreBehavior";
        protected virtual string LogColor { get => DEFAULT_COLOR; }
        protected virtual string LogName { get => DEFAULT_NAME; }
        #endregion
    }
}

