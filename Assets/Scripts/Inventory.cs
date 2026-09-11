using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Image BackPack;
    [SerializeField] private Sprite ClosedPack;
    [SerializeField] private Sprite OpenedPack;

    private bool isBackPackOpen;

    public void OpenBackPack()
    {
        if (!isBackPackOpen)
        {
            BackPack.sprite = OpenedPack;
            isBackPackOpen = true;
        }
        else
        {
            BackPack.sprite = ClosedPack;
            isBackPackOpen = false;
        }
    }
}
