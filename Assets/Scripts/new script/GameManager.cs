using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<CardScriptableObject> allCards = new List<CardScriptableObject>();
    private List<string> deck = new List<string>();
    private List<string> collectionCards = new List<string>();

    public List<string> CollectionCards => collectionCards;

    private const int MaxDeckSize = 10;

    public event Action<string> OnCardAddedToDeck;
    public event Action<string> OnCardRemovedFromDeck;
    public event Action<string> OnCardAddedToCollection;
    public event Action<string> OnCardRemovedFromCollection;
    public event Action<int> OnCoinsChanged;

    public int playerCoins;
    public int winCoinsReward;
    public string currentLevelName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<string> GetDeck()
    {
        return new List<string>(deck);
    }

    public void AddCardToDeck(string cardID)
    {
        if (deck.Count >= MaxDeckSize)
        {
            Debug.LogWarning("Deck is full. Cannot add more cards.");
            return;
        }

        if (!IsCardInDeck(cardID))
        {
            deck.Add(cardID);
            Debug.Log("Card added to deck: " + cardID);
            OnCardAddedToDeck?.Invoke(cardID);
        }
        else
        {
            Debug.LogWarning("Card is already in deck: " + cardID);
        }
    }

    public void RemoveCardFromDeck(string cardID)
    {
        if (deck.Contains(cardID))
        {
            deck.Remove(cardID);
            Debug.Log("Card removed from deck: " + cardID);
            OnCardRemovedFromDeck?.Invoke(cardID);
        }
        else
        {
            Debug.LogWarning("Card not found in deck: " + cardID);
        }
    }

    public bool IsCardInDeck(string cardID)
    {
        return deck.Contains(cardID);
    }

    public List<string> GetCollection()
    {
        return new List<string>(collectionCards);
    }

    public void AddCardToCollection(string cardID)
    {
        if (!IsCardInCollection(cardID))
        {
            collectionCards.Add(cardID);
            Debug.Log("Card added to collection: " + cardID);
            OnCardAddedToCollection?.Invoke(cardID);

            CardScriptableObject card = GetCardByID(cardID);
            if (card != null)
            {
                AddCardToAllCards(card);
            }
            else
            {
                Debug.LogWarning("Card not found with ID: " + cardID);
            }
        }
        else
        {
            Debug.LogWarning("Card is already in collection: " + cardID);
        }
    }

    public void RemoveCardFromCollection(string cardID)
    {
        if (collectionCards.Contains(cardID))
        {
            collectionCards.Remove(cardID);
            Debug.Log("Card removed from collection: " + cardID);
            OnCardRemovedFromCollection?.Invoke(cardID);
        }
        else
        {
            Debug.LogWarning("Card not found in collection: " + cardID);
        }
    }

    public bool IsCardInCollection(string cardID)
    {
        return collectionCards.Contains(cardID);
    }

    public CardScriptableObject GetCardByID(string cardID)
    {
        return allCards.Find(card => card.uniqueID == cardID);
    }

    public void AddCardToAllCards(CardScriptableObject card)
    {
        if (string.IsNullOrEmpty(card.uniqueID))
        {
            card.uniqueID = System.Guid.NewGuid().ToString();
        }

        if (!allCards.Exists(c => c.uniqueID == card.uniqueID))
        {
            allCards.Add(card);
            Debug.Log("Card added to allCards: " + card.cardName);
        }
        else
        {
            Debug.LogWarning("Card already in allCards: " + card.cardName);
        }
    }

    public void PrintAllCards()
    {
        Debug.Log("All cards in GameManager:");
        foreach (var card in allCards)
        {
            Debug.Log(card.cardName);
        }
    }

    public void SortCollectionByName()
    {
        collectionCards = collectionCards.OrderBy(id => GetCardByID(id).cardName).ToList();
        Debug.Log("Collection sorted by name.");
    }

    public void SortCollectionByManaCost()
    {
        collectionCards = collectionCards.OrderBy(id => GetCardByID(id).manaCost).ToList();
        Debug.Log("Collection sorted by mana cost.");
    }

    public void SaveCollection(string filePath)
    {
        File.WriteAllText(filePath, string.Join(",", collectionCards));
        Debug.Log("Collection saved to " + filePath);
    }

    public void LoadCollection(string filePath)
    {
        if (File.Exists(filePath))
        {
            string[] loadedCards = File.ReadAllText(filePath).Split(',');
            collectionCards = new List<string>(loadedCards);
            Debug.Log("Collection loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
        }
    }

    public void SaveDeck(string filePath)
    {
        File.WriteAllText(filePath, string.Join(",", deck));
        Debug.Log("Deck saved to " + filePath);
    }

    public void LoadDeck(string filePath)
    {
        if (File.Exists(filePath))
        {
            string[] loadedCards = File.ReadAllText(filePath).Split(',');
            deck = new List<string>(loadedCards);
            Debug.Log("Deck loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
        }
    }

    public void AddCoins(int amount)
    {
        playerCoins += amount;
        Debug.Log("Player received " + amount + " coins. Total coins: " + playerCoins);
        OnCoinsChanged?.Invoke(playerCoins);
    }

    public void SpendCoins(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            Debug.Log("Player spent " + amount + " coins. Total coins: " + playerCoins);
            OnCoinsChanged?.Invoke(playerCoins);
        }
        else
        {
            Debug.LogWarning("Not enough coins to spend. Current coins: " + playerCoins);
        }
    }

     public void CompleteLevel(string levelName)
    {

    Debug.Log("Completing level: " + levelName); // Dodaj logowanie przed zmianą stanu
        PlayerPrefs.SetInt(levelName, 1);
        PlayerPrefs.Save();
        Debug.Log("Level " + levelName + " completed.");
    }

    public bool IsLevelCompleted(string levelName)
    {
        return PlayerPrefs.GetInt(levelName, 0) == 1;
    }

    

}
