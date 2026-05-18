using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class NightLogicManager : MonoBehaviour {
    public static NightLogicManager Instance { get; private set; }
    
    [SerializeField] private ThreatManager threatManager;
    [Space]
    [SerializeField] private float nightDuration = 360f;
    public float NightDuration => nightDuration;
    [Space] 
    [SerializeField] private float startVolume = 0.4f; 
    [SerializeField] private float endVolume = 0.6f;
    [Space] 
    [SerializeField] private DecalProjector whiteboardDecal;
    
    #if UNITY_EDITOR
    [SerializeField] private NightShiftData.AILevelData[] testingData;
    #endif

    private NightShiftData data = LevelLoading.NightShiftData;
    public NightShiftData Data => data;
    
    private float currentTime;

    private float power = 100f;
    
    private AudioSource audioSource;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
        
        // Fallback level setting for testing
        if (data == null || data.IsNull()) {
            LevelLoading.NightShiftData = ScriptableObject.CreateInstance<NightShiftData>();

            #if UNITY_EDITOR
            LevelLoading.NightShiftData.startingAI = testingData;
            LevelLoading.OverrideCurrentSceneTracker(Scene.NightShift);
            #endif
            
            data = LevelLoading.NightShiftData;
        }

        if (data.whiteboardMat != null) {
            whiteboardDecal.material = data.whiteboardMat;
        }
        else {
            whiteboardDecal.enabled = false;
        }
        
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        UpdateTimer();
        causeTimer += Time.deltaTime;
        audioSource.volume = Mathf.Lerp(startVolume, endVolume, currentTime / nightDuration);
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