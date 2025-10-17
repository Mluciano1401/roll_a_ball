using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainPanel;
    public GameObject levelSelectPanel;
    public GameObject optionsPanel;

    [Header("Level Buttons")]
    public Button[] levelButtons;

    [Header("Audio")]
    public Slider volumeSlider;

    private static int loadAttempts = 0;
    private static float lastLoadTime = 0f;

    void Start()
    {
        Debug.Log("=== MainMenu.Start() ===");
        Debug.Log("MainMenu iniciado");

        // Detectar bucle infinito
        if (Time.time - lastLoadTime < 1f)
        {
            loadAttempts++;
            if (loadAttempts > 3)
            {
                Debug.LogError("❌ BUCLE INFINITO DETECTADO!");
                Debug.LogError("El MainMenu se está recargando continuamente.");
                Debug.LogError("Posibles causas:");
                Debug.LogError("1. Level1 no existe");
                Debug.LogError("2. Level1 no está en Build Settings");
                Debug.LogError("3. Hay un error en algún script");
                Debug.LogError("REVISA LA CONSOLE PARA VER ERRORES ANTERIORES");

                loadAttempts = 0;
                return; // Detener ejecución para evitar más bucles
            }
        }
        else
        {
            loadAttempts = 0;
        }
        lastLoadTime = Time.time;

        // Verificar que los paneles están asignados
        if (mainPanel == null) Debug.LogError("❌ MainPanel no está asignado!");
        if (levelSelectPanel == null) Debug.LogWarning("⚠️ LevelSelectPanel no está asignado");
        if (optionsPanel == null) Debug.LogWarning("⚠️ OptionsPanel no está asignado");

        // Mostrar solo el panel principal
        ShowMainPanel();

        // Configurar botones de nivel
        SetupLevelButtons();

        // Cargar volumen guardado
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        Debug.Log("MainMenu configuración completada");
    }

    void SetupLevelButtons()
    {
        if (levelButtons == null || levelButtons.Length == 0)
        {
            Debug.LogWarning("No hay botones de nivel asignados");
            return;
        }

        int unlockedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        Debug.Log("Niveles desbloqueados hasta: " + unlockedLevel);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null)
            {
                Debug.LogWarning("Level button " + i + " es null");
                continue;
            }

            int levelIndex = i + 1;

            if (levelIndex <= unlockedLevel)
            {
                // Nivel desbloqueado
                levelButtons[i].interactable = true;
                int capturedLevel = levelIndex;

                // Limpiar listeners anteriores
                levelButtons[i].onClick.RemoveAllListeners();
                // Añadir nuevo listener
                levelButtons[i].onClick.AddListener(() => LoadLevel(capturedLevel));

                Debug.Log("Nivel " + levelIndex + " desbloqueado");
            }
            else
            {
                // Nivel bloqueado
                levelButtons[i].interactable = false;
                Debug.Log("Nivel " + levelIndex + " bloqueado");
            }
        }
    }

    public void ShowMainPanel()
    {
        Debug.Log("Mostrando panel principal");
        if (mainPanel != null) mainPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        Debug.Log("Mostrando selección de niveles");
        if (mainPanel != null) mainPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        Debug.Log("Mostrando opciones");
        if (mainPanel != null) mainPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void PlayGame()
    {
        Debug.Log("=== PlayGame llamado ===");

        // Ocultar todos los paneles antes de cargar
        HideAllPanels();

        // IMPORTANTE: Siempre empezar desde Level1 cuando se hace clic en JUGAR
        int currentLevel = 1;

        Debug.Log("Intentando cargar Level1...");

        // Verificar que Level1 existe antes de intentar cargar
        if (!SceneExists("Level1"))
        {
            Debug.LogError("❌ ERROR: Level1 no existe o no está en Build Settings!");
            Debug.LogError("Solución: Ve a File → Build Profiles y añade Level1");

            // Mostrar mensaje en pantalla
            ShowMainPanel();
            return;
        }

        LoadLevel(currentLevel);
    }

    // Método nuevo para continuar desde el último nivel
    public void ContinueGame()
    {
        Debug.Log("ContinueGame llamado");
        HideAllPanels();

        // Cargar el último nivel desbloqueado
        int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        Debug.Log("Continuando desde nivel: " + savedLevel);

        LoadLevel(savedLevel);
    }

    public void LoadLevel(int levelNumber)
    {
        Debug.Log("=== LoadLevel llamado ===");
        Debug.Log("Número de nivel: " + levelNumber);

        // Ocultar todos los paneles antes de cargar la escena
        HideAllPanels();

        if (levelNumber >= 1 && levelNumber <= 15)
        {
            string sceneName = "Level" + levelNumber;
            Debug.Log("Nombre de escena: " + sceneName);

            // Verificar que existe
            if (!SceneExists(sceneName))
            {
                Debug.LogError($"❌ ERROR: {sceneName} no existe en Build Settings!");
                ShowMainPanel();
                return;
            }

            PlayerPrefs.SetInt("CurrentLevel", levelNumber);
            PlayerPrefs.Save();

            try
            {
                Debug.Log("Ejecutando LoadScene...");
                SceneManager.LoadScene(sceneName);
                Debug.Log("LoadScene ejecutado exitosamente");
            }
            catch (System.Exception e)
            {
                Debug.LogError("ERROR al cargar escena: " + e.Message);
                ShowMainPanel();
            }
        }
        else
        {
            Debug.LogError("Número de nivel inválido: " + levelNumber);
        }
    }

    // Verificar si una escena existe en Build Settings
    bool SceneExists(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        Debug.Log($"Total de escenas en Build: {sceneCount}");

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"  Escena {i}: {sceneNameInBuild}");

            if (sceneNameInBuild == sceneName)
            {
                Debug.Log($"✅ {sceneName} encontrado en Build Settings");
                return true;
            }
        }

        Debug.LogError($"❌ {sceneName} NO encontrado en Build Settings");
        return false;
    }

    // Método para ocultar todos los paneles
    void HideAllPanels()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void ResetProgress()
    {
        Debug.Log("Progreso reiniciado");
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();
        SetupLevelButtons();
    }

    public void SetVolume(float volume)
    {
        Debug.Log("Volumen cambiado a: " + volume);
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}