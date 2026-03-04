
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

public class Pagination : UdonSharpBehaviour
{
    [SerializeField] GameObject[] pages;
    [SerializeField] TextMeshProUGUI pageLabel;
    [SerializeField] Gems.UI.ButtonColored nextBtn;
    [SerializeField] Gems.UI.ButtonColored prevBtn;

    int currentPage;

    private void Start()
    {
        foreach (var page in pages)
        {
            page.SetActive(false);
        }
        pages[0].SetActive(true);
        pageLabel.text = "1/7";
    }

    private void OnEnable()
    {
        nextBtn.Disabled = currentPage >= 6;
        prevBtn.Disabled = currentPage <= 0;
    }

    [NetworkCallable]
    public void NextPage()
    {
        if (currentPage >= 6) return;

        pages[currentPage].SetActive(false);
        currentPage += 1;
        pages[currentPage].SetActive(true);
        pageLabel.text = $"{currentPage + 1}/7";

        nextBtn.Disabled = currentPage >= 6;
        prevBtn.Disabled = currentPage <= 0;
    }

    [NetworkCallable]
    public void PreviousPage()
    {
        if (currentPage <= 0) return;

        pages[currentPage].SetActive(false);
        currentPage -= 1;
        pages[currentPage].SetActive(true);
        pageLabel.text = $"{currentPage + 1}/7";

        nextBtn.Disabled = currentPage >= 6;
        prevBtn.Disabled = currentPage <= 0;
    }
}
