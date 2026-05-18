using UnityEngine;

[CreateAssetMenu(fileName = "DayData", menuName = "ScriptableObjects/DayData")]
public class DayShiftData : LevelData {
    public float shiftDuration = 0;
    public int numBrokenMachines = 0;
    public int numberOfGoldenEggs = 0;

    public override bool IsNull() {
        return shiftDuration == 0 || numBrokenMachines == 0;
    }
}