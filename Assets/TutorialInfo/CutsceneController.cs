using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public MenuLoader menuLoader;
    public float changeTime;
    public string sceneName;
    
    private void Update()
    {
        changeTime -= Time.deltaTime;
        if(changeTime <0)
        {
           SceneManager.LoadScene(sceneName);
        }
        
    }
    
}

