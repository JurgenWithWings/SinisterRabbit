using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllLevelData", menuName = "All Level Data")]
public class AllLevelData : ScriptableObject {
    public List<LevelData> levels = new List<LevelData>();
}