
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class EssenceModifer : UdonSharpBehaviour
{
    [SerializeField] GemcraftUI ui;

    [Header("Tuning")]
    [SerializeField] float sampleIntervalSeconds = 3f;
    [SerializeField] float radiusMeters = 4.0f;
    [SerializeField] int maxContributors = 6;
    [SerializeField] float bonusPerPlayer = 0.03f;

    int nearbyCount;
    float socialMultiplier = 1f;

    // Cache players
    VRCPlayerApi localPlayer;
    VRCPlayerApi[] players;

    float radiusSqr;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        radiusSqr = radiusMeters * radiusMeters;
        RefreshPlayerCache();

        Sample();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RefreshPlayerCache();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        RefreshPlayerCache();
    }

    public void RefreshPlayerCache()
    {
        players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);
    }

    public void Sample()
    {
        Vector3 localPlayerPosition = localPlayer.GetPosition();

        int count = 0;
        for (int i = 0; i < players.Length; i++)
        {
            VRCPlayerApi p = players[i];
            if (p == null) continue;
            if (!Utilities.IsValid(p)) continue;
            if (p.isLocal) continue;

            Vector3 remotePlayerPosition = p.GetPosition();
            float dx = remotePlayerPosition.x - localPlayerPosition.x;
            float dy = remotePlayerPosition.y - localPlayerPosition.y;
            float dz = remotePlayerPosition.z - localPlayerPosition.z;

            float d2 = dx * dx + dy * dy + dz * dz;

            if (d2 <= radiusSqr)
            {
                count++;
                if (count >= maxContributors) break;
            }
        }

        nearbyCount = count;
        socialMultiplier = bonusPerPlayer * nearbyCount;
        ui.UpdateSocialModifier(socialMultiplier);

        SendCustomEventDelayedSeconds(nameof(Sample), sampleIntervalSeconds);
    }

    public float SocialMuliplier
    {
        get => socialMultiplier;
    }
}
