using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButtons : MonoBehaviour {
    public void NightButtonPressed(NightShiftData data) {
        if (data == null || data.IsNull()) {
            Debug.LogWarning("NightShiftData is not set or has invalid values. Please assign a valid NightShiftData ScriptableObject.");
            return;
        }
        
        LevelLoadingData.NightShiftData = data;
        SceneManager.LoadScene(LevelLoadingData.NightShiftSceneName);
    }
    
    public void DayButtonPressed(DayShiftData data) {
        if (data == null || data.IsNull()) {
            Debug.LogWarning("DayShiftData is not set or has invalid values. Please assign a valid DayShiftData ScriptableObject.");
            return;
        }
        
        LevelLoadingData.DayShiftData = data;
        SceneManager.LoadScene(LevelLoadingData.DayShiftSceneName);
    }
}