using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DayLogicManager : MonoBehaviour {
    public static DayLogicManager Instance;

    private DayShiftData data = LevelLoadingData.DayShiftData;
    public DayShiftData Data => data;
    
    [SerializeField] private List<RepairableMachine> availableMachines;
    [SerializeField] private List<GoldenEggSpawner> availableEggSpawners;

    private float timeRemaining;
    private int repairedMachines;
    private int collectedEggs;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
        
        // Fallback level setting for testing
        if (data == null || data.shiftDuration == 0) {
            LevelLoadingData.DayShiftData = ScriptableObject.CreateInstance<DayShiftData>();
            LevelLoadingData.DayShiftData.shiftDuration = 60f;
            LevelLoadingData.DayShiftData.numBrokenMachines = 3;
            LevelLoadingData.DayShiftData.numberOfGoldenEggs = 3;
            data = LevelLoadingData.DayShiftData;
        }
    }
    
    private void Start() {
        timeRemaining = data.shiftDuration;
        
        AssignBrokenMachines();
        
        SpawnGoldenEggs();
    }

    private void Update() {
        UIDayTimer.OnTimerUpdate?.Invoke(timeRemaining);

        if (timeRemaining <= 0f) {
            SceneManager.LoadScene(LevelLoadingData.MainMenuSceneName);
            //TODO: Implement Game Over logic.
        }
        
        timeRemaining -= Time.deltaTime;
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

    private void OnOnRepairedMachine() {
        repairedMachines++;
        if (repairedMachines >= data.numBrokenMachines) {
            //TODO: Implement Win Logic
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

    private void OnOnEggCollected() {
        collectedEggs++;
        if (collectedEggs >= data.numberOfGoldenEggs) {
            //TODO: Implement Golden Egg Win Logic
            timeRemaining += 100f;
        }
    }

    public void AddTime(float time) {
        timeRemaining += time;
        UIDayTimer.OnTimerUpdate?.Invoke(timeRemaining);
    }
}