using UnityEngine;
using UnityEngine.SceneManagement;

public static class DependencySceneLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void LoadDependencies()
    {
        if (!SceneManager.GetSceneByName("Dependencies").isLoaded)
        {
            SceneManager.LoadScene("Dependencies", LoadSceneMode.Additive);
        }
    }
}
