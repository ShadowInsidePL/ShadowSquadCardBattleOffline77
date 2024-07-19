using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class BattleSelectButton : MonoBehaviour
{
    public string levelToLoad;
    public string requiredPreviousLevel; // Nazwa sceny poprzedniego poziomu, który trzeba ukończyć przed załadowaniem tej sceny
    public TextMeshProUGUI notificationText; // Tekst na ekranie

    void Start()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBattleSelectMusic();
        }
        else
        {
            Debug.LogError("AudioManager.instance nie jest przypisany!");
        }

        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false); // Tekst jest domyślnie ukryty na początku
        }
        else
        {
            Debug.LogError("notificationText nie jest przypisany w inspektorze!");
        }

        // Sprawdź, czy poziom jest odblokowany, jeśli nie, ukryj przycisk
        if (!IsLevelUnlocked())
        {
            gameObject.SetActive(false);
        }
    }

    public void SelectBattle()
    {
        Debug.Log("Attempting to select level: " + levelToLoad);

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance nie jest przypisany!");
            return;
        }

        // Sprawdź, czy wymagany poziom został ukończony
        bool levelCompleted = string.IsNullOrEmpty(requiredPreviousLevel) || GameManager.instance.IsLevelCompleted(requiredPreviousLevel);
        Debug.Log("Required level completed: " + levelCompleted);

        if (!levelCompleted)
        {
            Debug.Log("Musisz ukończyć poziom " + requiredPreviousLevel + ", zanim przejdziesz do poziomu " + levelToLoad + ".");
            ShowNotification("Musisz ukończyć poziom " + requiredPreviousLevel + ", zanim przejdziesz do poziomu " + levelToLoad + ".");
            return;
        }

        Debug.Log("Loading level: " + levelToLoad);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(0);
        }
        else
        {
            Debug.LogError("AudioManager.instance nie jest przypisany!");
        }

        SceneManager.LoadScene(levelToLoad);
    }

    // Funkcja do wyświetlania powiadomienia na ekranie
    private void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            StartCoroutine(HideNotificationAfterDelay(2.0f)); // Ukryj powiadomienie po 2 sekundach
        }
        else
        {
            Debug.LogError("notificationText nie jest przypisany w inspektorze!");
        }
    }

    // Coroutine do ukrywania powiadomienia po określonym czasie
    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    // Funkcja do sprawdzania, czy poziom jest odblokowany
    private bool IsLevelUnlocked()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance nie jest przypisany!");
            return false;
        }

        return string.IsNullOrEmpty(requiredPreviousLevel) || GameManager.instance.IsLevelCompleted(requiredPreviousLevel);
    }
}
