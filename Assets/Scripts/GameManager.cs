using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración del Nivel")]
    public int currentLevel = 1;
    public float[] levelTimes = new float[15]
    {
        60f, 55f, 50f, 48f, 45f,
        43f, 40f, 38f, 35f, 33f,
        30f, 28f, 25f, 23f, 20f
    };

    [Header("Estado del Juego")]
    public int totalCollectibles;
    public int collectedItems = 0;
    public float timeRemaining;
    public bool gameActive = false;

    private Vector3 playerStartPosition;
    private GameObject player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStartPosition = player.transform.position;
        }

        totalCollectibles = GameObject.FindGameObjectsWithTag("Collectible").Length;

        // CRÍTICO: NO sobrescribir currentLevel
        // Usar el valor configurado en el Inspector de cada escena
        Debug.Log($"=== INICIANDO NIVEL {currentLevel} ===");

        // Configurar tiempo según el nivel
        if (currentLevel > 0 && currentLevel <= levelTimes.Length)
        {
            timeRemaining = levelTimes[currentLevel - 1];
        }
        else
        {
            Debug.LogWarning($"CurrentLevel ({currentLevel}) fuera de rango. Usando tiempo por defecto.");
            timeRemaining = 60f;
        }

        gameActive = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI();
        }

        Debug.Log($"Tiempo límite: {timeRemaining}s");
        Debug.Log($"Coleccionables totales: {totalCollectibles}");
    }

    void Update()
    {
        if (!gameActive) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            GameOver();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI();
        }
    }

    public void CollectItem()
    {
        collectedItems++;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI();
        }

        if (collectedItems >= totalCollectibles)
        {
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        gameActive = false;

        // Guardar progreso
        int nextLevel = currentLevel + 1;
        if (nextLevel <= 15)
        {
            PlayerPrefs.SetInt("CurrentLevel", nextLevel);
            PlayerPrefs.Save();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLevelComplete();
        }
    }

    void GameOver()
    {
        gameActive = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver();
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        int nextLevel = currentLevel + 1;
        Debug.Log("=== LoadNextLevel LLAMADO ===");
        Debug.Log("Nivel actual: " + currentLevel);
        Debug.Log("Siguiente nivel: " + nextLevel);

        if (nextLevel <= 15)
        {
            string sceneName = "Level" + nextLevel;
            Debug.Log("Nombre de escena a cargar: " + sceneName);

            // Actualizar PlayerPrefs ANTES de cargar
            PlayerPrefs.SetInt("CurrentLevel", nextLevel);
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs actualizado a nivel: " + nextLevel);

            // Asegurar que el tiempo está correcto
            Time.timeScale = 1f;

            // Intentar cargar la escena
            try
            {
                Debug.Log("Intentando cargar escena: " + sceneName);
                SceneManager.LoadScene(sceneName);
                Debug.Log("LoadScene ejecutado exitosamente");
            }
            catch (System.Exception e)
            {
                Debug.LogError("ERROR al cargar escena: " + e.Message);
                Debug.LogError("Stack trace: " + e.StackTrace);
                Debug.LogWarning("Verifica que '" + sceneName + "' existe en Build Profiles");

                // Si falla, volver al menú
                Debug.Log("Volviendo a MainMenu como fallback...");
                SceneManager.LoadScene("MainMenu");
            }
        }
        else
        {
            // Todos los niveles completados
            Debug.Log("¡Todos los niveles completados!");
            Debug.Log("Volviendo al menú principal...");
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}