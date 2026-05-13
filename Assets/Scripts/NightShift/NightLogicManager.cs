using UnityEngine;
using UnityEngine.SceneManagement;

public class NightLogicManager : MonoBehaviour {
    public static NightLogicManager Instance { get; private set; }
    
    [SerializeField] private ThreatManager threatManager;
    [Space]
    [SerializeField] private float nightDuration = 360f;
    public float NightDuration => nightDuration;

    private NightShiftData data = LevelLoading.NightShiftData;
    public NightShiftData Data => data;
    
    private float currentTime;

    private float power = 100f;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
        
        // Fallback level setting for testing
        if (data == null || data.IsNull()) {
            LevelLoading.NightShiftData = ScriptableObject.CreateInstance<NightShiftData>();
            
            LevelLoading.NightShiftData.startingAI = new[] {
                new NightShiftData.AILevelData { ThreatType = ThreatType.Doorman, Level = 0 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Flock, Level = 10 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Technician, Level = 20 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Sheep, Level = 1 },
                new NightShiftData.AILevelData { ThreatType = ThreatType.Thief, Level = 4 }
            };
            
            data = LevelLoading.NightShiftData;
        }
    }

    private void Update() {
        UpdateTimer();
        causeTimer += Time.deltaTime;
    }
    
    private void UpdateTimer() {
        currentTime += Time.deltaTime;
        
        if (currentTime >= nightDuration) {
            GameOverManager.Instance.GameOver(CauseOfDeath.SixAM);
        }
        
        UINightTimer.OnTimerUpdate?.Invoke(currentTime);
    }
    
    private CauseOfDeath currentCauseOfDeath;
    private float causeTimer;
    public void DecreasePower(float amount, CauseOfDeath cause = CauseOfDeath.Power) {
        if (GameOverManager.Instance.IsGameOver) return;
        
        if (cause != CauseOfDeath.Power) {
            currentCauseOfDeath = cause;
            causeTimer = 0f;
        }
        else if (causeTimer > 1f) {
            currentCauseOfDeath = CauseOfDeath.Power;
        }
        
        power -= amount;
        if (power <= 0f) {
            power = 0f;
            GameOverManager.Instance.GameOver(currentCauseOfDeath);
        }
        
        UINightTimer.OnPowerUpdate?.Invoke(power);
    }
}