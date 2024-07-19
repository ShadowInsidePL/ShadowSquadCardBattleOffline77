using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardCollectionDisplay : MonoBehaviour
{
    public TMP_Text healthText, attackText, costText, nameText, actionDescriptionText, loreText;
    public Image characterArt, bgArt;
    public Button addButton, removeButton;

    private CardScriptableObject cardSO;
    private bool isInCollection = false;

    public CardScriptableObject CardSO
    {
        get { return cardSO; }
        set
        {
            cardSO = value;
            SetupCard();
        }
    }

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
            addButton.onClick.AddListener(AddToCollection);
        }
        else
        {
            Debug.LogWarning("Add button is not assigned.");
        }

        if (removeButton != null)
        {
            removeButton.onClick.AddListener(RemoveFromCollection);
        }
        else
        {
            Debug.LogWarning("Remove button is not assigned.");
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

        isInCollection = GameManager.instance.IsCardInCollection(cardSO.uniqueID);

        addButton.interactable = !isInCollection;
        removeButton.interactable = isInCollection;
    }

    public void AddToCollection()
    {
        if (cardSO != null && !isInCollection)
        {
            GameManager.instance.AddCardToCollection(cardSO.uniqueID);

            if (!GameManager.instance.allCards.Exists(c => c.uniqueID == cardSO.uniqueID))
            {
                GameManager.instance.AddCardToAllCards(cardSO);
            }

            Debug.Log("Dodano kartę do kolekcji: " + cardSO.cardName);

            isInCollection = GameManager.instance.IsCardInCollection(cardSO.uniqueID);

            addButton.interactable = !isInCollection;
            removeButton.interactable = isInCollection;
        }
        else
        {
            Debug.LogWarning("Card is already in collection or Card ScriptableObject is not assigned.");
        }
    }

    public void RemoveFromCollection()
    {
        if (cardSO != null && isInCollection)
        {
            GameManager.instance.RemoveCardFromCollection(cardSO.uniqueID);
            Debug.Log("Usunięto kartę z kolekcji: " + cardSO.cardName);

            isInCollection = GameManager.instance.IsCardInCollection(cardSO.uniqueID);

            addButton.interactable = !isInCollection;
            removeButton.interactable = isInCollection;
        }
        else
        {
            Debug.LogWarning("Card is not in collection or Card ScriptableObject is not assigned.");
        }
    }
}
