using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoading {
    public static DayShiftData DayShiftData;
    public static NightShiftData NightShiftData;
    
    public const string MainMenuSceneName = "MainMenu";
    public const string DayShiftSceneName = "Jurgen-DayShift";
    public const string NightShiftSceneName = "Jurgen-Office";

    private static AllLevelData AllData {
        get {
            if (dataCache == null) {
                dataCache = Resources.Load<AllLevelData>("AllLevelData");
            }
            return dataCache;
        }
    }
    private static AllLevelData dataCache;

    public static void LoadNextLevel() {
        switch (SceneManager.GetActiveScene().name) {
            case MainMenuSceneName:
                
                //TODO: Read the saved data and load the next scene
                
                break;
            case DayShiftSceneName:
                LoadLevel(DayShiftData.levelIndex + 1);
                break;
            case NightShiftSceneName:
                LoadLevel(NightShiftData.levelIndex + 1);
                break;
        }
    }

    public static void LoadLevel(int index) {
        if (index < 0 || index >= AllData.levels.Count) {
            Debug.LogError("Invalid level index: " + index);
            return;
        }

        LevelData levelData = AllData.levels[index];
        if (levelData is DayShiftData dayData) {
            DayShiftData = dayData;
            SceneManager.LoadScene(DayShiftSceneName);
        } else if (levelData is NightShiftData nightData) {
            NightShiftData = nightData;
            SceneManager.LoadScene(NightShiftSceneName);
        } else {
            Debug.LogError("Unknown level data type at index: " + index);
        }
    }
}