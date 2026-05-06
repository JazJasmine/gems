// Jaz's Gems — Gemcraft
// Purpose: Core gacha collection game with essence economy, pulls, purity tiers, and leaderboard
// Used by: Gemcraft game system

using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Games
    {
        public enum Purity
        {
            Clouded = 0,
            Clear = 1,
            Gleaming = 2,
            Brilliant = 3,
            Pristine = 4,
            Perfect = 5
        }

        public class Gemcraft : EmeraldBehaviour
        {
            bool isActive;
            [SerializeField] GemcraftData referenceGemData;
            [SerializeField] GemcraftUI ui;
            [SerializeField] EssenceModifer modifier;
            GemcraftData localData;
            DataList everyoneData = new DataList();

            [SerializeField] int tickRate;
            [SerializeField] int pityCounter;
            float essenceFraction;
            bool _ticking;
            bool _leaderboardUpdating;

            // -----------------------------
            // Roll odds (sum to 1.0)
            // Clouded  45.00%
            // Clear    30.00%
            // Gleaming 10.00%
            // Brilliant13.60%
            // Pristine  1.32%
            // Perfect   0.08% (pull-only jackpot)
            // -----------------------------
            const float CLOUDED = 0.45f;
            const float CLEAR = 0.45f + 0.30f;     // 0.75
            const float GLEAMING = 0.45f + 0.30f + 0.10f;   // 0.85
            const float BRILLIANT = 0.45f + 0.30f + 0.10f + 0.136f; // 0.986
            const float PRISTINE = 0.45f + 0.30f + 0.10f + 0.136f + 0.0132f; // 0.9992
                                                                             // Perfect = >= 0.9992 (last 0.0008)

            // Thresholds
            const int THRESH_CLOUDED = 5;
            const int THRESH_GLEAMING = 13;
            const int THRESH_BRILLIANT = 25;
            const int THRESH_PRISTINE = 44;
            const int THRESH_PERFECT = 99;

            // Facet gains per quality
            //const int FACETS_CLOUDED = 1;
            //const int FACETS_CLEAR = 2;
            //const int FACETS_GLEAMING = 3;
            //const int FACETS_BRILLIANT = 4;
            //const int FACETS_PRISTINE = 8;

            //string[] GEMSTONES = { "Emerald", "Amethyst", "Sapphire", "Ruby", "Garnet", "Diamond", "Jade", "Onyx", "Pearl", "Quartz", "Morganite", "Citrine" };
            //string[] PULL_STRENGTH = { "Common", "Uncommon", "Rare", "Epic", "Legendary", "Perfect" };
            //string[] PURITIES = { "Clouded", "Clear", "Gleaming", "Brilliant", "Pristine", "Perfect" };

            int[] facetGains = { 1, 2, 3, 4, 8 };
            int[] facetThresholds = { 0, 5, 13, 25, 44 };
            int[] facetRanges = { 5, 8, 12, 19 };
            DataList gemProgress = new DataList() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            DataList normalizedGemProgress = new DataList() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            DataList lockedGemIds = new DataList();


            void Start()
            {
                localData = (GemcraftData)Networking.FindComponentInPlayerObjects(Networking.LocalPlayer, referenceGemData);

                //if(Utilities.IsValid(Networking.InstanceOwner))
                //{
                //    isActive = Networking.InstanceOwner.displayName == "JazJasmine" || Networking.InstanceOwner.displayName == "[1] Local Player";
                //}
            }

            public override void OnPlayerJoined(VRCPlayerApi player)
            {
                everyoneData.Add((GemcraftData)Networking.FindComponentInPlayerObjects(player, referenceGemData));

                if (player.displayName == "JazJasmine")
                {
                    isActive = true;
                }
            }

            public override void OnPlayerLeft(VRCPlayerApi player)
            {
                everyoneData.Remove((GemcraftData)Networking.FindComponentInPlayerObjects(player, referenceGemData));

                if (player.displayName == "JazJasmine")
                {
                    isActive = false;
                }
            }

            public override void OnPlayerRestored(VRCPlayerApi player)
            {
                if (!player.isLocal) return;

                ui.UpdateLocalEssence(localData.Essence);

                // Update gem progress
                if (PlayerData.HasKey(player, "Gemcraft.Progress"))
                {
                    if (VRCJson.TryDeserializeFromJson(PlayerData.GetString(player, "Gemcraft.Progress"), out DataToken json))
                    {
                        gemProgress = json.DataList;
                    }
                }

                if (PlayerData.HasKey(player, "Gemcraft.NormalizedProgress"))
                {
                    if (VRCJson.TryDeserializeFromJson(PlayerData.GetString(player, "Gemcraft.NormalizedProgress"), out DataToken json))
                    {
                        normalizedGemProgress = json.DataList;
                    }
                }

                RecalculateNormalizedProgress();
                PersistProgress();

                LogInfo($"[GC] OnPlayerRestored: essence={localData.Essence}, prestige={localData.Prestige}, progressCount={gemProgress.Count}");
                ui.Initalize();
                ui.UpdateDropdown(!localData.UnlockedAllGems);
                ui.UpdatePityCounter(pityCounter - localData.PullsSinceLegendary);
                ui.UpdateLocalPresitge(localData.Prestige);

                UpdateLockedGemList();
            }

            private void OnEnable()
            {
                if (!_ticking)
                {
                    _ticking = true;
                    SendCustomEventDelayedSeconds(nameof(Tick), tickRate);
                }
                if (!_leaderboardUpdating)
                {
                    _leaderboardUpdating = true;
                    SendCustomEventDelayedSeconds(nameof(LeaderboardUpdate), 1);
                }
            }

            private void OnDisable()
            {
                _ticking = false;
                _leaderboardUpdating = false;
            }

            public void Tick()
            {
                if (!_ticking) return;
                if (!isActive) return;

                // Gain fraction of an essence based on social multiplier each tick. If it's above 1, add extra essenec to gain
                essenceFraction += tickRate * modifier.SocialMultiplier;
                int socialGain = Mathf.FloorToInt(essenceFraction);
                essenceFraction -= socialGain;

                int essenceGain = 1 + socialGain;
                localData.IncreaseEssence(essenceGain);

                // Update local UI
                ui.UpdateLocalEssence(localData.Essence);

                SendCustomEventDelayedSeconds(nameof(Tick), tickRate);
            }

            [NetworkCallable]
            public void Pull()
            {
                if (localData.Essence < 25) return; // Cost of pull
                localData.DecreaseEssence(25);
                ui.UpdateLocalEssence(localData.Essence);

                int gemId = RollGemId();
                int rarityId = RollRarity();

                if (rarityId >= 4)
                {
                    // Reset pity counter
                    localData.PullsSinceLegendary = 0;
                }
                else
                {
                    localData.PullsSinceLegendary += 1;
                }

                bool isPityPull = false;
                if (localData.PullsSinceLegendary >= pityCounter)
                {
                    isPityPull = true;
                    localData.PullsSinceLegendary = 0;
                }

                int overwrittenRarityId = isPityPull ? 4 : rarityId;

                LogInfo($"[GC] Pull: essence={localData.Essence}, gemId={gemId}, rarity={overwrittenRarityId}, pity={localData.PullsSinceLegendary}");
                ui.StartPullVisuals(gemId, overwrittenRarityId);
            }

            public void ApplyGemAwakening(int gemId, int rarityId)
            {
                double currentProgress = gemProgress[gemId].Number;
                LogInfo($"[GC] ApplyAwakening START: gemId={gemId}, rarityId={rarityId}, currentProgress={currentProgress}, prestige={localData.Prestige}");
                ui.UpdatePityCounter(pityCounter - localData.PullsSinceLegendary);

                // Perfect pull — separate path to avoid out-of-bounds on facetGains
                if (rarityId == 5)
                {
                    int currentPurity = PurityFromProgress(currentProgress);
                    gemProgress[gemId] = THRESH_PERFECT;

                    // Award prestige: base 600 + any intermediate purity breakthroughs skipped
                    localData.IncreasePrestige(600);
                    int newPurity = (int)Purity.Perfect;
                    for (int p = currentPurity + 1; p < newPurity; p++)
                    {
                        localData.IncreasePrestige(PrestigeForPurity(p));
                    }
                    ui.UpdateLocalPresitge(localData.Prestige);

                    LogInfo($"[GC] ApplyAwakening PERFECT: gemId={gemId}, newProgress={THRESH_PERFECT}, prestige={localData.Prestige}");
                    normalizedGemProgress[gemId] = 1f;
                    PersistProgress();
                    UpdateLockedGemList();
                    return;
                }

                // Gem already at max — only award prestige
                if (currentProgress >= THRESH_PRISTINE)
                {
                    localData.IncreasePrestige(50);
                    ui.UpdateLocalPresitge(localData.Prestige);
                    normalizedGemProgress[gemId] = 1f;
                    LogInfo($"[GC] ApplyAwakening MAX: gemId={gemId}, +50 prestige, prestige={localData.Prestige}");
                    return;
                }

                // Normal pull (rarityId 0-4)
                int gain = facetGains[rarityId];
                double total = currentProgress + gain;
                gemProgress[gemId] = total;

                int prevPurity = PurityFromProgress(currentProgress);
                int nextPurity = PurityFromProgress(total);
                LogInfo($"[GC] ApplyAwakening NORMAL: gemId={gemId}, gain={gain}, total={total}, prevPurity={prevPurity}, nextPurity={nextPurity}, normalized={normalizedGemProgress[gemId].Number}");

                if (nextPurity > prevPurity)
                {
                    int prestigeGain = PrestigeForPurity(nextPurity);
                    localData.IncreasePrestige(prestigeGain);
                    ui.UpdateLocalPresitge(localData.Prestige);
                    LogInfo($"[GC] ApplyAwakening PURITY UP: prestigeGain={prestigeGain}, newPrestige={localData.Prestige}");
                }

                normalizedGemProgress[gemId] = NormalizeProgress(total);
                PersistProgress();
                UpdateLockedGemList();
            }

            void RecalculateNormalizedProgress()
            {
                for (int i = 0; i < 12; i++)
                {
                    double progress = gemProgress[i].Number;
                    if (progress <= 0)
                        normalizedGemProgress[i] = 0;
                    else if (progress >= THRESH_PRISTINE)
                        normalizedGemProgress[i] = 1f;
                    else
                        normalizedGemProgress[i] = NormalizeProgress(progress);
                }
            }

            void PersistProgress()
            {
                if (VRCJson.TrySerializeToJson(gemProgress, JsonExportType.Minify, out DataToken json))
                {
                    LogInfo($"[GC] PersistProgress: progress={json.String}");
                    PlayerData.SetString("Gemcraft.Progress", json.String);
                }

                if (VRCJson.TrySerializeToJson(normalizedGemProgress, JsonExportType.Minify, out DataToken normalizedJson))
                {
                    PlayerData.SetString("Gemcraft.NormalizedProgress", normalizedJson.String);
                }
            }

            public void LeaderboardUpdate()
            {
                if (!_leaderboardUpdating) return;
                if (!isActive) return;

                SortPlayersByPrestige(everyoneData);
                var localRank = GetLocalRank(everyoneData);

                ui.UpdateLeaderBoard(everyoneData, localData, localRank);

                SendCustomEventDelayedSeconds(nameof(LeaderboardUpdate), 30);
            }

            [NetworkCallable]
            public void OnSignatureGemChange(int gemId, string gemString)
            {
                localData.SetPrimaryGem(gemId, PurityFromProgress(gemProgress[gemId].Number));
                LeaderboardUpdate();
            }

            int RollGemId()
            {
                if (!localData.UnlockedAllGems)
                {
                    float pDiscover = Mathf.Clamp(0.2f + 0.02f * lockedGemIds.Count, 0.15f, 0.6f);

                    if (Random.Range(0f, 1f) < pDiscover)
                    {
                        // Pick random among locked gems
                        int randomIndex = Random.Range(0, lockedGemIds.Count);
                        return (int)lockedGemIds[randomIndex].Number;
                    }

                }

                return Random.Range(0, 12);
            }

            void UpdateLockedGemList()
            {
                if (!localData.UnlockedAllGems)
                {
                    lockedGemIds = new DataList();
                    for (int i = 0; i < 12; i++)
                    {
                        var progress = gemProgress[i].Number;
                        if (progress <= 0)
                        {
                            lockedGemIds.Add(i);
                        }
                    }
                    localData.UnlockedGems = 12 - lockedGemIds.Count;
                    ui.UpdateDropdown(!localData.UnlockedAllGems);
                }
            }

            int RollRarity()
            {
                float r = Random.Range(0f, 1f);

                if (r < CLOUDED) return 0;
                if (r < CLEAR) return 1;
                if (r < GLEAMING) return 2;
                if (r < BRILLIANT) return 3;
                if (r < PRISTINE) return 4;
                return 5;
            }

            float NormalizeProgress(double total)
            {
                int currentPurity = PurityFromProgress(total);
                if (currentPurity >= 4) return 1; // Already done, nothing to progress

                var a = (total - facetThresholds[currentPurity]);
                var b = (facetRanges[currentPurity]);
                if (a == b) return .99f;
                return (float)a / b;
            }

            public int PurityFromProgress(double progress)
            {
                if (progress == 99) return (int)Purity.Perfect;
                if (progress < THRESH_CLOUDED) return (int)Purity.Clouded;
                if (progress < THRESH_GLEAMING) return (int)Purity.Clear;
                if (progress < THRESH_BRILLIANT) return (int)Purity.Gleaming;
                if (progress < THRESH_PRISTINE) return (int)Purity.Brilliant;
                return (int)Purity.Pristine;
            }

            int PrestigeForPurity(int p)
            {
                switch (p)
                {
                    case (int)Purity.Clear: return 10;
                    case (int)Purity.Gleaming: return 25;
                    case (int)Purity.Brilliant: return 60;
                    case (int)Purity.Pristine: return 150;
                    case (int)Purity.Perfect: return 600;
                    default: return 0;
                }
            }

            bool HasHigherPrestige(GemcraftData a, GemcraftData b)
            {
                return a.Prestige > b.Prestige;
            }

            void SortPlayersByPrestige(DataList players)
            {
                int count = players.Count;

                for (int i = 0; i < count - 1; i++)
                {
                    int bestIndex = i;
                    GemcraftData best = (GemcraftData)players[i].Reference;

                    for (int j = i + 1; j < count; j++)
                    {
                        GemcraftData candidate = (GemcraftData)players[j].Reference;

                        if (HasHigherPrestige(candidate, best))
                        {
                            bestIndex = j;
                            best = candidate;
                        }
                    }

                    if (bestIndex != i)
                    {
                        DataToken temp = players[i];
                        players[i] = players[bestIndex];
                        players[bestIndex] = temp;
                    }
                }
            }

            int GetLocalRank(DataList players)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    GemcraftData candidate = (GemcraftData)players[i].Reference;
                    if (candidate.Owner.isLocal) return i + 1;
                }
                return -1;
            }

            public DataList GemProgress
            {
                get => gemProgress;
            }
            public DataList NormalizedProgress
            {
                get => normalizedGemProgress;
            }
        }
    }
}