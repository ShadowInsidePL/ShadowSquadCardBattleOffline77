using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string battleSelectScene;
    [System.Serializable]
    public struct LevelInfo
    {
        public string levelName;
        public string requiredPreviousLevel;
    }

    public List<LevelInfo> levels;

    void Start()
    {
        AudioManager.instance.PlayMenuMusic();
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(battleSelectScene);
        AudioManager.instance.PlaySFX(0);
    }

    public void LoadLevel(string levelName)
    {
        LevelInfo levelInfo = levels.Find(level => level.levelName == levelName);
        if (string.IsNullOrEmpty(levelInfo.levelName))
        {
            Debug.LogError("Level not found: " + levelName);
            return;
        }

        if (string.IsNullOrEmpty(levelInfo.requiredPreviousLevel) || PlayerPrefs.GetInt(levelInfo.requiredPreviousLevel, 0) == 1)
        {
            SceneManager.LoadScene(levelName);
            AudioManager.instance.PlaySFX(0);
        }
        else
        {
            Debug.Log("Musisz ukończyć poziom " + levelInfo.requiredPreviousLevel + ", zanim przejdziesz do poziomu " + levelName + ".");
            // Możesz dodać tutaj kod, który wyświetli komunikat w UI
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Zamknięto grę");
        AudioManager.instance.PlaySFX(0);
    }
}
