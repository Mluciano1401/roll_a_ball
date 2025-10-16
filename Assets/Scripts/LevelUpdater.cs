using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Este script ayuda a configurar rápidamente el número de nivel
// basándose en el nombre de la escena
public class LevelUpdater : MonoBehaviour
{
    [ContextMenu("Auto-Configurar Nivel")]
    void AutoConfigureLevel()
    {
#if UNITY_EDITOR
        string sceneName = EditorSceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Level"))
        {
            string numberPart = sceneName.Replace("Level", "");
            if (int.TryParse(numberPart, out int levelNumber))
            {
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null)
                {
                    gm.currentLevel = levelNumber;
                    EditorUtility.SetDirty(gm);
                    Debug.Log($"Nivel configurado a: {levelNumber}");
                }
                else
                {
                    Debug.LogError("No se encontró GameManager en la escena");
                }
            }
        }
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelUpdater))]
public class LevelUpdaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelUpdater updater = (LevelUpdater)target;

        if (GUILayout.Button("Auto-Configurar Nivel"))
        {
            updater.SendMessage("AutoConfigureLevel");
        }
    }
}
#endif