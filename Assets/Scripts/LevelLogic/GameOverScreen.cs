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
    [Space]
    [SerializeField] private Animation animation;
    [Space]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip secondThudSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioSource voiceAudioSource;
    
    public void SetupScreen(GameOverData.DeathInfo deathInfo) {
        littleText.text = deathInfo.deathText;
        tooltipText.text = deathInfo.tooltip;
        image.sprite = deathInfo.image;
        voiceAudioSource.clip = deathInfo.deathVoiceLine;
        voiceAudioSource.Play();
        
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
                topButtonText.text = "Try Again";
                topButton.onClick.AddListener(Restart);
                break;
                
            case CauseOfDeath.SixAM:
            case CauseOfDeath.Repaired:
                animation.clip = animation["CompletionFadeIn"].clip;
                audioSource.clip = winSound;
                bigText.text = "Completed!";
                bigText.color = goodTextColor;
                topButtonText.text = "NextLevel";
                topButton.onClick.AddListener(LoadNextLevel);
                break;
        }
        
        animation.Play();
        audioSource.Play();
        
        bottomButton.onClick.AddListener(() => {
            LevelLoading.LoadScene(Scene.MainMenu);
        });
    }

    private void LoadNextLevel() {
        LevelLoading.LoadNextLevel();
    }

    private void Restart() {
        LevelLoading.ReloadLevel();
    }

    public void PlaySecondThud() {
        audioSource.PlayOneShot(secondThudSound);
    }
}