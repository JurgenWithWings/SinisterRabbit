using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DayLogicManager : MonoBehaviour {
    public static DayLogicManager Instance { get; private set; }

    private DayShiftData data = LevelLoading.DayShiftData;
    public DayShiftData Data => data;
    
    [SerializeField] private Collider startingArea;
    [SerializeField] private List<RepairableMachine> availableMachines;
    [SerializeField] private List<GoldenEggSpawner> availableEggSpawners;

    #if UNITY_EDITOR
    [Header("Testing Data")] 
    [SerializeField] private float shiftDuration = 30f;
    [SerializeField] private int numBrokenMachines = 5;
    [SerializeField] private int numberOfGoldenEggs = 3;
    #endif
    
    private bool timerStarted;
    private float timeRemaining;
    private int repairedMachines;
    private int collectedEggs;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
        
        // Fallback level setting for testing
        if (data == null || data.IsNull()) {
            LevelLoading.DayShiftData = ScriptableObject.CreateInstance<DayShiftData>();
            
            #if UNITY_EDITOR
            LevelLoading.DayShiftData.shiftDuration = shiftDuration;
            LevelLoading.DayShiftData.numBrokenMachines = numBrokenMachines;
            LevelLoading.DayShiftData.numberOfGoldenEggs = numberOfGoldenEggs;
            LevelLoading.OverrideCurrentSceneTracker(Scene.DayShift);
            #endif
            
            data = LevelLoading.DayShiftData;
        }
    }
    
    private void Start() {
        timeRemaining = data.shiftDuration;
        
        UIDayTimer.OnTimerUpdate?.Invoke(timeRemaining);
        
        GameOverManager.OnGameOver += OnGameOver;
        
        AssignBrokenMachines();
        
        SpawnGoldenEggs();
    }

    private void OnDestroy() {
        GameOverManager.OnGameOver -= OnGameOver;
    }

    private void OnGameOver() {
        timerStarted = false;
        UIDayTimer.OnTimerVisibility?.Invoke(false);
    }

    private void Update() {
        if (timerStarted) {
            UpdateTimer();
        }
    }

    private void UpdateTimer() {
        if (timeRemaining <= 0f) {
            GameOverManager.Instance.GameOver(CauseOfDeath.Timer);
            timerStarted = false;
        }
        
        timeRemaining -= Time.deltaTime;
        
        UIDayTimer.OnTimerUpdate?.Invoke(timeRemaining);
    }

    private void AssignBrokenMachines() {
        if (availableMachines.Count < data.numBrokenMachines) {
            Debug.LogWarning("Not enough machines to break! Please add more machines to the availableMachines list.");
            return;
        }
        
        for (int i = 0; i < data.numBrokenMachines; i++) {
            bool done = false;
            while (!done) {
                int randomIndex = Random.Range(0, availableMachines.Count);
                if (!availableMachines[randomIndex].Broken) {
                    availableMachines[randomIndex].SetBrokenState(true);
                    availableMachines[randomIndex].OnRepairedMachine += OnOnRepairedMachine;
                    done = true;
                }
            }
        }
    }

    private void SpawnGoldenEggs() {
        if (availableMachines.Count < data.numberOfGoldenEggs) {
            Debug.LogWarning("Not enough egg spawns! Please add more machines to the availableEggSpawners list.");
            return;
        }
        
        for (int i = 0; i < data.numberOfGoldenEggs; i++) {
            bool done = false;
            while (!done) {
                int randomIndex = Random.Range(0, availableEggSpawners.Count);
                if (!availableEggSpawners[randomIndex].Spawned) {
                    availableEggSpawners[randomIndex].Spawn();
                    availableEggSpawners[randomIndex].OnEggCollected += OnOnEggCollected;
                    done = true;
                }
            }
        }
    }

    private void OnOnRepairedMachine() {
        repairedMachines++;
        if (repairedMachines >= data.numBrokenMachines) {
            GameOverManager.Instance.GameOver(CauseOfDeath.Repaired);
        }
    }
    
    private void OnOnEggCollected() {
        collectedEggs++;
        if (collectedEggs >= data.numberOfGoldenEggs) {
            //TODO: Implement Golden Egg Win Logic
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.TryGetComponent(out TriggerEfffector effector)) {
            timerStarted = true;
            startingArea.enabled = false;
        }
    }
    
    public void AddTime(float time) {
        timeRemaining += time;
        UIDayTimer.OnTimerUpdate?.Invoke(timeRemaining);
    }
}