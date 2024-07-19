using UnityEngine;
using System.Collections.Generic;

public class DeckDisplay : MonoBehaviour
{
    public GameObject cardDisplayPrefab; // Prefab karty do wyświetlania
    public RectTransform contentParent; // Kontener na karty (RectTransform dla UI)

    private List<string> currentDeck; // Lista ID kart w aktualnym decku

    void OnEnable()
    {
        GameManager.instance.OnCardAddedToDeck += HandleCardAddedToDeck;
        GameManager.instance.OnCardRemovedFromDeck += HandleCardRemovedFromDeck;
        LoadDeck();
        DisplayDeck();
    }

    void OnDisable()
    {
        GameManager.instance.OnCardAddedToDeck -= HandleCardAddedToDeck;
        GameManager.instance.OnCardRemovedFromDeck -= HandleCardRemovedFromDeck;
    }

    void LoadDeck()
    {
        currentDeck = GameManager.instance.GetDeck();
        Debug.Log("Loaded deck cards: " + currentDeck.Count);
    }

    void DisplayDeck()
    {
        // Czyszczenie istniejących kart z UI
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Wyświetlanie kart z bieżącej talii
        foreach (string cardID in currentDeck)
        {
            AddCardToDeckUI(cardID);
        }
    }

    void HandleCardAddedToDeck(string cardID)
    {
        if (!currentDeck.Contains(cardID))
        {
            currentDeck.Add(cardID);
            AddCardToDeckUI(cardID);
            Debug.Log($"Added card to UI: {cardID}");
        }
    }

    void HandleCardRemovedFromDeck(string cardID)
    {
        if (currentDeck.Contains(cardID))
        {
            currentDeck.Remove(cardID);
            RemoveCardFromDeckUI(cardID);
            Debug.Log($"Removed card from UI: {cardID}");
        }
    }

    void AddCardToDeckUI(string cardID)
    {
        CardScriptableObject card = GameManager.instance.GetCardByID(cardID);
        if (card != null)
        {
            // Instancjonowanie karty jako dziecko kontenera contentParent
            GameObject cardObject = Instantiate(cardDisplayPrefab, contentParent);

            // Ustawienie RectTransform
            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
            cardRectTransform.anchoredPosition = Vector2.zero;
            cardRectTransform.localScale = Vector3.one * 100; // Ustawienie skali na 100

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

    void RemoveCardFromDeckUI(string cardID)
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
}
