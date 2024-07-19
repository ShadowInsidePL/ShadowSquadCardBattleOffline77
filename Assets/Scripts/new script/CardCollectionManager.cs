using System.Collections.Generic;
using UnityEngine;

public class CardCollectionManager : MonoBehaviour
{
    public static CardCollectionManager instance;

    public List<CardScriptableObject> cardCollection = new List<CardScriptableObject>();

    void Awake()
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

    public void AddCardToCollection(CardScriptableObject card)
    {
        if (card != null && !cardCollection.Contains(card))
        {
            cardCollection.Add(card);
            Debug.Log("Card added to collection: " + card.name);
        }
    }

    public void RemoveCardFromCollection(CardScriptableObject card)
    {
        if (card != null && cardCollection.Contains(card))
        {
            cardCollection.Remove(card);
            Debug.Log("Card removed from collection: " + card.name);
        }
    }
}
