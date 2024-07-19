using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public string currentLevelName; // Nazwa aktualnego poziomu

   public void CompleteLevel(string levelName)
{
    Debug.Log("Completing level: " + levelName); // Dodaj logowanie przed zmianą stanu
    PlayerPrefs.SetInt(levelName, 1);
    PlayerPrefs.Save();
    Debug.Log("Level " + levelName + " completed.");
}

}
