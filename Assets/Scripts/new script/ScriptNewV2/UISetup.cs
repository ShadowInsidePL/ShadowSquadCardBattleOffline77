using UnityEngine;
using UnityEngine.UI;

public class UISetup : MonoBehaviour
{
    public Canvas canvas;
    public Camera uiCamera;

    void Start()
    {
        if (canvas != null && uiCamera != null)
        {
            // Ustawienie Canvas w trybie Screen Space - Camera
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera;
            canvas.planeDistance = 5; // Ustaw odpowiednią odległość

            // Sprawdź ustawienia Sorting Layer
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 5;

            // Dodanie komponentu maski do panelu
            GameObject panel = GameObject.Find("Panel"); // Znajdź swój panel
            if (panel != null)
            {
                if (panel.GetComponent<Mask>() == null)
                {
                    panel.AddComponent<Mask>(); // Dodaj Mask
                }
            }

            // Przypisz warstwę "UI" do panelu i jego dzieci
            AssignLayerRecursively(panel, LayerMask.NameToLayer("UI"));
        }
    }

    void AssignLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            AssignLayerRecursively(child.gameObject, layer);
        }
    }
}
