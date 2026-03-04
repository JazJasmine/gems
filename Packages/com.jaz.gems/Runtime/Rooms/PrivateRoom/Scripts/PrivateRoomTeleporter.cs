
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Rooms
    {

        public class PrivateRoomTeleporter : UdonSharpBehaviour
        {
            [SerializeField] PrivateRoom room;
            [SerializeField] bool IsExit;

            public override void Interact()
            {
                if (IsExit)
                {
                    room._Exit();
                }
                else
                {
                    room._Enter();
                }
            }
        }
    }
}