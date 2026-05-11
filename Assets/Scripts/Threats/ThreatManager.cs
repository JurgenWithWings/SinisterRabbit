using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThreatManager : MonoBehaviour {
    [SerializeField] private List<Threat> threats = new List<Threat>();
    
    /// Key: State Name, Value: Threat occupying it
    private Dictionary<string, Threat> occupiedStates = new();

    private void Awake() {
        foreach (Threat threat in threats) {
            threat.Init(this);
        }
    }

    private void Start() {
        UpdateLevels(0);
        
        UINightTimer.OnHourPassed += OnHourPassed;
    }
    
    private void OnDestroy() {
        UINightTimer.OnHourPassed -= OnHourPassed;
    }

    private void OnHourPassed(int hour) {
        UpdateLevels(hour);
    }

    private void UpdateLevels(int hour) {
        NightShiftData.AILevelData[] aiData = hour switch {
            0 => LevelLoadingData.NightShiftData.startingAI,
            1 => LevelLoadingData.NightShiftData.oneAMLevels,
            2 => LevelLoadingData.NightShiftData.twoAMLevels,
            3 => LevelLoadingData.NightShiftData.threeAMLevels,
            4 => LevelLoadingData.NightShiftData.fourAMLevels,
            5 => LevelLoadingData.NightShiftData.fiveAMLevels,
            _ => LevelLoadingData.NightShiftData.startingAI
        };
        
        if (aiData == null) return;
        
        foreach (NightShiftData.AILevelData ai in aiData) {
            foreach (Threat threat in threats) {
                if (threat.ThreatType == ai.ThreatType) {
                    threat.UpdateAILevel(ai.Level);
                }
            }
        }
    }
    
    public bool TryEnterState(string state, Threat threat) {
        if (occupiedStates.ContainsKey(state)) {
            return false;
        }

        occupiedStates[state] = threat;
        return true;
    }

    public void LeaveState(string state, Threat threat) {
        if (occupiedStates.TryGetValue(state, out Threat current) && current == threat) {
            occupiedStates.Remove(state);
        }
    }

    public bool IsOccupied(string state) {
        return occupiedStates.ContainsKey(state);
    }

    public void GameOver(Threat source) {
        Debug.Log($"Game Over caused by {source.name}");
        
        SceneManager.LoadScene(LevelLoadingData.MainMenuSceneName);
        //TODO: Code GameOver Logic Here.

    }
}