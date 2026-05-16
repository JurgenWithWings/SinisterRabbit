using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Level {
    MainMenu,
    DayShift,
    NightShift,
}

public static class LevelLoading {
    public static DayShiftData DayShiftData;
    public static NightShiftData NightShiftData;
    
    public const string LoadingScreenSceneName = "LoadingScreen";
    public const string MainMenuSceneName = "MainMenu";
    public const string FactoryGeoSceneName = "FactoryGeo";
    public const string DayShiftSceneName = "Jurgen-DayShift";
    public const string NightShiftSceneName = "Jurgen-Office";

    private static Level currentLevel = Level.MainMenu;

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

    public static void OverrideCurrentSceneTracker(Level level) {
        currentLevel = level;
    }
    
    public static void LoadNextLevel() {
        switch (currentLevel) {
            case Level.MainMenu:
                int levelIndex = GetHighestLevelCompleted() + 1;
                if (levelIndex < 0 || levelIndex >= AllData.levels.Count) {
                    Debug.LogError("No more levels to load. Highest completed index: " + (levelIndex - 1));
                    return;
                }
                LoadLevel(levelIndex);
                break;
            
            case Level.DayShift:
                LoadLevel(DayShiftData.levelIndex + 1);
                break;
            
            case Level.NightShift:
                LoadLevel(NightShiftData.levelIndex + 1);
                break;
        }
    }

    public static void ReloadLevel() {
        switch (currentLevel) {
            case Level.MainMenu:
                LoadScene(Level.MainMenu);
                break;
            
            case Level.DayShift:
                if (DayShiftData != null) {
                    LoadScene(Level.DayShift);
                } else {
                    Debug.LogError("No DayShift data to reload.");
                }
                break;
            
            case Level.NightShift:
                if (NightShiftData != null) {
                    LoadScene(Level.NightShift);
                } else {
                    Debug.LogError("No NightShift data to reload.");
                }
                break;
        }
    }

    /// <summary>
    /// Loads the level data asset and scene based on the level index in AllLevelData.
    /// </summary>
    /// <param name="index">The level index in AllLevelData.</param>
    public static void LoadLevel(int index) {
        if (index < 0 || index >= AllData.levels.Count) {
            Debug.LogError("Invalid level index: " + index);
            return;
        }

        if (index > AllData.levels.Count) {
            LoadScene(Level.MainMenu);
            return;
        }
        
        LevelData levelData = AllData.levels[index];
        if (levelData is DayShiftData dayData) {
            DayShiftData = dayData;
            LoadScene(Level.DayShift);
        } else if (levelData is NightShiftData nightData) {
            NightShiftData = nightData;
            LoadScene(Level.NightShift);
        } else {
            Debug.LogError("Unknown level data type at index: " + index);
        }
    }

    /// <summary>
    /// Load only the scene without changing the level data. Used for returning to main menu or reloading current level.
    /// </summary>
    /// <param name="level">The scene to load</param>
    public static void LoadScene(Level level) {
        LoadSceneWithLoadingScreen(level);
    }
    
    private static void LoadSceneWithLoadingScreen(Level level) {
        List<AsyncOperation> operations = new List<AsyncOperation>();
        switch (level) {
            case Level.MainMenu:
                currentLevel = Level.MainMenu;
                operations.Add(SceneManager.LoadSceneAsync(MainMenuSceneName, LoadSceneMode.Single));
                break;
            case Level.DayShift:
                currentLevel = Level.DayShift;
                operations.Add(SceneManager.LoadSceneAsync(FactoryGeoSceneName, LoadSceneMode.Single));
                operations.Add(SceneManager.LoadSceneAsync(DayShiftSceneName, LoadSceneMode.Additive));
                break;
            case Level.NightShift:
                currentLevel = Level.NightShift;
                operations.Add(SceneManager.LoadSceneAsync(FactoryGeoSceneName, LoadSceneMode.Single));
                operations.Add(SceneManager.LoadSceneAsync(NightShiftSceneName, LoadSceneMode.Additive));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }

        LoadingScreen.instance?.StartLoadingScreen(operations);
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