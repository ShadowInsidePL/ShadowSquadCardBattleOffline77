
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuLoader : MonoBehaviour
{
    // Funkcja do wczytywania menu
    public void LoadMenu()
    {
        // Wczytaj scenę menu głównego. Załóżmy, że ma ona indeks 0.
        SceneManager.LoadScene(0);
    }
}
