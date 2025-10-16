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

        // Obtener el nivel actual desde PlayerPrefs
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        if (currentLevel > 0 && currentLevel <= levelTimes.Length)
        {
            timeRemaining = levelTimes[currentLevel - 1];
        }
        else
        {
            timeRemaining = 60f;
        }

        gameActive = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI();
        }
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
        if (nextLevel <= 15)
        {
            SceneManager.LoadScene("Level" + nextLevel);
        }
        else
        {
            // Todos los niveles completados
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
