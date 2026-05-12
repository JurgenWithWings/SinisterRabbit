using System;
using System.Collections;
using UnityEngine;

public enum CauseOfDeath {
    Doorman,
    Flock,
    Technician,
    Thief,
    Timer,
    SixAM,
    Repaired,
}

public class GameOverManager : MonoBehaviour {
    public static GameOverManager Instance { get; private set; }
    
    [SerializeField] private GameObject player;
    [SerializeField] private GameOverScreen gameOverScreen;
    [SerializeField] private GameObject gameOverBox;

    private static GameOverData GameOverData {
        get {
            if (dataCache == null) {
                dataCache = Resources.Load<GameOverData>("GameOverData");
            }
            return dataCache;
        }
    }
    private static GameOverData dataCache;
    
    public bool IsGameOver { get; private set; }
    
    public static event Action OnGameOver;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void GameOver(CauseOfDeath cause) {
        if (!IsGameOver) {
            StartCoroutine(GameOverCoroutine(cause));
        }
    }
    
    private IEnumerator GameOverCoroutine(CauseOfDeath cause) {
        IsGameOver = true;
        OnGameOver?.Invoke();
        
        if (player.TryGetComponent(out Player playerComponent)) {
            playerComponent.Teleport(gameOverBox.transform.position);
        }
        else {
            player.transform.position = gameOverBox.transform.position;
        }
        gameOverScreen.gameObject.SetActive(true);
        
        GameOverData.DeathInfo deathInfo = cause switch {
            CauseOfDeath.Doorman => GameOverData.doormanDeathInfo,
            CauseOfDeath.Flock => GameOverData.flockDeathInfo,
            CauseOfDeath.Technician => GameOverData.technicianDeathInfo,
            CauseOfDeath.Thief => GameOverData.thiefDeathInfo,
            CauseOfDeath.Timer => GameOverData.timerDeathInfo,
            CauseOfDeath.SixAM => GameOverData.sixAMDeathInfo,
            CauseOfDeath.Repaired => GameOverData.repairedDeathInfo,
        };
        gameOverScreen.SetupScreen(deathInfo);
        
        yield return new WaitForSeconds(2f);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}