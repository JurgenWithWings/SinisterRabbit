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
            0 => LevelLoading.NightShiftData.startingAI,
            1 => LevelLoading.NightShiftData.oneAMLevels,
            2 => LevelLoading.NightShiftData.twoAMLevels,
            3 => LevelLoading.NightShiftData.threeAMLevels,
            4 => LevelLoading.NightShiftData.fourAMLevels,
            5 => LevelLoading.NightShiftData.fiveAMLevels,
            _ => LevelLoading.NightShiftData.startingAI
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
        GameOverManager.Instance.GameOver(source.DeathCause);
    }
}