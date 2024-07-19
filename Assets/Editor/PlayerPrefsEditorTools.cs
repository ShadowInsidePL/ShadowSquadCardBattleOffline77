using UnityEditor;
using UnityEngine;

public class PlayerPrefsEditorTools
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPlayerPrefs()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Cannot clear PlayerPrefs while the game is running!");
            return;
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }
}