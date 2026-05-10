using UnityEngine;

[CreateAssetMenu(fileName = "DayData", menuName = "ScriptableObjects/DayData")]
public class DayShiftData : ScriptableObject {
    public float shiftDuration = 120f;
    public int numBrokenMachines = 6;
    public int numberOfGoldenEggs = 3;
}