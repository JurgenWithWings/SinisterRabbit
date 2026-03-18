using System.Collections.Generic;
using UnityEngine;

public class ThreatManager : MonoBehaviour {
    [SerializeField] private List<Threat> threats = new List<Threat>();
    
    private Dictionary<string, Threat> occupiedStates = new();

    private void Awake() {
        foreach (Threat threat in threats) {
            threat.Init(this);
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
        // Code GameOver Logic Here.
        
        // Game Quits on Death
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}