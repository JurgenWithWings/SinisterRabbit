using UnityEngine;
using UnityEngine.SceneManagement;

public class NightLogicManager : MonoBehaviour {
    public static NightLogicManager Instance { get; private set; }
    
    [SerializeField] private ThreatManager threatManager;
    [Space]
    [SerializeField] private float nightDuration = 360f;
    public float NightDuration => nightDuration;

    private NightShiftData data = LevelLoadingData.NightShiftData;
    public NightShiftData Data => data;
    
    private float currentTime;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
        
        // Fallback level setting for testing
        if (data == null || data.IsNull()) {
            LevelLoadingData.NightShiftData = ScriptableObject.CreateInstance<NightShiftData>();
            
            LevelLoadingData.NightShiftData.startingAI = new[] {
                new NightShiftData.AILevelData { ThreatType = ThreatType.Doorman, Level = 20 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Flock, Level = 3 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Technician, Level = 2 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Sheep, Level = 1 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Thief, Level = 4 }
            };
            LevelLoadingData.NightShiftData.oneAMLevels = new[] {
                new NightShiftData.AILevelData { ThreatType = ThreatType.Doorman, Level = 20 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Flock, Level = 10 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Technician, Level = 6 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Sheep, Level = 6 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Thief, Level = 7 }
            };
            
            data = LevelLoadingData.NightShiftData;
        }
    }

    private void Update() {
        UpdateTimer();
    }
    
    private void UpdateTimer() {
        currentTime += Time.deltaTime;
        
        if (currentTime >= nightDuration) {
            SceneManager.LoadScene(LevelLoadingData.MainMenuSceneName);
            //TODO: Implement Win logic.
        }
        
        UINightTimer.OnTimerUpdate?.Invoke(currentTime);
    }
}