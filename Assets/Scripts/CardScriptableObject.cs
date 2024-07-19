using UnityEngine;

public enum CardRarity
{
    Normal,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardScriptableObject : ScriptableObject
{
    public string uniqueID;  // Unikalny identyfikator karty
    public string cardName;
    public int currentHealth;
    public int attackPower;
    public int manaCost;
    public string actionDescription;
    public string cardLore;
    public Sprite characterSprite;
    public Sprite bgSprite;
    public float dropRate;
    public CardRarity rarity;

    public bool hasHealingAbility; // Flaga, czy karta ma zdolność leczenia
    public int healingPower; // Ilość punktów zdrowia, którą karta może przywrócić
    public bool hasAutoHealAbility; // Flaga, czy karta ma zdolność automatycznego leczenia
    public int autoHealPower; // Ilość punktów zdrowia, którą karta automatycznie przywraca co turę
    public bool hasAddHealthAbility; // Flaga, czy karta może dodać zdrowie
    public int healAmount; // Ilość zdrowia, którą karta dodaje
    public bool hasAddManaAbility; // Flaga, czy karta może dodać manę
    public int manaAmount; // Ilość many, którą karta dodaje
    public bool hasDrawCardsAbility;
    public int cardsToDraw;
    public bool hasIncreaseAttackAbility;
    public int attackIncreaseAmount;
    public bool hasStunAbility; // Flaga, czy karta ma zdolność ogłuszania
    public int stunTurns; // Ilość tur trwania ogłuszenia
    public bool hasInvincibilityAbility; // Flaga, czy karta ma zdolność nietykalności
    public int invincibilityTurns; // Ilość tur trwania nietykalności

    // Konstruktor do tworzenia przykładowych kart
    public CardScriptableObject(string name, int health, int attack, int cost, string description, string lore)
    {
        uniqueID = System.Guid.NewGuid().ToString();  // Generowanie unikalnego identyfikatora
        cardName = name;
        currentHealth = health;
        attackPower = attack;
        manaCost = cost;
        actionDescription = description;
        cardLore = lore;
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();
        }
    }
}
