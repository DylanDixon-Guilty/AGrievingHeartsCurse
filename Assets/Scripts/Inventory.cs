using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Image backPack;
    [SerializeField] private GameObject hotBar;
    [SerializeField] private Sprite closedPack;
    [SerializeField] private Sprite openedPack;

    private bool isBackPackOpen;

    public void OpenBackPack()
    {
        if (!isBackPackOpen)
        {
            backPack.sprite = openedPack;
            hotBar.SetActive(true);
            isBackPackOpen = true;
        }
        else
        {
            backPack.sprite = closedPack;
            hotBar.SetActive(false);
            isBackPackOpen = false;
        }
    }
}
