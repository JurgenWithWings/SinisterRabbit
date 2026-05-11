using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DayLogicManager : MonoBehaviour {
    public static DayLogicManager Instance { get; private set; }

    private DayShiftData data = LevelLoadingData.DayShiftData;
    public DayShiftData Data => data;
    
    [SerializeField] private Collider startingArea;
    [SerializeField] private List<RepairableMachine> availableMachines;
    [SerializeField] private List<GoldenEggSpawner> availableEggSpawners;

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
            LevelLoadingData.DayShiftData = ScriptableObject.CreateInstance<DayShiftData>();
            
            LevelLoadingData.DayShiftData.shiftDuration = 60f;
            LevelLoadingData.DayShiftData.numBrokenMachines = 3;
            LevelLoadingData.DayShiftData.numberOfGoldenEggs = 3;
            
            data = LevelLoadingData.DayShiftData;
        }
    }
    
    private void Start() {
        timeRemaining = data.shiftDuration;
        
        UIDayTimer.OnTimerUpdate?.Invoke(timeRemaining);
        
        AssignBrokenMachines();
        
        SpawnGoldenEggs();
    }

    private void Update() {
        if (timerStarted) {
            UpdateTimer();
        }
    }

    private void UpdateTimer() {
        if (timeRemaining <= 0f) {
            SceneManager.LoadScene(LevelLoadingData.MainMenuSceneName);
            //TODO: Implement Game Over logic.
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
            //TODO: Implement Win Logic
            SceneManager.LoadScene(LevelLoadingData.MainMenuSceneName);
        }
    }
    
    private void OnOnEggCollected() {
        collectedEggs++;
        if (collectedEggs >= data.numberOfGoldenEggs) {
            //TODO: Implement Golden Egg Win Logic
            timeRemaining += 100f;
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