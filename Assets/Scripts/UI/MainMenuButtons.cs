using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        continueButton.onClick.AddListener(ContinueButton);
        newGameButton.onClick.AddListener(NewGameButton);
        quitButton.onClick.AddListener(QuitButton);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ContinueButton() {
        SceneManager.LoadScene(LevelLoadingData.DayShiftSceneName);
    }
    
    private void NewGameButton() {
        SceneManager.LoadScene(LevelLoadingData.NightShiftSceneName);
    }

    private void QuitButton() {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        
        Application.Quit();
    }
}