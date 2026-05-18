using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NightData", menuName = "ScriptableObjects/NightData")]
public class NightShiftData : LevelData {
    public float camsPowerDrain = 0.5f;
    
    [Serializable] public struct AILevelData {
        public ThreatType ThreatType;
        [Range(0, 20)] public int Level;
    }
    
    [Header("AI Levels Per Hour")]
    public AILevelData[] startingAI = {
        new AILevelData { ThreatType = ThreatType.Doorman, Level = 20 },
        new AILevelData { ThreatType = ThreatType.Flock, Level = 20 },
        new AILevelData { ThreatType = ThreatType.Technician, Level = 20 },
        new AILevelData { ThreatType = ThreatType.Chef, Level = 20 },
        new AILevelData { ThreatType = ThreatType.Sheep, Level = 20 },
        new AILevelData { ThreatType = ThreatType.Thief, Level = 20 }
    };

    public AILevelData[] oneAMLevels;
    public AILevelData[] twoAMLevels;
    public AILevelData[] threeAMLevels;
    public AILevelData[] fourAMLevels;
    public AILevelData[] fiveAMLevels;


    public override bool IsNull() {
        return startingAI == null || startingAI.Length == 0;
    }
}

public abstract class LevelData : ScriptableObject {
    public int levelIndex;
    public abstract bool IsNull();
}