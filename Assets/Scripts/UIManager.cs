using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        Time.timeScale = 0f;
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
        Time.timeScale = 0f;
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
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
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
