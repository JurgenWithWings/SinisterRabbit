using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
    public static LoadingScreen instance;
    
    [SerializeField] private Camera fallbackCam;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Slider slider;

    public static event Action OnFinishedLoading;
    
    public void Awake() {
        if (instance != null || instance == this) {
            Debug.LogError("More than one instance of LoadingScreen found!");
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        LevelLoading.LoadScene(Scene.MainMenu);
    }
    
    public void StartLoadingScreen(List<AsyncOperation> operations) {
        StartCoroutine(LoadLevelAsync(operations));
    }
    
    private IEnumerator LoadLevelAsync(List<AsyncOperation> operations) {
        fallbackCam.enabled = true;
        canvas.enabled = true;
        slider.value = 0f;
        Time.timeScale = 0;
        
        int doneCounter = 0;
        while (doneCounter < operations.Count) {
            doneCounter = 0;
            float progress = 0;
            foreach (AsyncOperation operation in operations) {
                if (operation.isDone) {
                    doneCounter++;
                }
                progress += operation.progress / 0.9f;
            }
            
            slider.value = progress / operations.Count;
            Debug.Log($"Progress: {progress} - {progress / operations.Count}");
            Debug.Log($"DoneCounter: {doneCounter} / {operations.Count}");
            yield return null;
        }
        
        slider.value = 1f;
        yield return new WaitForSecondsRealtime(1f); // Delay for better UX
        OnFinishedLoading?.Invoke();
        Time.timeScale = 1;
        canvas.enabled = false;
        fallbackCam.enabled = false;
    }
}
