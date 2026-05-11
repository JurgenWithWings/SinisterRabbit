using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NightData", menuName = "ScriptableObjects/NightData")]
public class NightShiftData : ScriptableObject {
    [Serializable] public struct AILevelData {
        public ThreatType ThreatType;
        [Range(0, 20)] public int Level;
    }
    
    [Header("AI Levels Per Hour")]
    public AILevelData[] startingAI = {
        new AILevelData { ThreatType = ThreatType.Doorman, Level = 1 },
        new AILevelData { ThreatType = ThreatType.Flock, Level = 1 },
        new AILevelData { ThreatType = ThreatType.Technician, Level = 1 },
        new AILevelData { ThreatType = ThreatType.Sheep, Level = 1 },
        new AILevelData { ThreatType = ThreatType.Thief, Level = 1 }
    };

    public AILevelData[] oneAMLevels;
    public AILevelData[] twoAMLevels;
    public AILevelData[] threeAMLevels;
    public AILevelData[] fourAMLevels;
    public AILevelData[] fiveAMLevels;


    public bool IsNull() {
        return startingAI == null || startingAI.Length == 0;
    }
}