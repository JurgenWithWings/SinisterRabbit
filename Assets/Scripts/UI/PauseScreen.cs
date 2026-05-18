using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour {
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Button resumeButton; 
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    public static Action OnPause;
    
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
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
    }

    private void ResumeGame() {
        if (GameOverManager.Instance.IsGameOver) return;
        if (Time.time < 1) return; // Prevent pausing during initial load
        
        if (LevelLoading.CurrentScene == Scene.DayShift) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
    }
    
    private void LoadMainMenu() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LevelLoading.LoadScene(Scene.MainMenu);
    }
    
    private void QuitGame() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
