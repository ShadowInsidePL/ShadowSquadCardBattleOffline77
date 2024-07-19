using UnityEngine;

public class PackSwitcher : MonoBehaviour
{
    public CardPackManager cardPackManager; // Referencja do menedżera paczek kart

    public void NextPack()
    {
        cardPackManager.SwitchPack(1);
    }

    public void PreviousPack()
    {
        cardPackManager.SwitchPack(-1);
    }
}
