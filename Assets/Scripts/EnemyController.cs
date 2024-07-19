using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;
    public Transform cardSpawnPoint;

    public enum AIType { placeFromDeck, handRandomPlace, handDefensive, handAttacking }
    public AIType enemyAIType;

    private List<CardScriptableObject> cardsInHand = new List<CardScriptableObject>();
    public int startHandSize;

    void Start()
    {
        SetupDeck();
        if (enemyAIType != AIType.placeFromDeck)
        {
            SetupHand();
        }
    }

    void Update()
    {
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
    }

    public void StartAction()
    {
        StartCoroutine(EnemyActionCo());
    }

    IEnumerator EnemyActionCo()
    {
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }
        yield return new WaitForSeconds(.5f);

        if (enemyAIType != AIType.placeFromDeck)
        {
            for (int i = 0; i < BattleController.instance.cardsToDrawPerTurn; i++)
            {
                cardsInHand.Add(activeCards[0]);
                activeCards.RemoveAt(0);

                if (activeCards.Count == 0)
                {
                    SetupDeck();
                }
            }
        }

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(CardPointController.instance.enemyCardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        if (enemyAIType == AIType.placeFromDeck || enemyAIType == AIType.handRandomPlace)
        {
            cardPoints.Remove(selectedPoint);

            while (selectedPoint.activeCard != null && cardPoints.Count > 0)
            {
                randomPoint = Random.Range(0, cardPoints.Count);
                selectedPoint = cardPoints[randomPoint];
                cardPoints.RemoveAt(randomPoint);
            }
        }

        CardScriptableObject selectedCard = null;
        int iterations = 0;
        List<CardPlacePoint> preferredPoints = new List<CardPlacePoint>();
        List<CardPlacePoint> secondaryPoints = new List<CardPlacePoint>();

        switch (enemyAIType)
        {
            case AIType.placeFromDeck:
                if (selectedPoint.activeCard == null)
                {
                    PlayEnemyCard(activeCards[0], selectedPoint);
                    activeCards.RemoveAt(0);
                }
                break;

            case AIType.handRandomPlace:
                selectedCard = selectedCardToPlay();
                iterations = 50;

                while (selectedCard != null && iterations > 0 && selectedPoint.activeCard == null)
                {
                    PlayEnemyCard(selectedCard, selectedPoint);

                    selectedCard = selectedCardToPlay();

                    iterations--;
                    yield return new WaitForSeconds(CardPointController.instance.timeBetweenAttacks);
                    while (selectedPoint.activeCard != null && cardPoints.Count > 0)
                    {
                        randomPoint = Random.Range(0, cardPoints.Count);
                        selectedPoint = cardPoints[randomPoint];
                        cardPoints.RemoveAt(randomPoint);
                    }
                }
                break;

            case AIType.handDefensive:
                selectedCard = selectedCardToPlay();

                preferredPoints.Clear();
                secondaryPoints.Clear();
                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointController.instance.playerCardPoints[i].activeCard != null)
                        {
                            preferredPoints.Add(cardPoints[i]);
                        }
                        else
                        {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {
                    if (preferredPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoints.Count);
                        selectedPoint = preferredPoints[selectPoint];
                        preferredPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];
                        secondaryPoints.RemoveAt(selectPoint);
                    }
                    PlayEnemyCard(selectedCard, selectedPoint);

                    selectedCard = selectedCardToPlay();
                    iterations--;
                    yield return new WaitForSeconds(CardPointController.instance.timeBetweenAttacks);
                }
                break;

            case AIType.handAttacking:
                selectedCard = selectedCardToPlay();

                preferredPoints.Clear();
                secondaryPoints.Clear();
                for (int i = 0; i < cardPoints.Count; i++)
                {
                    if (cardPoints[i].activeCard == null)
                    {
                        if (CardPointController.instance.playerCardPoints[i].activeCard == null)
                        {
                            preferredPoints.Add(cardPoints[i]);
                        }
                        else
                        {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }

                iterations = 50;
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {
                    if (preferredPoints.Count > 0)
                    {
                        int selectPoint = Random.Range(0, preferredPoints.Count);
                        selectedPoint = preferredPoints[selectPoint];
                        preferredPoints.RemoveAt(selectPoint);
                    }
                    else
                    {
                        int selectPoint = Random.Range(0, secondaryPoints.Count);
                        selectedPoint = secondaryPoints[selectPoint];
                        secondaryPoints.RemoveAt(selectPoint);
                    }
                    PlayEnemyCard(selectedCard, selectedPoint);

                    selectedCard = selectedCardToPlay();
                    iterations--;
                    yield return new WaitForSeconds(CardPointController.instance.timeBetweenAttacks);
                }
                break;
        }

        yield return new WaitForSeconds(.5f);

        BattleController.instance.AdvanceTurn();
    }

    void SetupHand()
    {
        for (int i = 0; i < startHandSize; i++)
        {
            if (activeCards.Count == 0)
            {
                SetupDeck();
            }
            cardsInHand.Add(activeCards[0]);
            activeCards.RemoveAt(0);
        }
    }

    public void PlayEnemyCard(CardScriptableObject cardSO, CardPlacePoint placePoint)
    {
        Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
        newCard.cardSO = cardSO;
        newCard.isPlayer = false;  // Upewnij się, że karta jest przeciwnika
        newCard.SetupCard();
        newCard.MoveToPoint(placePoint.transform.position, placePoint.transform.rotation);

        placePoint.activeCard = newCard;
        newCard.assignedPlace = placePoint;

        cardsInHand.Remove(cardSO);

        BattleController.instance.SpendEnemyMana(cardSO.manaCost);
        AudioManager.instance.PlaySFX(4);

        // Aktywacja umiejętności karty przeciwnika po umieszczeniu jej na polu bitwy
        if (newCard.hasAddHealthAbility)
        {
            Debug.Log("Activating AddEnemyMaxHealth for " + cardSO.cardName);
            BattleController.instance.AddEnemyMaxHealth(newCard.healAmount);
            newCard.hasAddHealthAbility = false; // Umiejętność jednorazowa
        }
        if (newCard.hasAddManaAbility)
        {
            BattleController.instance.AddEnemyMaxMana(newCard.manaAmount);
            newCard.hasAddManaAbility = false; // Umiejętność jednorazowa
        }
        if (newCard.hasIncreaseAttackAbility)
        {
            bool isPlayer = false; // Przykład: Karta należy do przeciwnika
            int increaseAmount = newCard.attackIncreaseAmount; // Przykład: Zwiększenie ataku karty
            bool isPlacedOnArena = (placePoint.activeCard != null); // Przykład warunku, kiedy karta jest na arenie

            BattleController.instance.IncreaseAttackPower(isPlayer, increaseAmount, isPlacedOnArena);

            newCard.hasIncreaseAttackAbility = false; // Umiejętność jednorazowa
        }
        if (newCard.hasStunAbility)
        {
            Debug.Log("Activating Stun for " + cardSO.cardName);
            BattleController.instance.TryStunOpponentCard(newCard);
            newCard.hasStunAbility = false; // Umiejętność jednorazowa
        }
        if (newCard.hasInvincibilityAbility)
        {
            Debug.Log("Activating Invincibility for " + cardSO.cardName);
            newCard.ActivateInvincibility(newCard.invincibilityTurns);
            newCard.hasInvincibilityAbility = false; // Umiejętność jednorazowa
        }
    }

    CardScriptableObject selectedCardToPlay()
    {
        CardScriptableObject cardToPlay = null;

        List<CardScriptableObject> cardsToPlay = new List<CardScriptableObject>();
        foreach (CardScriptableObject card in cardsInHand)
        {
            if (card.manaCost <= BattleController.instance.enemyMana)
            {
                cardsToPlay.Add(card);
            }
        }
        if (cardsToPlay.Count > 0)
        {
            int selected = Random.Range(0, cardsToPlay.Count);

            cardToPlay = cardsToPlay[selected];
        }

        return cardToPlay;
    }

    public void DrawMultipleEnemyCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (activeCards.Count == 0)
            {
                SetupDeck();
            }
            cardsInHand.Add(activeCards[0]);
            activeCards.RemoveAt(0);
        }
    }
}
