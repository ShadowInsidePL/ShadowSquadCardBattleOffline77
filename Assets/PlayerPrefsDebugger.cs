using UnityEngine;

public class PlayerPrefsDebugger : MonoBehaviour
{
    public string[] levelNames;

   void Start()
{
    PlayerPrefs.SetInt("Walka 1", 1); // Przykład ustawienia wartości
    PlayerPrefs.SetInt("Walka 2", 0); // Przykład ustawienia wartości
    PlayerPrefs.Save();
    LogPlayerPrefs();
    

}


    void LogPlayerPrefs()
    {
        Debug.Log("Stan PlayerPrefs:");
        foreach (string key in new[] { "Walka 1", "Walka 2" }) // Dodaj wszystkie klucze, które są używane
        {
          Debug.Log(key + ": " + PlayerPrefs.GetInt(key, 0)); // Wartość domyślna 0

        }
    }
}
