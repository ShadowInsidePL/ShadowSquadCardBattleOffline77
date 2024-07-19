using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPointController : MonoBehaviour
{
    public static CardPointController instance;

    

    private void Awake()
    {
        instance = this;
    }

    public CardPlacePoint[] playerCardPoints, enemyCardPoints;
    public float timeBetweenAttacks = .25f;
    
    void Start()
    {
        
    }

  
    void Update()
    {
        
    }

    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCo());
    }
     public void EnemyAttack()
    {
        StartCoroutine(EnemyAttackCo());
    }
    
    
 IEnumerator PlayerAttackCo()
{
    yield return new WaitForSeconds(timeBetweenAttacks);

    for (int i = 0; i < playerCardPoints.Length; i++)
    {
        if (playerCardPoints[i].activeCard != null)
        {
            if (playerCardPoints[i].activeCard.isStunned)
            {
                Debug.Log($"{playerCardPoints[i].activeCard.cardSO.cardName} is stunned and cannot attack.");
            }
            else
            {
                if (enemyCardPoints[i].activeCard != null)
                {
                    // attack the enemy card
                    enemyCardPoints[i].activeCard.DamageCard(playerCardPoints[i].activeCard.attackPower);
                }
                else
                {
                    BattleController.instance.DamageEnemy(playerCardPoints[i].activeCard.attackPower);
                    // Attack the enemy overall health
                }
                playerCardPoints[i].activeCard.anim.SetTrigger("Attack");
            }

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
        if (BattleController.instance.battleEnded == true)
        {
            i = playerCardPoints.Length;
        }
    }

    CheckAssignedCards();
    BattleController.instance.AdvanceTurn();
}

IEnumerator EnemyAttackCo()
{
    yield return new WaitForSeconds(timeBetweenAttacks);

    for (int i = 0; i < enemyCardPoints.Length; i++)
    {
        if (enemyCardPoints[i].activeCard != null)
        {
            if (enemyCardPoints[i].activeCard.isStunned)
            {
                Debug.Log($"{enemyCardPoints[i].activeCard.cardSO.cardName} is stunned and cannot attack.");
            }
            else
            {
                if (playerCardPoints[i].activeCard != null)
                {
                    // attack the player card
                    playerCardPoints[i].activeCard.DamageCard(enemyCardPoints[i].activeCard.attackPower);
                }
                else
                {
                    // Attack the players overall health
                    BattleController.instance.DamagePlayer(enemyCardPoints[i].activeCard.attackPower);
                }
                enemyCardPoints[i].activeCard.anim.SetTrigger("Attack");
            }

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
        if (BattleController.instance.battleEnded == true)
        {
            i = enemyCardPoints.Length;
        }
    }

    CheckAssignedCards();
    BattleController.instance.AdvanceTurn();
}



    public void CheckAssignedCards()
    {
        foreach(CardPlacePoint point in enemyCardPoints)
        {
            if(point.activeCard != null)
            {
                if(point.activeCard.currentHealth <= 0)
                {
                    point.activeCard = null;
                }
            }
        }

        foreach(CardPlacePoint point in playerCardPoints)
        {
            if(point.activeCard != null)
            {
                if(point.activeCard.currentHealth <= 0)
                {
                    point.activeCard = null;
                }
            }
    }
}
}

