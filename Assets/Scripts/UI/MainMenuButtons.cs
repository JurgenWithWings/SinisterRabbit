using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        continueButton.onClick.AddListener(ContinueButton);
        newGameButton.onClick.AddListener(NewGameButton);
        levelSelectButton.onClick.AddListener(LevelSelectButton);
        settingsButton.onClick.AddListener(SettingsButton);
        quitButton.onClick.AddListener(QuitButton);
    }

    private void ContinueButton() {
        NewGameButton();
    }
    
    private void NewGameButton() {
        SceneManager.LoadScene("Jurgen-Office");
    }
    
    private void LevelSelectButton() {
        NewGameButton();
    }
    
    private void SettingsButton() {
        
    }

    private void QuitButton() {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        
        Application.Quit();
    }
}