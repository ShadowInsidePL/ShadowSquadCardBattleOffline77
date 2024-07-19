using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public string levelToLoad;
    public string requiredPreviousLevel; // Nazwa sceny poprzedniego poziomu, który trzeba ukończyć przed załadowaniem tej sceny
    public TextMeshProUGUI notificationText; // Tekst na ekranie
    public bool requiresCardsToEnter = true; // Flaga wskazująca, czy wymagane są karty do wejścia

    void Start()
    {
        // Upewnij się, że AudioManager istnieje i odtwarza muzykę
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBattleSelectMusic();
        }
        else
        {
            Debug.LogError("AudioManager.instance nie jest przypisany!");
        }

        // Ukryj tekst powiadomienia na początku
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("notificationText nie jest przypisany w inspektorze!");
        }
    }

    public void SelectLevel()
    {
        Debug.Log("Próba wyboru poziomu: " + levelToLoad);

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance nie jest przypisany!");
            return;
        }

        // Sprawdź, czy wymagany poziom został ukończony
        bool levelCompleted = string.IsNullOrEmpty(requiredPreviousLevel) || GameManager.instance.IsLevelCompleted(requiredPreviousLevel);
        Debug.Log("Wymagany poziom ukończony: " + levelCompleted);

        // Sprawdź liczbę kart
        bool hasEnoughCards = !requiresCardsToEnter || GameManager.instance.GetDeck().Count >= 5;
        Debug.Log("Posiada wystarczająco kart: " + hasEnoughCards);

        if (!levelCompleted)
        {
            string message = "Musisz ukończyć poziom " + requiredPreviousLevel + ", zanim przejdziesz do poziomu " + levelToLoad + ".";
            Debug.Log(message);
            ShowNotification(message);
            return;
        }

        if (!hasEnoughCards)
        {
            string message = "Nie masz wystarczająco kart, aby wejść na poziom. Potrzebujesz co najmniej 5 kart.";
            Debug.Log(message);
            ShowNotification(message);
            return;
        }

        Debug.Log("Ładowanie poziomu: " + levelToLoad);

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
}
