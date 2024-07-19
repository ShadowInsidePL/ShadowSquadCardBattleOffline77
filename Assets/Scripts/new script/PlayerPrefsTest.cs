using UnityEngine;

public class PlayerPrefsTest : MonoBehaviour
{
    void Start()
    {
        // Testowanie zapisywania i odczytywania wartości
        PlayerPrefs.SetInt("TestKey", 123);
        PlayerPrefs.Save();
        
        int testValue = PlayerPrefs.GetInt("TestKey", -1); // Wartość domyślna -1, jeśli klucz nie istnieje
        Debug.Log("TestKey value: " + testValue);
    }
}
