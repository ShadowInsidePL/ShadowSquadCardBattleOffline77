using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;
    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public List<List<CardScriptableObject>> cardPacks = new List<List<CardScriptableObject>>();
    private int currentPackIndex = 0;

    public Card cardToSpawn;

    public int drawCardCost = 2;

    public float waitBetweenDrawingCards = .25f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (GameManager.instance != null)
        {
            List<string> cardIDs = GameManager.instance.GetDeck();
            deckToUse = new List<CardScriptableObject>();

            foreach (string cardID in cardIDs)
            {
                CardScriptableObject card = GameManager.instance.GetCardByID(cardID);
                if (card != null)
                {
                    deckToUse.Add(card);
                }
                else
                {
                    Debug.LogWarning("Card with ID " + cardID + " not found in allCards.");
                }
            }
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Using default deck.");
            if (cardPacks.Count > 0)
            {
                deckToUse = new List<CardScriptableObject>(cardPacks[currentPackIndex]);
            }
        }
        SetupDeck();
    }

    void Update()
    {
        // Usuń komentarz, jeśli chcesz, aby naciśnięcie klawisza T ręcznie rysowało kartę
        // if(Input.GetKeyDown(KeyCode.T))
        // {
        //     DrawCardToHand();
        // }
    }

    public void SetupDeck()
    {
        activeCards.Clear();
        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++;
        }

        Debug.Log("Deck setup complete. Total active cards: " + activeCards.Count);
    }

   public void DrawCardToHand()
{
    if (activeCards.Count == 0)
    {
        Debug.LogWarning("No cards available in activeCards. Setting up deck.");
        SetupDeck();
    }

    if (activeCards.Count > 0)
    {
        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        newCard.cardSO = activeCards[0];
        activeCards.RemoveAt(0);

        Debug.Log("Drawing card: " + newCard.cardSO.cardName);
        HandController.instance.AddCardToHand(newCard);
        AudioManager.instance.PlaySFX(3);
    }
    else
    {
        Debug.LogError("Failed to draw card. activeCards is empty.");
    }
}



    public void DrawCardForMana()
    {
        if (BattleController.instance.playerMana >= drawCardCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerMana(drawCardCost);
        }
        else
        {
            UIController.instance.ShowManaWarning();
            UIController.instance.drawCardButton.SetActive(false);
        }
    }

    public void DrawMultipleCards(int amountToDraw)
    {
        StartCoroutine(DrawMultipleCo(amountToDraw));
    }

   IEnumerator DrawMultipleCo(int amountToDraw)
{
    for (int i = 0; i < amountToDraw; i++)
    {
        if (activeCards.Count == 0)
        {
            Debug.LogWarning("No more cards to draw. Setting up deck.");
            SetupDeck();
        }

        if (activeCards.Count > 0)
        {
            DrawCardToHand();
            Debug.Log("Card drawn to hand. Remaining cards in active deck: " + activeCards.Count);
        }
        else
        {
            Debug.LogWarning("Failed to draw card. activeCards is empty.");
            break; // Wyjście z pętli, jeśli brak kart
        }

        yield return new WaitForSeconds(waitBetweenDrawingCards);
    }
}


    public void AddCardToDeck(CardScriptableObject card)
    {
        deckToUse.Add(card);
    }

    public void RemoveCardFromDeck(CardScriptableObject card)
    {
        deckToUse.Remove(card);
    }

    public void SwitchPack(int packIndex)
    {
        if (packIndex >= 0 && packIndex < cardPacks.Count)
        {
            currentPackIndex = packIndex;
            deckToUse = new List<CardScriptableObject>(cardPacks[currentPackIndex]);
            SetupDeck();
        }
    }

    [System.Serializable]
    public class DeckData
    {
        public List<string> cardNames;

        public DeckData(List<string> cardNames)
        {
            this.cardNames = cardNames;
        }
    }
}
