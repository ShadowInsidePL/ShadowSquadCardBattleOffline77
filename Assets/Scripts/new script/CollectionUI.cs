using System.Collections.Generic;
using UnityEngine;

public class CollectionUI : MonoBehaviour
{
    public GameObject cardUIPrefab; // Prefab UI do wyświetlania karty
    public Transform collectionPanel; // Panel, w którym będą wyświetlane karty

    public void DisplayCollection(List<CardScriptableObject> cards)
    {
        // Usuń stare karty z panelu
        foreach (Transform child in collectionPanel)
        {
            Destroy(child.gameObject);
        }

        // Dodaj nowe karty do panelu
        foreach (CardScriptableObject cardSO in cards)
        {
            GameObject cardUI = Instantiate(cardUIPrefab, collectionPanel);
            CardDisplay cardDisplay = cardUI.GetComponent<CardDisplay>();
            cardDisplay.cardSO = cardSO;
            cardDisplay.SetupCard();
        }
    }
}
