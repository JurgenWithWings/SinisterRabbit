using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {
    [SerializeField] private TMP_Text bigText;
    [SerializeField] private Color goodTextColor;
    [SerializeField] private Color badTextColor;
    [SerializeField] private TMP_Text littleText;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Button topButton;
    [SerializeField] private TMP_Text topButtonText;
    [SerializeField] private Button bottomButton;
    [SerializeField] private Image image;
    
    public void SetupScreen(GameOverData.DeathInfo deathInfo) {
        littleText.text = deathInfo.deathText;
        tooltipText.text = deathInfo.tooltip;
        image.sprite = deathInfo.image;
        
        topButton.onClick.RemoveAllListeners();
        bottomButton.onClick.RemoveAllListeners();
        
        switch (deathInfo.cause) {
            case CauseOfDeath.Doorman:
            case CauseOfDeath.Flock:
            case CauseOfDeath.Technician:
            case CauseOfDeath.Thief:
            case CauseOfDeath.Timer: 
            case CauseOfDeath.Power:
                bigText.text = "Game Over";
                bigText.color = badTextColor;
                topButtonText.text = "Restart";
                topButton.onClick.AddListener(Restart);
                break;
                
            case CauseOfDeath.SixAM:
            case CauseOfDeath.Repaired:
                bigText.text = "Completed!";
                bigText.color = goodTextColor;
                topButtonText.text = "NextLevel";
                topButton.onClick.AddListener(LoadNextLevel);
                break;
        }
        
        bottomButton.onClick.AddListener(() => {
            LevelLoading.LoadScene(Level.MainMenu);
        });
    }

    private void LoadNextLevel() {
        LevelLoading.LoadNextLevel();
    }

    private void Restart() {
        LevelLoading.ReloadLevel();
    }
}