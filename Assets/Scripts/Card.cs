using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public bool isPlayer;

    public int currentHealth;
    public int attackPower;
    public int manaCost;

    public TMP_Text healtText, attackText, costText, nameText, actionDescriptionText, loreText;

    public Image characterArt, bgArt, stunImage; // Dodanie stunImage
    public Sprite stunSprite; // Dodanie sprite dla ogłuszenia

    private Vector3 targetPoint;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    private Quaternion targetRot;

    public bool inHand;
    public int handPosition;
    private HandController theHC;

    private bool isSelected;

    private Collider theCol;

    public LayerMask whatIsDesktop, whatIsPlacement;

    private bool justPresed;
    public CardPlacePoint assignedPlace;

    public Animator anim;

    public bool hasAutoHealAbility;
    public int autoHealPower;
    public bool hasAddHealthAbility;
    public int healAmount;
    public bool hasAddManaAbility;
    public int manaAmount;

    public bool hasDrawCardsAbility;
    public int cardsToDraw;

    public bool hasIncreaseAttackAbility;
    public int attackIncreaseAmount;

    public bool hasStunAbility;
    public int stunTurns;

    public bool hasInvincibilityAbility;
    public int invincibilityTurns;

    public bool isStunned;
    public int remainingStunTurns;

    public bool isInvincible;
    public int remainingInvincibilityTurns;

    // Dodaj zmienną do prefabrykatu systemu particle
    public GameObject damageParticlePrefab;
    public GameObject healingParticlePrefab;
    public GameObject attackIncreaseParticlePrefab;

    void Start()
    {
        if (targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }
        SetupCard();

        theHC = FindObjectOfType<HandController>();
        theCol = GetComponent<Collider>();

        // Ukryj stunImage na początku
        if (stunImage != null)
        {
            stunImage.enabled = false;
        }
    }

    public void SetupCard()
    {
        currentHealth = cardSO.currentHealth;
        attackPower = cardSO.attackPower;
        manaCost = cardSO.manaCost;

        hasAutoHealAbility = cardSO.hasAutoHealAbility;
        autoHealPower = cardSO.autoHealPower;
        hasAddHealthAbility = cardSO.hasAddHealthAbility;
        healAmount = cardSO.healAmount;
        hasAddManaAbility = cardSO.hasAddManaAbility;
        manaAmount = cardSO.manaAmount;

        hasDrawCardsAbility = cardSO.hasDrawCardsAbility;
        cardsToDraw = cardSO.cardsToDraw;

        hasIncreaseAttackAbility = cardSO.hasIncreaseAttackAbility;
        attackIncreaseAmount = cardSO.attackIncreaseAmount;

        hasStunAbility = cardSO.hasStunAbility;
        stunTurns = cardSO.stunTurns;

        hasInvincibilityAbility = cardSO.hasInvincibilityAbility;
        invincibilityTurns = cardSO.invincibilityTurns;

        UpdateCardDisplay();

        nameText.text = cardSO.cardName;
        actionDescriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;

        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;
    }

    public void UpdateStunStatus()
    {
        if (isStunned)
        {
            remainingStunTurns--;
            if (remainingStunTurns <= 0)
            {
                isStunned = false;
                Debug.Log($"{cardSO.cardName} nie jest oszołomiony.");
                // Ukryj stunImage po zakończeniu stunu
                if (stunImage != null)
                {
                    stunImage.enabled = false;
                }
            }
        }
    }

    public void UpdateInvincibilityStatus()
    {
        if (isInvincible)
        {
            remainingInvincibilityTurns--;
            if (remainingInvincibilityTurns <= 0)
            {
                isInvincible = false;
                Debug.Log($"{cardSO.cardName} is no longer invincible.");
                // Przywróć normalną przejrzystość po zakończeniu niewidzialności
                SetCardTransparency(1f);
            }
        }
    }

    public void ActivateInvincibility(int turns)
    {
        isInvincible = true;
        remainingInvincibilityTurns = turns;
        Debug.Log($"{cardSO.cardName} is invincible for {turns} turns.");
        // Ustaw zmniejszoną przejrzystość podczas niewidzialności
        SetCardTransparency(0.5f);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        if (isSelected && BattleController.instance.battleEnded == false && Time.timeScale != 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, whatIsDesktop) && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
            {
                MoveToPoint(hit.point + new Vector3(0f, 2f, 0f), Quaternion.identity);
            }

            if (Input.GetMouseButtonDown(1) && BattleController.instance.battleEnded == false)
            {
                ReturnToHand();
            }
            if (Input.GetMouseButtonDown(0) && justPresed == false && BattleController.instance.battleEnded == false)
            {
                if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement))
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                    if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                    {
                        if (BattleController.instance.playerMana >= manaCost)
                        {
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;

                            MoveToPoint(selectedPoint.transform.position, Quaternion.identity);

                            inHand = false;

                            isSelected = false;

                            theHC.RemoveCardFromHand(this);

                            BattleController.instance.SpendPlayerMana(manaCost);

                            AudioManager.instance.PlaySFX(4);

                            // Aktywacja umiejętności karty po umieszczeniu jej na polu bitwy
                            if (hasAddHealthAbility)
                            {
                                Debug.Log("Activating AddPlayerMaxHealth for " + cardSO.cardName);
                                BattleController.instance.AddPlayerMaxHealth(healAmount);
                                hasAddHealthAbility = false; // Umiejętność jednorazowa
                            }
                            if (hasAddManaAbility)
                            {
                                BattleController.instance.AddPlayerMaxMana(manaAmount);
                                hasAddManaAbility = false; // Umiejętność jednorazowa
                            }
                            if (hasDrawCardsAbility)
                            {
                                Debug.Log("Activating DrawCards for " + cardSO.cardName);
                                BattleController.instance.DrawCards(true, cardsToDraw);
                                hasDrawCardsAbility = false; // Umiejętność jednorazowa
                            }
                            if (hasIncreaseAttackAbility)
                            {
                                Debug.Log("Activating IncreaseAttack for " + cardSO.cardName);
                                BattleController.instance.IncreaseAttackPower(true, attackIncreaseAmount, true);
                                hasIncreaseAttackAbility = false; // Umiejętność jednorazowa
                            }
                            if (hasStunAbility)
                            {
                                Debug.Log("Activating Stun for " + cardSO.cardName);
                                BattleController.instance.TryStunOpponentCard(this);
                                hasStunAbility = false; // Umiejętność jednorazowa
                            }
                            if (hasInvincibilityAbility)
                            {
                                Debug.Log("Activating Invincibility for " + cardSO.cardName);
                                ActivateInvincibility(invincibilityTurns);
                                hasInvincibilityAbility = false; // Umiejętność jednorazowa
                            }
                        }
                        else
                        {
                            ReturnToHand();

                            UIController.instance.ShowManaWarning();
                        }
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }
                else
                {
                    ReturnToHand();
                }
            }
        }

        justPresed = false;
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;
    }

    private void OnMouseOver()
    {
        if (inHand && isPlayer && BattleController.instance.battleEnded == false)
        {
            MoveToPoint(theHC.cardPositions[handPosition] + new Vector3(0f, 1f, 0.5f), Quaternion.identity);
        }
    }

    private void OnMouseExit()
    {
        if (inHand && isPlayer && BattleController.instance.battleEnded == false)
        {
            MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
        }
    }

    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer && BattleController.instance.battleEnded == false && Time.timeScale != 0f)
        {
            isSelected = true;
            theCol.enabled = false;

            justPresed = true;
        }
    }

    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;
        MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
    }

    public void DamageCard(int damageAmount)
    {
        if (isInvincible)
        {
            Debug.Log($"{cardSO.cardName} is invincible and takes no damage.");
            return;
        }

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            assignedPlace.activeCard = null;

            MoveToPoint(BattleController.instance.discardPoint.position, BattleController.instance.discardPoint.rotation);

            anim.SetTrigger("Jump");

            Destroy(gameObject, 5f);
            AudioManager.instance.PlaySFX(2);
        }
        else
        {
            AudioManager.instance.PlaySFX(1);
        }

        anim.SetTrigger("Hurt");

        // Instancja systemu particle w miejscu karty
        if (damageParticlePrefab != null)
        {
            Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
            Debug.Log("Particle system instantiated.");
        }
        else
        {
            Debug.LogWarning("Damage particle prefab is not assigned.");
        }

        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        healtText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
        costText.text = manaCost.ToString();
    }

    public void ClampHealth()
    {
        if (currentHealth > cardSO.currentHealth)
        {
            currentHealth = cardSO.currentHealth;
        }
    }

    public void AutoHeal()
    {
        if (hasAutoHealAbility && !inHand && assignedPlace != null)
        {
            Card[] teamCards = FindObjectsOfType<Card>();
            foreach (Card card in teamCards)
            {
                if (card.isPlayer == this.isPlayer && card != this && card.currentHealth > 0 && !card.inHand && card.assignedPlace != null)
                {
                    card.currentHealth += autoHealPower;

                    // Upewniamy się, że currentHealth nie przekracza maksymalnej wartości HP karty
                    card.ClampHealth();

                    card.UpdateCardDisplay();
                    Debug.Log($"{cardSO.cardName} healed {card.cardSO.cardName} for {autoHealPower} health. New health: {card.currentHealth}");

                    // Instancja systemu cząsteczek w miejscu leczonej karty
                    if (healingParticlePrefab != null)
                    {
                        GameObject particleInstance = Instantiate(healingParticlePrefab, card.transform.position, Quaternion.identity);
                        Debug.Log("Healing particle effect instantiated.");
                        Destroy(particleInstance, 2f); // Zniszcz instancję systemu cząsteczek po 2 sekundach
                    }
                    else
                    {
                        Debug.LogWarning("Healing particle prefab is not assigned.");
                    }
                }
            }
        }
        else
        {
            Debug.Log($"{cardSO.cardName} cannot heal. Conditions not met. inHand: {inHand}, assignedPlace: {assignedPlace}");
        }
    }

    public void IncreaseAttackPower(int increaseAmount)
    {
        attackPower += increaseAmount;
        UpdateCardDisplay();

        // Instancja systemu particle w miejscu karty
        if (attackIncreaseParticlePrefab != null)
        {
            GameObject particleInstance = Instantiate(attackIncreaseParticlePrefab, transform.position, Quaternion.identity);
            Debug.Log("Attack increase particle effect instantiated.");
            Destroy(particleInstance, 2f); // Zniszcz instancję systemu cząsteczek po 2 sekundach
        }
        else
        {
            Debug.LogWarning("Attack increase particle prefab is not assigned.");
        }
    }

    public CardPlacePoint GetOppositePoint()
    {
        if (assignedPlace == null) return null;

        CardPlacePoint[] pointsArray = isPlayer ? CardPointController.instance.enemyCardPoints : CardPointController.instance.playerCardPoints;
        int index = System.Array.IndexOf(isPlayer ? CardPointController.instance.playerCardPoints : CardPointController.instance.enemyCardPoints, assignedPlace);

        if (index >= 0 && index < pointsArray.Length)
        {
            return pointsArray[index];
        }

        return null;
    }

    public void StunCard(int turns)
    {
        isStunned = true;
        remainingStunTurns = turns;
        Debug.Log($"{cardSO.cardName} została ogłuszona na {turns} tury.");

        // Pokaż stunImage podczas ogłuszenia
        if (stunImage != null && stunSprite != null)
        {
            stunImage.sprite = stunSprite;
            stunImage.enabled = true;
        }
    }

    // Metoda pomocnicza do ustawienia przejrzystości karty
    private void SetCardTransparency(float alpha)
    {
        Color color = characterArt.color;
        color.a = alpha;
        characterArt.color = color;

        color = bgArt.color;
        color.a = alpha;
        bgArt.color = color;

        // Ustawienie przejrzystości tekstu
        healtText.alpha = alpha;
        attackText.alpha = alpha;
        costText.alpha = alpha;
        nameText.alpha = alpha;
        actionDescriptionText.alpha = alpha;
        loreText.alpha = alpha;
    }
}
