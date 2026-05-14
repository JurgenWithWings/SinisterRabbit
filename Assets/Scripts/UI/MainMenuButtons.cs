using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TMP_Text resetText;

    public static Action OnResetProgress;

    private void Awake() {
        continueButton.onClick.AddListener(ContinueButton);
        newGameButton.onClick.AddListener(NewGameButton);
        quitButton.onClick.AddListener(QuitButton);
        resetButton.onClick.AddListener(ResetProgress);
        
        if (LevelLoading.GetHighestLevelCompleted() < 0) {
            continueButton.interactable = false;
            resetButton.gameObject.SetActive(false);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ContinueButton() {
        LevelLoading.LoadNextLevel();
    }
    
    private void NewGameButton() {
        LevelLoading.LoadLevel(0);
    }

    private void QuitButton() {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        
        Application.Quit();
    }

    private int resetCounter = 3;
    private void ResetProgress() {
        resetCounter--;
        resetText.text = $"Reset Progress ({resetCounter})";
        
        if (resetCounter <= 0) {
            continueButton.interactable = false;
            resetButton.interactable = false;
            resetText.text = "Progress Reset";
            LevelLoading.ResetProgress();
            OnResetProgress?.Invoke();
        }
    }
}