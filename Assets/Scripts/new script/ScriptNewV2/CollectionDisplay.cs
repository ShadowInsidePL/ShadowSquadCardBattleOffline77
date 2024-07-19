using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CollectionDisplay : MonoBehaviour
{
    public GameObject cardDisplayPrefab; // Prefab karty do wyświetlania
    public RectTransform contentParent; // Kontener na karty (RectTransform dla UI)

    private List<string> currentCollection; // Lista ID kart w aktualnej kolekcji

    void OnEnable()
    {
        GameManager.instance.OnCardAddedToCollection += HandleCardAddedToCollection;
        GameManager.instance.OnCardRemovedFromCollection += HandleCardRemovedFromCollection;
        LoadCollection();
        DisplayCollection();
    }

    void OnDisable()
    {
        GameManager.instance.OnCardAddedToCollection -= HandleCardAddedToCollection;
        GameManager.instance.OnCardRemovedFromCollection -= HandleCardRemovedFromCollection;
    }

    void LoadCollection()
    {
        currentCollection = GameManager.instance.GetCollection();
        Debug.Log("Loaded collection cards: " + currentCollection.Count);
    }

    void DisplayCollection()
    {
        // Usuń wszystkie dzieci z contentParent, aby uniknąć duplikatów
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        foreach (string cardID in currentCollection)
        {
            AddCardToCollectionUI(cardID);
        }

        // Force rebuild layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent);
    }

    void HandleCardAddedToCollection(string cardID)
    {
        if (!currentCollection.Contains(cardID))
        {
            currentCollection.Add(cardID);
            AddCardToCollectionUI(cardID);
            Debug.Log($"Added card to collection UI: {cardID}");
        }
    }

    void HandleCardRemovedFromCollection(string cardID)
    {
        if (currentCollection.Contains(cardID))
        {
            currentCollection.Remove(cardID);
            RemoveCardFromCollectionUI(cardID);
            Debug.Log($"Removed card from collection UI: {cardID}");
        }
    }

    void AddCardToCollectionUI(string cardID)
    {
        CardScriptableObject card = GameManager.instance.GetCardByID(cardID);
        if (card != null)
        {
            // Instancjonowanie karty jako dziecko kontenera contentParent
            GameObject cardObject = Instantiate(cardDisplayPrefab, contentParent);

            // Ustawienie wyświetlania karty
            CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
            cardDisplay.cardSO = card;
            cardDisplay.SetupCard();

            // Dodanie debugowania
            Debug.Log($"Displayed card: {cardID}");
        }
        else
        {
            Debug.LogWarning("Card not found with ID: " + cardID);
        }
    }

    void RemoveCardFromCollectionUI(string cardID)
    {
        foreach (Transform child in contentParent)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();
            if (cardDisplay != null && cardDisplay.cardSO.uniqueID == cardID)
            {
                Destroy(child.gameObject);
                Debug.Log($"Destroyed card UI for: {cardID}");
                break;
            }
        }
    }

    // Metoda do odświeżania kolekcji kart
    public void RefreshCollectionDisplay()
    {
        currentCollection = GameManager.instance.GetCollection();
        DisplayCollection();
    }
}
