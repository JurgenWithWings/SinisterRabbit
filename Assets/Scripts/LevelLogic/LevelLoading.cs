using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoading {
    public static DayShiftData DayShiftData;
    public static NightShiftData NightShiftData;
    
    public const string MainMenuSceneName = "MainMenu";
    public const string DayShiftSceneName = "Jurgen-DayShift";
    public const string NightShiftSceneName = "Jurgen-Office";

    public static string SaveFilePath = Application.dataPath + "saveData.csv";
    
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
                int levelIndex = GetHighestLevelCompleted() + 1;
                if (levelIndex < 0 || levelIndex >= AllData.levels.Count) {
                    Debug.LogError("No more levels to load. Highest completed index: " + (levelIndex - 1));
                    return;
                }
                LoadLevel(levelIndex);
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

    private static string[] fileContents;
    private static void LoadFile() {
        if (fileContents != null) return;
        if (File.Exists(SaveFilePath)) {
            string contents = File.ReadAllText(SaveFilePath);
            fileContents = contents.Split(",");
        }
        else {
            File.WriteAllText(SaveFilePath, "");
            fileContents = new[] {
                (-1).ToString(), // Highest level completed index (-1 means no levels completed)
            };
        }
    }
    
    public static void SaveProgress(int levelIndex) {
        LoadFile();
        fileContents[0] = levelIndex.ToString();
        File.WriteAllText(SaveFilePath, string.Join(",", fileContents));
    }
    
    public static int GetHighestLevelCompleted() {
        LoadFile();
        if (int.TryParse(fileContents[0], out int highestIndex)) {
            return highestIndex;
        }
        return -1; // Default to -1 if parsing fails
    }
    
    public static void ResetProgress() {
        fileContents = null;
        File.Delete(SaveFilePath);
    }
}