using UnityEngine;

[CreateAssetMenu(fileName = "DayData", menuName = "ScriptableObjects/DayData")]
public class DayShiftData : ScriptableObject {
    public float shiftDuration = 120f;
    public int numBrokenMachines = 6;
    public int numberOfGoldenEggs = 3;

    public bool IsNull() {
        return shiftDuration == 0 || numBrokenMachines == 0;
    }
}