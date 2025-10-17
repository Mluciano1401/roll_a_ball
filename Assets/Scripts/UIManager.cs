using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Game Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI collectiblesText;
    public TextMeshProUGUI levelText;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

    [Header("Complete Panel Elements")]
    public TextMeshProUGUI completeTimeText;
    public TextMeshProUGUI nextLevelText;

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
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        // Conectar botones automáticamente en Unity 6
        SetupButtons();
    }

    void SetupButtons()
    {
        Debug.Log("=== CONFIGURANDO BOTONES UI ===");

        // Buscar y conectar botones en GameOverPanel
        if (gameOverPanel != null)
        {
            Button[] gameOverButtons = gameOverPanel.GetComponentsInChildren<Button>(true);
            Debug.Log("Botones encontrados en GameOverPanel: " + gameOverButtons.Length);

            foreach (Button btn in gameOverButtons)
            {
                string btnName = btn.gameObject.name.ToLower();
                Debug.Log("  Procesando botón: " + btn.gameObject.name);

                if (btnName.Contains("restart"))
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(OnRestartButton);
                    Debug.Log("    ✅ Restart conectado");
                }
                else if (btnName.Contains("menu"))
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(OnMainMenuButton);
                    Debug.Log("    ✅ Menu conectado");
                }
            }
        }
        else
        {
            Debug.LogWarning("GameOverPanel es null!");
        }

        // Buscar y conectar botones en LevelCompletePanel
        if (levelCompletePanel != null)
        {
            Button[] completeButtons = levelCompletePanel.GetComponentsInChildren<Button>(true);
            Debug.Log("Botones encontrados en LevelCompletePanel: " + completeButtons.Length);

            foreach (Button btn in completeButtons)
            {
                string btnName = btn.gameObject.name.ToLower();
                Debug.Log("  Procesando botón: " + btn.gameObject.name);

                if (btnName.Contains("next"))
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(OnNextLevelButton);
                    btn.interactable = true; // Asegurar que está activo
                    Debug.Log("    ✅ Next Level conectado e interactable");
                }
                else if (btnName.Contains("menu"))
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(OnMainMenuButton);
                    Debug.Log("    ✅ Menu conectado");
                }
            }
        }
        else
        {
            Debug.LogWarning("LevelCompletePanel es null!");
        }

        Debug.Log("=== CONFIGURACIÓN DE BOTONES COMPLETADA ===");
    }

    public void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        // Actualizar timer
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(GameManager.Instance.timeRemaining / 60);
            int seconds = Mathf.FloorToInt(GameManager.Instance.timeRemaining % 60);
            timerText.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);

            // Cambiar color si queda poco tiempo
            if (GameManager.Instance.timeRemaining < 10f)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }

        // Actualizar coleccionables
        if (collectiblesText != null)
        {
            collectiblesText.text = string.Format("Coleccionables: {0}/{1}",
                GameManager.Instance.collectedItems,
                GameManager.Instance.totalCollectibles);
        }

        // Actualizar nivel
        if (levelText != null)
        {
            levelText.text = "Nivel " + GameManager.Instance.currentLevel;
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // NO pausar para que botones funcionen
        // Time.timeScale = 0f;  ← COMENTADO

        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameActive = false;
        }
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);

            if (completeTimeText != null)
            {
                float timeUsed = GameManager.Instance.levelTimes[GameManager.Instance.currentLevel - 1]
                    - GameManager.Instance.timeRemaining;
                completeTimeText.text = string.Format("¡Tiempo: {0:F2}s!", timeUsed);
            }

            if (nextLevelText != null)
            {
                int nextLevel = GameManager.Instance.currentLevel + 1;
                if (nextLevel <= 15)
                {
                    nextLevelText.text = "Siguiente: Nivel " + nextLevel;
                }
                else
                {
                    nextLevelText.text = "¡Todos los niveles completados!";
                }
            }
        }

        // NO pausar el juego para que los botones funcionen
        // Time.timeScale = 0f;  ← COMENTADO

        // Alternativa: solo detener el GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameActive = false;
        }
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    public void OnNextLevelButton()
    {
        Debug.Log("=== OnNextLevelButton PRESIONADO ===");
        Debug.Log("Time.timeScale actual: " + Time.timeScale);

        // CRÍTICO: Restaurar el tiempo antes de cargar
        Time.timeScale = 1f;
        Debug.Log("Time.timeScale establecido a: " + Time.timeScale);

        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager encontrado. Current Level: " + GameManager.Instance.currentLevel);
            Debug.Log("Llamando a LoadNextLevel...");

            // Ocultar panel antes de cargar
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(false);
                Debug.Log("LevelCompletePanel ocultado");
            }

            GameManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("¡GameManager.Instance es null!");
            Debug.LogError("Intentando buscar GameManager en la escena...");

            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                Debug.Log("GameManager encontrado con FindObjectOfType");
                gm.LoadNextLevel();
            }
            else
            {
                Debug.LogError("No se pudo encontrar GameManager en ninguna parte!");
            }
        }
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
}