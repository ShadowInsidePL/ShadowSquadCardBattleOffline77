using UnityEngine;
using System.Collections.Generic;

public class AllCardsDisplay : MonoBehaviour
{
    public GameObject cardDisplayPrefab; // Prefab karty do wyświetlania
    public Transform contentParent; // Kontener na karty
    public int columns = 5; // Liczba kolumn
    public Vector2 cardSize = new Vector2(1.0f, 1.5f); // Rozmiar karty w jednostkach świata
    public Vector2 spacing = new Vector2(0.2f, 0.2f); // Odstępy między kartami
    public Vector2 startPos = new Vector2(-4.0f, 3.0f); // Początkowa pozycja pierwszej karty

    private List<CardScriptableObject> allCards; // Lista wszystkich kart

    void Start()
    {
        LoadCards();
        DisplayCards();
    }

    void LoadCards()
    {
        allCards = GameManager.instance.allCards;
    }

    void DisplayCards()
    {
        int row = 0;
        int col = 0;

        foreach (CardScriptableObject card in allCards)
        {
            GameObject cardObject = Instantiate(cardDisplayPrefab, contentParent);

            // Obliczanie pozycji karty z uwzględnieniem skali
            float xPosition = startPos.x + col * (cardSize.x + spacing.x) * 100;
            float yPosition = startPos.y - row * (cardSize.y + spacing.y) * 100;
            cardObject.transform.localPosition = new Vector3(xPosition, yPosition, 0);

            // Aktualizacja kolumny i rzędu
            col++;
            if (col >= columns)
            {
                col = 0;
                row++;
            }

            RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
            cardRectTransform.localScale = Vector3.one * 100; // Ustawienie skali na 100

            CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
            cardDisplay.cardSO = card;
            cardDisplay.SetupCard();
        }
    }
}
