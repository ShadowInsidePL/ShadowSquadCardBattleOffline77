using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject cardDisplayPrefab; // Prefab dla wyświetlania kart
    public float cardSpacing = 200f;     // Odstęp między kartami
    public Button addToDeckButton;       // Przycisk do dodawania kart do talii

    private List<CardScriptableObject> currentDisplayedCards; // Lista aktualnie wyświetlanych kart
    private List<CardScriptableObject> selectedCards = new List<CardScriptableObject>(); // Lista wybranych kart

    private void Start()
    {
        // Dodaj listener do przycisku
        addToDeckButton.onClick.AddListener(AddSelectedCardsToDeck);
    }

    public void DisplayDrawnCards(List<CardScriptableObject> drawnCards, Transform parentTransform)
    {
        // Przechowaj aktualnie wyświetlane karty
        currentDisplayedCards = drawnCards;

        // Usuń istniejące karty
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }

        // Instancjuj i ustaw karty z odstępami
        for (int i = 0; i < drawnCards.Count; i++)
        {
            GameObject cardDisplay = Instantiate(cardDisplayPrefab, parentTransform);
            RectTransform cardRectTransform = cardDisplay.GetComponent<RectTransform>();
            cardRectTransform.anchoredPosition = new Vector2(i * cardSpacing, 0);
            CardDisplay cardDisplayComponent = cardDisplay.GetComponent<CardDisplay>();
            cardDisplayComponent.cardSO = drawnCards[i];
            cardDisplayComponent.SetupCard();

            Debug.Log("Wyświetlana karta: " + cardDisplayComponent.cardSO.cardName + " z ID: " + cardDisplayComponent.cardSO.uniqueID);

            // Dodaj możliwość zaznaczania kart
            Toggle toggle = cardDisplay.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener((isSelected) =>
                {
                    if (isSelected)
                    {
                        selectedCards.Add(cardDisplayComponent.cardSO);
                        Debug.Log("Zaznaczono kartę: " + cardDisplayComponent.cardSO.cardName + " z ID: " + cardDisplayComponent.cardSO.uniqueID);
                    }
                    else
                    {
                        selectedCards.Remove(cardDisplayComponent.cardSO);
                        Debug.Log("Odznaczono kartę: " + cardDisplayComponent.cardSO.cardName + " z ID: " + cardDisplayComponent.cardSO.uniqueID);
                    }
                });
            }
        }

        // Force the parent transform to update its layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform.GetComponent<RectTransform>());
    }

    public void DisplayCollectionCards(List<CardScriptableObject> collectionCards, Transform parentTransform)
    {
        // Przechowaj aktualnie wyświetlane karty
        currentDisplayedCards = collectionCards;

        // Usuń istniejące karty
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }

        // Instancjuj i ustaw karty z odstępami
        for (int i = 0; i < collectionCards.Count; i++)
        {
            GameObject cardDisplay = Instantiate(cardDisplayPrefab, parentTransform);
            RectTransform cardRectTransform = cardDisplay.GetComponent<RectTransform>();
            cardRectTransform.anchoredPosition = new Vector2(i * cardSpacing, 0);
            CardCollectionDisplay cardDisplayComponent = cardDisplay.GetComponent<CardCollectionDisplay>();
            cardDisplayComponent.CardSO = collectionCards[i]; // Użycie właściwości CardSO

            Debug.Log("Wyświetlana karta w kolekcji: " + cardDisplayComponent.CardSO.cardName + " z ID: " + cardDisplayComponent.CardSO.uniqueID);

            // Dodaj możliwość zaznaczania kart
            Toggle toggle = cardDisplay.GetComponentInChildren<Toggle>();
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener((isSelected) =>
                {
                    if (isSelected)
                    {
                        selectedCards.Add(cardDisplayComponent.CardSO);
                        Debug.Log("Zaznaczono kartę z kolekcji: " + cardDisplayComponent.CardSO.cardName + " z ID: " + cardDisplayComponent.CardSO.uniqueID);
                    }
                    else
                    {
                        selectedCards.Remove(cardDisplayComponent.CardSO);
                        Debug.Log("Odznaczono kartę z kolekcji: " + cardDisplayComponent.CardSO.cardName + " z ID: " + cardDisplayComponent.CardSO.uniqueID);
                    }
                });
            }
        }

        // Force the parent transform to update its layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentTransform.GetComponent<RectTransform>());
    }

    private void AddSelectedCardsToDeck()
    {
        foreach (var card in selectedCards)
        {
            GameManager.instance.AddCardToDeck(card.uniqueID);
            Debug.Log("Dodano kartę do talii: " + card.cardName + " z ID: " + card.uniqueID);
        }
        // Wyczyszczenie listy wybranych kart po dodaniu do talii
        selectedCards.Clear();
    }
}
