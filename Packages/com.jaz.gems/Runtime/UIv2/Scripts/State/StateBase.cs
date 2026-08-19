
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace UIv2
    {
        public class StateBase : EmeraldBehaviour
        {
            protected void TakeOwnershipIfNeeded()
            {
                if (!Networking.IsOwner(gameObject))
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
        }
    }
}