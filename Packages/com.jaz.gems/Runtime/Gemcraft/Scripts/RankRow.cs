
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class RankRow : UdonSharpBehaviour
{
    [SerializeField] GameObject values;

    [Header("Values")]
    [SerializeField] Image rankIcon;
    [SerializeField] Image[] purityStars;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI player;
    [SerializeField] TextMeshProUGUI prestige;
    [SerializeField] TextMeshProUGUI totalEarned;

    int currentPurity = -1;
    Color currentColor = new Color(1,1,1,1);

    public void SetActive(bool active)
    {
        values.SetActive(active);
    }

    public void UpdateValues(Sprite gemIcon, Color gemColor, int purityId, string playerName, int prestigeValue, int total)
    {
        if (currentColor != gemColor)
        {
            // Color changed
            rankIcon.color = gemColor;
            foreach (var star in purityStars)
            {
                star.color = gemColor;
            }
        }

        if (currentPurity != purityId)
        {
            // Purity Changed
            foreach (var star in purityStars)
            {
                star.gameObject.SetActive(false);
            }

            for (int i = 0; i <= purityId; i++)
            {
                purityStars[i].gameObject.SetActive(true);
            }
        }

        rankIcon.sprite = gemIcon;
        player.text = playerName;
        prestige.text = prestigeValue.ToString();
        totalEarned.text = total.ToString();


        currentPurity = purityId;
        currentColor = gemColor;
    }

    public void UpdateRank(int r)
    {
        rank.text = $"{r}.";
    }
}
