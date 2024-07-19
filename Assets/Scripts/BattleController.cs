using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;
    public int winCoinsReward; // Liczba monet za wygraną
    public string currentLevelName; // Nazwa aktualnego poziomu

    private void Awake()
    {
        instance = this;
    }

    public int startingMana = 4, maxMana = 12;
    public int playerMana, enemyMana;
    public int playerHealth, enemyHealth;
    private int currentPlayerMaxMana, currentEnemyMaxMana;
    private int playerMaxHealth = 20; // Maksymalne zdrowie gracza
    private int enemyMaxHealth = 20; // Maksymalne zdrowie przeciwnika

    public enum TurnOrder { playerActive, playerCardAttacks, enemyActive, enemyCardAttacks }
    public TurnOrder currentPhase;
    public Transform discardPoint;

    public int startingCardsAmount = 5;
    public int cardsToDrawPerTurn = 2;

    public bool battleEnded = true;

    public float resultScreenDelayTime = 1f;
    [Range(0f, 1f)]
    public float playerFirstChance = .5f;

   void Start()
{
    Debug.Log("BattleController Start method called.");
    currentPlayerMaxMana = startingMana;
    FillPlayerMana();

    playerHealth = playerMaxHealth; // Inicjalizacja zdrowia gracza
    enemyHealth = enemyMaxHealth; // Inicjalizacja zdrowia przeciwnika

    Debug.Log("Drawing initial cards.");
    DeckController.instance.DrawMultipleCards(startingCardsAmount);
    UIController.instance.SetPlayerHealthText(playerHealth);
    UIController.instance.SetEnemyHealthText(enemyHealth);

    currentEnemyMaxMana = startingMana;
    FillEnemyMana();

    if (Random.value >= playerFirstChance)
    {
        currentPhase = TurnOrder.playerCardAttacks;
        AdvanceTurn();
    }

    AudioManager.instance.PlayBGM();
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTurn();
        }

        // Debugowanie wartości zdrowia w czasie rzeczywistym
        Debug.Log("Player Health: " + playerHealth + " / " + playerMaxHealth);
        Debug.Log("Enemy Health: " + enemyHealth + " / " + enemyMaxHealth);
    }

    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana -= amountToSpend;
        if (playerMana < 0)
        {
            playerMana = 0;
        }
        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void FillPlayerMana()
    {
        playerMana = currentPlayerMaxMana;
        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void SpendEnemyMana(int amountToSpend)
    {
        enemyMana -= amountToSpend;
        if (enemyMana < 0)
        {
            enemyMana = 0;
        }
        UIController.instance.SetEnemyManaText(enemyMana);
    }

    public void FillEnemyMana()
    {
        enemyMana = currentEnemyMaxMana;
        UIController.instance.SetEnemyManaText(enemyMana);
    }

    public void AddPlayerMaxHealth(int amount)
    {
        playerHealth += amount;
        ClampHealth(true);
        Debug.Log("Player Max Health: " + playerMaxHealth + " | Player Current Health: " + playerHealth);
        UIController.instance.SetPlayerHealthText(playerHealth);
    }

    public void AddEnemyMaxHealth(int amount)
    {
        enemyHealth += amount;
        ClampHealth(false);
        Debug.Log("Enemy Max Health: " + enemyMaxHealth + " | Enemy Current Health: " + enemyHealth);
        UIController.instance.SetEnemyHealthText(enemyHealth);
    }

    public void AddPlayerMaxMana(int amount)
    {
        currentPlayerMaxMana += amount;
        if (currentPlayerMaxMana > maxMana) currentPlayerMaxMana = maxMana;
        FillPlayerMana();
        Debug.Log("Player Max Mana: " + currentPlayerMaxMana + " | Player Current Mana: " + playerMana);
    }

    public void AddEnemyMaxMana(int amount)
    {
        currentEnemyMaxMana += amount;
        if (currentEnemyMaxMana > maxMana) currentEnemyMaxMana = maxMana;
        FillEnemyMana();
        Debug.Log("Enemy Max Mana: " + currentEnemyMaxMana + " | Enemy Current Mana: " + enemyMana);
    }

    public void AdvanceTurn()
    {
        if (!battleEnded)
        {
            currentPhase++;

            if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
            {
                currentPhase = 0;
            }

            StartTurn();

            switch (currentPhase)
            {
                case TurnOrder.playerActive:
                    UIController.instance.endTurnButton.SetActive(true);
                    UIController.instance.drawCardButton.SetActive(true);

                    if (currentPlayerMaxMana < maxMana)
                    {
                        currentPlayerMaxMana++;
                    }

                    FillPlayerMana();
                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);
                    break;

                case TurnOrder.playerCardAttacks:
                    Debug.Log("Skipping player card attacks");
                    CardPointController.instance.PlayerAttack();
                    break;

                case TurnOrder.enemyActive:
                    if (currentEnemyMaxMana < maxMana)
                    {
                        currentEnemyMaxMana++;
                    }
                    FillEnemyMana();
                    EnemyController.instance.StartAction();
                    break;

                case TurnOrder.enemyCardAttacks:
                    CardPointController.instance.EnemyAttack();
                    break;
            }
        }
    }

    private void StartTurn()
    {
        Debug.Log("StartTurn called. Current phase: " + currentPhase);

        UpdateInvincibilityStatus(); // Dodaj to wywołanie

        if (currentPhase == TurnOrder.playerActive)
        {
            AutoHealPlayerCards();
            Debug.Log("Player cards are healing.");
        }
        else if (currentPhase == TurnOrder.enemyActive)
        {
            AutoHealEnemyCards();
            Debug.Log("Enemy cards are healing.");
        }
    }

    private void UpdateInvincibilityStatus()
    {
        Card[] allCards = FindObjectsOfType<Card>();
        foreach (Card card in allCards)
        {
            card.UpdateInvincibilityStatus();
        }
    }

    private void AutoHealPlayerCards()
    {
        Debug.Log("AutoHealPlayerCards called.");
        Card[] playerCards = FindObjectsOfType<Card>();
        foreach (Card card in playerCards)
        {
            if (card.isPlayer && card.hasAutoHealAbility && !card.inHand && card.assignedPlace != null) // Sprawdzamy, czy karta nie jest w ręce i jest na arenie
            {
                Debug.Log("Auto-healing card: " + card.cardSO.cardName);
                card.AutoHeal();
            }
        }
    }

    private void AutoHealEnemyCards()
    {
        Debug.Log("AutoHealEnemyCards called.");
        Card[] enemyCards = FindObjectsOfType<Card>();
        foreach (Card card in enemyCards)
        {
            if (!card.isPlayer && card.hasAutoHealAbility && !card.inHand && card.assignedPlace != null) // Sprawdzamy, czy karta jest na arenie
            {
                Debug.Log("Auto-healing enemy card: " + card.cardSO.cardName);
                card.AutoHeal();
            }
            else
            {
                Debug.Log($"Card {card.cardSO.cardName} is not eligible for auto-healing. isPlayer: {card.isPlayer}, hasAutoHealAbility: {card.hasAutoHealAbility}, inHand: {card.inHand}, assignedPlace: {card.assignedPlace}");
            }
        }
    }

    public void EndPlayerTurn()
    {
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawCardButton.SetActive(false);

        AdvanceTurn();
    }

    public void DamagePlayer(int damageAmount)
    {
        if (playerHealth > 0 || !battleEnded)
        {
            playerHealth -= damageAmount;
            ClampHealth(true);
            Debug.Log("Player Health after damage: " + playerHealth);

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                EndBattle();
            }
            UIController.instance.SetPlayerHealthText(playerHealth);
            UIDamageIndicator damageClone = Instantiate(UIController.instance.playerDamage, UIController.instance.playerDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
            AudioManager.instance.PlaySFX(6);
        }
    }

    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0 || battleEnded == false)
        {
            enemyHealth -= damageAmount;
            ClampHealth(false);

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                EndBattle();
            }
            UIController.instance.SetEnemyHealthText(enemyHealth);

            UIDamageIndicator damageClone = Instantiate(UIController.instance.enemyDamage, UIController.instance.enemyDamage.transform.parent);
            damageClone.damageText.text = damageAmount.ToString();
            damageClone.gameObject.SetActive(true);
            AudioManager.instance.PlaySFX(5);
        }
    }

 void EndBattle()
{
    battleEnded = true;

    HandController.instance.EmptyHand();

    if (enemyHealth <= 0)
    {
        UIController.instance.battleResultText.text = "You won!";
        foreach (CardPlacePoint point in CardPointController.instance.enemyCardPoints)
        {
            if (point.activeCard != null)
            {
                point.activeCard.MoveToPoint(discardPoint.position, point.activeCard.transform.rotation);
            }
        }
       

        GameManager.instance.CompleteLevel(currentLevelName); // Ukończ poziom i zapisz jego stan
        GameManager.instance.AddCoins(winCoinsReward);
    }
    else
    {
        UIController.instance.battleResultText.text = "You lost!";
        foreach (CardPlacePoint point in CardPointController.instance.playerCardPoints)
        {
            if (point.activeCard != null)
            {
                point.activeCard.MoveToPoint(discardPoint.position, point.activeCard.transform.rotation);
            }
        }
    }

    StartCoroutine(ShowResultCo());
}

    IEnumerator ShowResultCo()
    {
        yield return new WaitForSeconds(resultScreenDelayTime);
        UIController.instance.battleEndScreen.SetActive(true);
    }

    // Metoda pomocnicza do ograniczenia zdrowia do maksymalnego zdrowia
    private void ClampHealth(bool isPlayer)
    {
        if (isPlayer)
        {
            if (playerHealth > playerMaxHealth)
            {
                playerHealth = playerMaxHealth;
                Debug.LogWarning("Player health exceeded max health. Clamping to max health.");
            }
        }
        else
        {
            if (enemyHealth > enemyMaxHealth)
            {
                enemyHealth = enemyMaxHealth;
                Debug.LogWarning("Enemy health exceeded max health. Clamping to max health.");
            }
        }
    }

    public void DrawCards(bool isPlayer, int amount)
    {
        Debug.Log("DrawCards called. isPlayer: " + isPlayer + ", amount: " + amount);
        if (isPlayer)
        {
            DeckController.instance.DrawMultipleCards(amount);
        }
        else
        {
            EnemyController.instance.DrawMultipleEnemyCards(amount);
        }
    }

    public void IncreaseAttackPower(bool isPlayer, int increaseAmount, bool isPlacedOnArena)
    {
        Debug.Log($"IncreaseAttackPower called. isPlayer: {isPlayer}, increaseAmount: {increaseAmount}");

        // Znajdź wszystkie karty w scenie
        Card[] allCards = FindObjectsOfType<Card>();

        // Zwiększ moc ataku kart spełniających warunki
        foreach (Card card in allCards)
        {
            if (card.isPlayer == isPlayer && card.currentHealth > 0 && card.assignedPlace != null)
            {
                card.IncreaseAttackPower(increaseAmount);
                Debug.Log($"{card.cardSO.cardName} attack increased by {increaseAmount}. New attack: {card.attackPower}");
            }
        }
    }

    public void TryStunOpponentCard(Card activeCard)
    {
        if (activeCard.assignedPlace != null)
        {
            CardPlacePoint opponentPoint = activeCard.GetOppositePoint();

            if (opponentPoint != null && opponentPoint.activeCard != null)
            {
                opponentPoint.activeCard.StunCard(activeCard.stunTurns);
                Debug.Log($"{opponentPoint.activeCard.cardSO.cardName} has been stunned for {activeCard.stunTurns} turns by {activeCard.cardSO.cardName}.");
            }
            else
            {
                Debug.Log($"{activeCard.cardSO.cardName} tried to stun but no opponent card found.");
            }
        }
    }
}
