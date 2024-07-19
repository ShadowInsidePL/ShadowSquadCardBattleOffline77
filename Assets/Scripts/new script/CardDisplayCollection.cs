using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplayCollection : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public TMP_Text healthText, attackText, costText, nameText, actionDescriptionText, loreText;
    public Image characterArt, bgArt;
    public Button addButton, removeButton;

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

        if (addButton != null)
        {
            addButton.onClick.AddListener(AddToDeck);
        }

        if (removeButton != null)
        {
            removeButton.onClick.AddListener(RemoveFromDeck);
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
}

        
 