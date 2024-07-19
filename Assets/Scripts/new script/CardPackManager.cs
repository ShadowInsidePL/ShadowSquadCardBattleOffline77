using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardPackManager : MonoBehaviour
{
    public List<CardScriptableObject> availableCards;
    public int packPrice;
    public UIManager uiManager;
    public string purchaseSceneName;
    public Button buyButton;
    public Button openButton;
    public Animator animator;
    public int PackDrop;
    public static CardPackManager instance;

    public delegate void LegendaryCardDrawnHandler(CardScriptableObject legendaryCard);
    public event LegendaryCardDrawnHandler OnLegendaryCardDrawn;

    private void Awake()
    {
        instance = this;
    }

    private List<List<CardScriptableObject>> cardPacks = new List<List<CardScriptableObject>>();
    private int currentPackIndex = 0;

    private void Start()
    {
        cardPacks = new List<List<CardScriptableObject>>();
        Debug.Log("CardPackManager initialized. Number of card packs: " + cardPacks.Count);

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClick);
        }
        else
        {
            Debug.LogError("Buy button not assigned.");
        }

        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenCardPack);
            // Sprawdzenie stanu przycisku "Open"
            if (PlayerPrefs.GetInt("PackOpened", 0) == 1)
            {
                openButton.gameObject.SetActive(false);
            }
            else
            {
                openButton.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Open button not assigned.");
        }

        GameManager.instance.OnCoinsChanged += UpdatePlayerCoins;
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnCoinsChanged -= UpdatePlayerCoins;
        }
    }

    private void UpdatePlayerCoins(int coins)
    {
        Debug.Log("Coins updated to: " + coins);
    }

    public void OnBuyButtonClick()
    {
        AttemptPurchase();
    }

    private void AttemptPurchase()
    {
        int playerCoins = GameManager.instance.playerCoins;
        if (playerCoins >= packPrice)
        {
            GameManager.instance.SpendCoins(packPrice);

            SceneManager.LoadScene(purchaseSceneName);
        }
        else
        {
            Debug.Log("Za mało monet.");
        }
    }

    public void OpenCardPack()
    {
        List<CardScriptableObject> drawnCards = DrawCardsFromPack();

        cardPacks.Add(drawnCards);
        AddCardsToCollection(drawnCards);
        DisplayPurchasedCards(drawnCards);
        DisplayCurrentPack();

        // Ustawienie flagi, że paczka została otwarta
        PlayerPrefs.SetInt("PackOpened", 1);
        PlayerPrefs.Save();

        // Ukrycie przycisku "Open"
        openButton.gameObject.SetActive(false);
    }

    private List<CardScriptableObject> DrawCardsFromPack()
    {
        List<CardScriptableObject> drawnCards = new List<CardScriptableObject>();

        for (int i = 0; i < PackDrop; i++)
        {
            float totalWeight = 0;
            foreach (var card in availableCards)
            {
                totalWeight += card.dropRate;
            }

            float randomValue = Random.Range(0, totalWeight);
            float cumulativeWeight = 0;

            foreach (var card in availableCards)
            {
                cumulativeWeight += card.dropRate;
                if (randomValue <= cumulativeWeight)
                {
                    CardScriptableObject drawnCard = Instantiate(card);
                    drawnCard.uniqueID = System.Guid.NewGuid().ToString();
                    drawnCards.Add(drawnCard);
                    Debug.Log("Wylosowano kartę: " + drawnCard.cardName + " z ID: " + drawnCard.uniqueID);

                    if (drawnCard.rarity == CardRarity.Legendary)
                    {
                        Debug.Log("Karta legendarna wylosowana: " + drawnCard.cardName);
                        animator.SetTrigger("CardShop");
                        OnLegendaryCardDrawn?.Invoke(drawnCard);
                    }
                    break;
                }
            }
        }

        return drawnCards;
    }

    public void SwitchPack(int packIndex)
    {
        if (packIndex >= 0 && packIndex < cardPacks.Count)
        {
            currentPackIndex = packIndex;
            DisplayCurrentPack();
        }
        else
        {
            Debug.LogError("Nieprawidłowy indeks paczki.");
        }
    }

    private void DisplayCurrentPack()
    {
        if (cardPacks.Count == 0)
        {
            Debug.LogError("Brak paczek do wyświetlenia.");
            return;
        }

        List<CardScriptableObject> currentPack = cardPacks[currentPackIndex];
        GameObject parentObject = GameObject.Find("CardDisplayParent");

        if (parentObject != null)
        {
            Transform parentTransform = parentObject.transform;
            if (uiManager != null)
            {
                uiManager.DisplayDrawnCards(currentPack, parentTransform);
            }
            else
            {
                Debug.LogError("UIManager nie jest przypisany.");
            }
        }
        else
        {
            Debug.LogError("GameObject CardDisplayParent nie został znaleziony.");
        }
    }

    public List<List<CardScriptableObject>> GetCardPacks()
    {
        return cardPacks;
    }

    private void DisplayPurchasedCards(List<CardScriptableObject> cards)
    {
        Debug.Log("Nowo zakupione karty:");
        foreach (var card in cards)
        {
            Debug.Log(card.cardName);
        }
    }

    private void AddCardsToCollection(List<CardScriptableObject> drawnCards)
    {
        foreach (var card in drawnCards)
        {
            GameManager.instance.AddCardToCollection(card.uniqueID);
            Debug.Log("Dodano kartę do kolekcji: " + card.cardName + " z ID: " + card.uniqueID);
        }
    }

    private void OnEnable()
    {
        // Resetowanie flagi przy ponownym wejściu na scenę
        PlayerPrefs.SetInt("PackOpened", 0);
        PlayerPrefs.Save();

        if (openButton != null)
        {
            openButton.gameObject.SetActive(true);
        }
    }
}
