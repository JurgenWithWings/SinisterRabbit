using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour {
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject background;
    [SerializeField] private Button resumeButton; 
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    public static Action OnPause;

    private bool isPaused;
    
    private void Awake() {
        OnPause += ShowPauseScreen;
        
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDestroy() {
        OnPause -= ShowPauseScreen;
    }

    private void ShowPauseScreen() {
        if (GameOverManager.Instance.IsGameOver) return;
        if (Time.time < 1) return; // Prevent pausing during initial load
        if (isPaused) return;

        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        AudioListener.pause = true;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        background.SetActive(true);
    }

    private void ResumeGame() {
        if (GameOverManager.Instance.IsGameOver) return;
        if (Time.time < 1) return; // Prevent pausing during initial load
        if (!isPaused) return;
        
        isPaused = false;
        if (LevelLoading.CurrentScene == Scene.DayShift) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            Cursor.lockState = CursorLockMode.Confined;
        }
        
        AudioListener.pause = false;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        background.SetActive(false);
    }
    
    private void LoadMainMenu() {
        ResumeGame();
        LevelLoading.LoadScene(Scene.MainMenu);
    }
    
    private void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
