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

    void Start()
    {
        // Mostrar solo el panel principal
        ShowMainPanel();

        // Configurar botones de nivel
        SetupLevelButtons();

        // Cargar volumen guardado
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        }
    }

    void SetupLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;

            if (levelIndex <= unlockedLevel)
            {
                // Nivel desbloqueado
                levelButtons[i].interactable = true;
                int capturedLevel = levelIndex; // Capturar para el closure
                levelButtons[i].onClick.AddListener(() => LoadLevel(capturedLevel));
            }
            else
            {
                // Nivel bloqueado
                levelButtons[i].interactable = false;
            }
        }
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void PlayGame()
    {
        // Cargar el último nivel desbloqueado
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber >= 1 && levelNumber <= 15)
        {
            PlayerPrefs.SetInt("CurrentLevel", levelNumber);
            SceneManager.LoadScene("Level" + levelNumber);
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();
        SetupLevelButtons();
        Debug.Log("Progreso reiniciado");
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}