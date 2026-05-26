using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AutoPlayMainMenu
{
    static AutoPlayMainMenu()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            string mainMenuPath = "Assets/Scenes/MainMenu.unity";

            if (SceneManager.GetActiveScene().path != mainMenuPath)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(mainMenuPath);
            }
        }
    }
}