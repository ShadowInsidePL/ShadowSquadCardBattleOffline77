using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public TMP_Text healthText, attackText, costText, nameText, actionDescriptionText, loreText;
    public Image characterArt, bgArt;
    public Button addButton, removeButton, collectionButton;
    public GameObject legendaryEffectPrefab;

    private bool isInDeck = false;

    void Start()
    {
        if (cardSO != null)
        {
            SetupCard();
        }
        else
        {
            Debug.LogWarning("Card ScriptableObject is not assigned.");
        }

        if (collectionButton != null)
        {
            collectionButton.onClick.AddListener(AddToCollections);
        }

        if (addButton != null)
        {
            addButton.onClick.AddListener(AddToDeck);
        }

        if (removeButton != null)
        {
            removeButton.onClick.AddListener(RemoveFromDeck);
        }

        if (legendaryEffectPrefab != null)
        {
            legendaryEffectPrefab.SetActive(false); // Ukrycie prefabu na początku
        }

        if (CardPackManager.instance != null)
        {
            CardPackManager.instance.OnLegendaryCardDrawn += HandleLegendaryCardDrawn;
        }
        else
        {
            Debug.LogError("CardPackManager instance is null.");
        }
    }

    void OnDestroy()
    {
        if (CardPackManager.instance != null)
        {
            CardPackManager.instance.OnLegendaryCardDrawn -= HandleLegendaryCardDrawn;
        }
    }

    public void SetupCard()
    {
        if (cardSO == null)
        {
            Debug.LogWarning("Card ScriptableObject is null in SetupCard.");
            return;
        }

        healthText.text = cardSO.currentHealth.ToString();
        attackText.text = cardSO.attackPower.ToString();
        costText.text = cardSO.manaCost.ToString();
        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;
        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;

        isInDeck = GameManager.instance.IsCardInDeck(cardSO.uniqueID);

        if (addButton != null)
        {
            addButton.interactable = !isInDeck;
        }

        if (removeButton != null)
        {
            removeButton.interactable = isInDeck;
        }

        if (collectionButton != null)
        {
            collectionButton.interactable = !isInDeck;
        }
    }

    public void AddToDeck()
    {
        if (cardSO != null && !isInDeck)
        {
            GameManager.instance.AddCardToDeck(cardSO.uniqueID);

            if (!GameManager.instance.allCards.Exists(c => c.uniqueID == cardSO.uniqueID))
            {
                GameManager.instance.AddCardToAllCards(cardSO);
            }

            Debug.Log("Dodano kartę do talii: " + cardSO.cardName);

            isInDeck = GameManager.instance.IsCardInDeck(cardSO.uniqueID);

            if (addButton != null)
            {
                addButton.interactable = !isInDeck;
            }

            if (removeButton != null)
            {
                removeButton.interactable = isInDeck;
            }
        }
        else
        {
            Debug.LogWarning("Card is already in deck or Card ScriptableObject is not assigned.");
        }
    }

    public void RemoveFromDeck()
    {
        if (cardSO != null && isInDeck)
        {
            GameManager.instance.RemoveCardFromDeck(cardSO.uniqueID);
            Debug.Log("Usunięto kartę z talii: " + cardSO.cardName);

            isInDeck = GameManager.instance.IsCardInDeck(cardSO.uniqueID);

            if (addButton != null)
            {
                addButton.interactable = !isInDeck;
            }

            if (removeButton != null)
            {
                removeButton.interactable = isInDeck;
            }
        }
        else
        {
            Debug.LogWarning("Card is not in deck or Card ScriptableObject is not assigned.");
        }
    }

    public void AddToCollections()
    {
        if (cardSO != null && !isInDeck)
        {
            GameManager.instance.AddCardToCollection(cardSO.uniqueID);

            if (!GameManager.instance.allCards.Exists(c => c.uniqueID == cardSO.uniqueID))
            {
                GameManager.instance.AddCardToAllCards(cardSO);
            }

            Debug.Log("Dodano kartę do kolekcji: " + cardSO.cardName);

            isInDeck = GameManager.instance.IsCardInCollection(cardSO.uniqueID);

            if (collectionButton != null)
            {
                collectionButton.interactable = !isInDeck;
            }

            if (removeButton != null)
            {
                removeButton.interactable = isInDeck;
            }
        }
        else
        {
            Debug.LogWarning("Card is already in deck or Card ScriptableObject is not assigned.");
        }
    }

    private void HandleLegendaryCardDrawn(CardScriptableObject legendaryCard)
    {
        Debug.Log("HandleLegendaryCardDrawn called for card: " + legendaryCard.cardName);
        if (legendaryCard == cardSO)
        {
            Debug.Log("Instantiating legendary effect for card: " + legendaryCard.cardName);
            if (legendaryEffectPrefab != null)
            {
                legendaryEffectPrefab.SetActive(true); // Pokazanie prefabu
                Debug.Log("Legendary effect instantiated for card: " + legendaryCard.cardName);
            }
            else
            {
                Debug.LogError("Legendary effect prefab is not assigned.");
            }
        }
    }
}
