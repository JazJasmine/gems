
using Newtonsoft.Json.Linq;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class GemcraftData : UdonSharpBehaviour
{
    VRCPlayerApi owner;
    [UdonSynced] int prestige;
    [UdonSynced] int totalEssenceEarned;
    [UdonSynced] int primaryGem = -1;
    [UdonSynced] int primaryGemPurity = 0;

    [UdonSynced] int essence; // <- this is only synced as I need to persist and I am lazy.
    [UdonSynced] int unlockedGems = 0;
    [UdonSynced] int pullsSinceLegendary = 0;

    readonly int MAX_ESSENCE = 600;

    private void Start()
    {
        owner = Networking.GetOwner(gameObject);
    }

    public void IncreaseEssence(int value)
    {
        if (essence >= MAX_ESSENCE) return;
        essence = Mathf.Min(MAX_ESSENCE, essence + value); // Ceil essence to MAX_ESSENCE

        totalEssenceEarned += value;
    }

    public void DecreaseEssence(int value)
    {
        essence = Mathf.Max(0, essence - value); // Ceil essence to 0
    }

    public void IncreasePrestige(int value)
    {
        prestige += value;
    }

    public void SetPrimaryGem(int gemId, int gemPurity)
    {
        primaryGem = gemId;
        primaryGemPurity = gemPurity;
    }

    public VRCPlayerApi Owner
    {
        get => owner;
    }

    public int Prestige
    {
        get => prestige;
    }

    public int TotalEssenceEarned
    {
        get => totalEssenceEarned;
    }

    public int PrimaryGem
    {
        get => primaryGem;
    }

    public int PrimaryGemPurity
    {
        get => primaryGemPurity;
    }

    public int Essence
    {
        get => essence;
    }

    public int UnlockedGems
    {
        get => unlockedGems;
        set {
            unlockedGems = value;
        }
    }

    public bool UnlockedAllGems
    {
        get => unlockedGems >= 12;
    }

    public int PullsSinceLegendary
    {
        get => pullsSinceLegendary;
        set
        {
            pullsSinceLegendary = value;
        }
    }
}
