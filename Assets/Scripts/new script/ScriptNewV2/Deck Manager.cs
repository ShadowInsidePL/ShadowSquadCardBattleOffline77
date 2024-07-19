using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardPrefab;

    void OnEnable()
    {
        SortAndLoadDeck();
        SubscribeToEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    void SubscribeToEvents()
    {
        GameManager.instance.OnCardAddedToDeck += OnCardAddedOrRemoved;
        GameManager.instance.OnCardRemovedFromDeck += OnCardAddedOrRemoved;
    }

    void UnsubscribeFromEvents()
    {
        GameManager.instance.OnCardAddedToDeck -= OnCardAddedOrRemoved;
        GameManager.instance.OnCardRemovedFromDeck -= OnCardAddedOrRemoved;
    }

    void OnCardAddedOrRemoved(string cardID)
    {
        SortAndLoadDeck();
    }

    void SortAndLoadDeck()
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        List<string> deck = GameManager.instance.GetDeck();
        List<CardScriptableObject> cardsInDeck = new List<CardScriptableObject>();
        List<CardScriptableObject> cardsNotInDeck = new List<CardScriptableObject>();

        // Pobierz wszystkie karty z kolekcji
        List<string> collectionCards = GameManager.instance.GetCollection();

        // Oddziel karty na dwie listy: w talii i nie w talii
        foreach (string cardID in collectionCards)
        {
            CardScriptableObject cardSO = GameManager.instance.GetCardByID(cardID);
            if (cardSO != null)
            {
                if (deck.Contains(cardID))
                {
                    cardsInDeck.Add(cardSO); // Dodaj karty z talii
                }
                else
                {
                    cardsNotInDeck.Add(cardSO); // Dodaj karty z kolekcji, które nie są w talii
                }
            }
        }

        // Sortuj karty w talii według własnych kryteriów
        cardsInDeck.Sort((card1, card2) => card1.cardName.CompareTo(card2.cardName));
        cardsNotInDeck.Sort((card1, card2) => card1.cardName.CompareTo(card2.cardName));

        // Instancjonuj karty w kolejności: najpierw karty w talii, potem karty nie w talii
        foreach (CardScriptableObject cardSO in cardsInDeck)
        {
            GameObject cardGO = Instantiate(cardPrefab, cardContainer);
            CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
            cardDisplay.cardSO = cardSO;
            cardDisplay.SetupCard();
        }

        foreach (CardScriptableObject cardSO in cardsNotInDeck)
        {
            GameObject cardGO = Instantiate(cardPrefab, cardContainer);
            CardDisplay cardDisplay = cardGO.GetComponent<CardDisplay>();
            cardDisplay.cardSO = cardSO;
            cardDisplay.SetupCard();
        }
    }
}
