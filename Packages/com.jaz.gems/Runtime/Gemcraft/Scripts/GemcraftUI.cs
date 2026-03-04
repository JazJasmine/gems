
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace Gems
{
    namespace Games
    {
        public class GemcraftUI : UdonSharpBehaviour
        {
            [SerializeField] Gemcraft gemcraft;


            // { "Emerald", "Amethyst", "Sapphire", "Ruby", "Garnet", "Diamond", "Jade", "Onyx", "Pearl", "Quartz", "Morganite", "Citrine" };
            public Color Emerald;
            public Color Amethyst;
            public Color Sapphire;
            public Color Ruby;
            public Color Garnet;
            public Color Diamond;
            public Color Jade;
            public Color Onyx;
            public Color Pearl;
            public Color Quartz;
            public Color Morganite;
            public Color Citrine;
            Color transparent = new Color(0, 0, 0, 0);

            Color[] byId;
            [Header("Static UI Elements")]
            [SerializeField] RankRow[] rankRows;
            [SerializeField] RankRow personalRank;
            [SerializeField] Gems.UI.Dropdown signatureGemDropdown;


            [Header("Mobile UI Elements")]
            [SerializeField] Canvas progressCanvas;
            [SerializeField] GameObject helpCanvas;
            [SerializeField] TextMeshProUGUI localEssence;
            [SerializeField] TextMeshProUGUI localPrestige;
            [SerializeField] TextMeshProUGUI localEssenceBoost;
            [SerializeField] Gems.UI.Button btn;
            [SerializeField] Image gemIcon;
            [SerializeField] TextMeshProUGUI gemLabel;
            [SerializeField] TextMeshProUGUI rarityLabel;
            [SerializeField] TextMeshProUGUI pityLabel;
            [SerializeField] CircleGemProgress pullGemProgress;
            [SerializeField] CircleGemProgress[] gemProgress;
            [SerializeField] AudioSource greatSound;
            [SerializeField] AudioSource normalSound;
            [SerializeField] AudioSource badSound;

            [SerializeField] Sprite questionMarkSprite;
            [SerializeField] Sprite[] gemSprites;
            string[] GEMSTONES = { "Emerald", "Amethyst", "Sapphire", "Ruby", "Garnet", "Diamond", "Jade", "Onyx", "Pearl", "Quartz", "Morganite", "Citrine" };
            string[] PULL_STRENGTH = { "Common", "Uncommon", "Rare", "Epic", "Legendary", "Perfect" };


            float shuffleDuration = 2.0f;
            float startInterval = 0.05f;
            float endInterval = 0.18f;

            int wCommon = 40;
            int wUncommon = 25;
            int wRare = 18;
            int wEpic = 12;
            int wLegendary = 4;
            int wMythic = 1;

            // Store result intermediately
            int resultGemId;
            int resultRarity;
            float endTime;
            float currentInterval;

            void Start()
            {
                byId = new Color[] { Emerald, Amethyst, Sapphire, Ruby, Garnet, Diamond, Jade, Onyx, Pearl, Quartz, Morganite, Citrine };
            }

            public void Initalize()
            {
                CloseCollection();
                CloseHelp();
                gemLabel.text = "???";
                rarityLabel.text = "???";
                pullGemProgress.SetFillAmount(0);
                gemIcon.sprite = questionMarkSprite;

                for (int i = 0; i < gemProgress.Length; i++)
                {
                    var progress = gemProgress[i];
                    int total = (int)gemcraft.GemProgress[i].Number;

                    if (total <= 0)
                    {
                        progress.Unknown = true;
                    }
                    else
                    {
                        progress.Unknown = false;
                        var purityId = gemcraft.PurityFromProgress(total);
                        progress.SetPurity(PurityToString(purityId), purityId);
                        progress.SetFillAmount((float)gemcraft.NormalizedProgress[i].Number);
                    }
                }
            }

            public void UpdateLocalEssence(int value)
            {
                localEssence.text = $"{value}/600";
            }

            public void UpdateLocalPresitge(int value)
            {
                localPrestige.text = $"Prestige: {value}";
            }
            public void UpdateSocialModifier(float percentage)
            {
                var inP = (int)Mathf.Floor(percentage * 100);
                localEssenceBoost.text = $"+{inP}%";
            }

            public void UpdatePityCounter(int value)
            {
                pityLabel.text = $"{value} left for guarenteed legendary";
            }

            public void StartPullVisuals(int gemId, int rarityId)
            {
                btn.Disabled = true;
                resultGemId = gemId;
                resultRarity = rarityId;

                endTime = Time.time + shuffleDuration;
                currentInterval = startInterval;

                PullStep();
            }

            public void PullStep()
            {
                if (Time.time >= endTime)
                {
                    RevealResult();
                    return;
                }

                int gemId = Random.Range(0, gemSprites.Length);
                int rarityIndex = RollTeaseRarityIndex();

                ApplyGemPullVisuals(gemId, rarityIndex);

                float t = 1f - Mathf.Clamp01((endTime - Time.time) / shuffleDuration); // 0..1
                currentInterval = Mathf.Lerp(startInterval, endInterval, t);

                SendCustomEventDelayedSeconds(nameof(PullStep), currentInterval);
            }

            void RevealResult()
            {
                // Show the real result
                ApplyGemPullVisuals(resultGemId, resultRarity);

                if (resultRarity >= 4)
                {
                    // Legendary or Perfect Pull. Play great sound
                    greatSound.Play();
                }
                else if (resultRarity == 3)
                {
                    // Epic pull
                    normalSound.Play();
                }
                else
                {
                    badSound.Play();
                }

                gemcraft.ApplyGemAwakening(resultGemId, resultRarity);
                SendCustomEventDelayedSeconds(nameof(DelayedUpdate), .2f);

            }

            public void DelayedUpdate()
            {
                var progress = gemProgress[resultGemId];
                var target = (float)gemcraft.NormalizedProgress[resultGemId].Number;
                pullGemProgress.SetFillSmooth(target);
                progress.SetFillSmooth(target);
                progress.Unknown = false;

                int total = (int)gemcraft.GemProgress[resultGemId].Number;
                var purityId = gemcraft.PurityFromProgress(total);
                progress.SetPurity(PurityToString(purityId), purityId);

                SendCustomEventDelayedSeconds(nameof(FinishedUpdate), .25f);
            }

            public void FinishedUpdate()
            {
                btn.Disabled = false;
            }

            int RollTeaseRarityIndex()
            {
                int total = wCommon + wUncommon + wRare + wEpic + wLegendary + wMythic;
                int r = Random.Range(0, total);

                if ((r -= wCommon) < 0) return 0;
                if ((r -= wUncommon) < 0) return 1;
                if ((r -= wRare) < 0) return 2;
                if ((r -= wEpic) < 0) return 3;
                if ((r -= wLegendary) < 0) return 4;
                return 5;
            }

            void ApplyGemPullVisuals(int gemId, int rarityId)
            {
                gemIcon.sprite = gemSprites[gemId];
                gemLabel.text = GEMSTONES[gemId];
                rarityLabel.text = PULL_STRENGTH[rarityId];

                gemIcon.color = byId[gemId];
                pullGemProgress.Color = byId[gemId];
                pullGemProgress.SetFillAmount((float)gemcraft.NormalizedProgress[gemId].Number);
            }

            public void UpdateLeaderBoard(DataList orderedPlayers, GemcraftData localData, int localRank)
            {
                for (int i = 0; i <= 5; i++)
                {
                    var currentRankRow = rankRows[i];

                    if (orderedPlayers.TryGetValue(i, out var d))
                    {
                        GemcraftData data = (GemcraftData)d.Reference;
                        Sprite gemSprite = data.PrimaryGem == -1 ? null : gemSprites[data.PrimaryGem];
                        Color gemColor = gemSprite == null ? transparent : byId[data.PrimaryGem];
                        currentRankRow.UpdateValues(gemSprite, gemColor, data.PrimaryGemPurity, data.Owner.displayName, data.Prestige, data.TotalEssenceEarned);
                        currentRankRow.SetActive(true);
                    }
                    else
                    {
                        currentRankRow.SetActive(false);
                    }
                }


                Sprite localSprite = localData.PrimaryGem == -1 ? null : gemSprites[localData.PrimaryGem];
                Color localColor = localData.PrimaryGem == -1 ? transparent : byId[localData.PrimaryGem];
                personalRank.UpdateValues(localSprite, localColor, localData.PrimaryGemPurity, localData.Owner.displayName, localData.Prestige, localData.TotalEssenceEarned);
                personalRank.UpdateRank(localRank);
            }

            public void UpdateDropdown(bool disabled)
            {
                signatureGemDropdown.Disabled = disabled;
            }

            string PurityToString(int purityId)
            {
                switch (purityId)
                {
                    case (int)Purity.Clouded: return "Clouded";
                    case (int)Purity.Clear: return "Clear";
                    case (int)Purity.Gleaming: return "Gleaming";
                    case (int)Purity.Brilliant: return "Brilliant";
                    case (int)Purity.Pristine: return "Pristine";
                    case (int)Purity.Perfect: return "Perfect";
                }

                return "";
            }

            [NetworkCallable]
            public void ToggleCollection()
            {
                progressCanvas.enabled = !progressCanvas.enabled;
            }

            void CloseCollection()
            {
                progressCanvas.enabled = false;
            }

            [NetworkCallable]
            public void ToggleHelp()
            {
                helpCanvas.SetActive(!helpCanvas.activeSelf);
            }

            void CloseHelp()
            {
                helpCanvas.SetActive(false);
            }
        }
    }
}