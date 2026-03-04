
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

public class Announcements : UdonSharpBehaviour
{
    [SerializeField] ToastTracking toast;
    VRCPlayerApi localPlayer;

    void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    public void Local(string message, float duration = 5f)
    {
        Announce(message, duration);
    }

    public void Broadcast(string message, float duration = 5f)
    {
        if (localPlayer.displayName != "JazJasmine" && !localPlayer.isInstanceOwner) return;

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Announce", message, duration);
    }

    public void Broadcast(int[] playerIds, string message, float duration = 5f)
    {
        if (localPlayer.displayName != "JazJasmine" && !localPlayer.isInstanceOwner) return;

        foreach (int playerId in playerIds) {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayerIdAnnounce", playerId, message, duration);
        }
    }

    public void Broadcast(int playerId, string message, float duration = 5f)
    {
        if (localPlayer.displayName != "JazJasmine" && !localPlayer.isInstanceOwner) return;

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayerIdAnnounce", playerId, message, duration);
    }

    [NetworkCallable]
    public void Announce(string message, float duration)
    {
        toast.Show(message, duration);
    }

    [NetworkCallable]
    public void AnnounceUI(string message)
    {
        Broadcast(message);
    }

    [NetworkCallable]
    public void PlayerIdAnnounce(int playerId, string message, float duration)
    {
        if (localPlayer.playerId != playerId) return;
            
        toast.Show(message, duration);
    }
}
