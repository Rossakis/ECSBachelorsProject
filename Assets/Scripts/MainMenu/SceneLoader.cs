using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string EcsSceneName;
    public string MonoSceneName;
    
    public void LoadEcsScene()
    {
        if (!string.IsNullOrEmpty(EcsSceneName))
        {
            SceneManager.LoadScene(EcsSceneName);
        }
        else
        {
            Debug.LogWarning("ECS Scene name is not set.");
        }
    }
    
    public void LoadMonoScene()
    {
        if (!string.IsNullOrEmpty(MonoSceneName))
        {
            SceneManager.LoadScene(MonoSceneName);
        }
        else
        {
            Debug.LogWarning("Mono scene name is not set.");
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
