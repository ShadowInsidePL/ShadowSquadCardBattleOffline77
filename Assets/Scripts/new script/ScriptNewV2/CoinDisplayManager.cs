using TMPro;
using UnityEngine;

public class CoinDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI coinText; // Referencja do TextMeshPro
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
        UpdateCoinText(gameManager.playerCoins);
        gameManager.OnCoinsChanged += UpdateCoinText;
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnCoinsChanged -= UpdateCoinText;
        }
    }

    private void UpdateCoinText(int coins)
    {
        coinText.text = "Coins: " + coins;
    }
}
