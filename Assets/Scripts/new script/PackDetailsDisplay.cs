using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PackDetailsDisplay : MonoBehaviour
{
    public TMP_Text packNameText;   // Użyj TextMesh Pro dla tekstu nazwy paczki
    public TMP_Text packPriceText;  // Użyj TextMesh Pro dla tekstu ceny paczki
    public Image packImage;         // Grafika paczki pozostaje bez zmian

    public void SetPackDetails(string packName, int packPrice, Sprite image)
    {
        packNameText.text = packName;
        packPriceText.text = packPrice.ToString() + " Coins";
        packImage.sprite = image;
    }
}
