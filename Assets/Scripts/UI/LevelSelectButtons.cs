using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButtons : MonoBehaviour {
    [SerializeField] private Button[] buttons;

    private void Start() {
        MainMenuButtons.OnResetProgress += SetButtonStates;

        SetButtonStates();
    }

    private void OnDestroy() {
        MainMenuButtons.OnResetProgress -= SetButtonStates;
    }

    private void SetButtonStates() {
        int highestLevel = LevelLoading.GetHighestLevelCompleted();
        
        for (int i = 0; i < buttons.Length; i++) {
            int index = i;

            buttons[index].interactable = index <= highestLevel + 1;
            
            buttons[index].onClick.AddListener(() => LevelLoading.LoadLevel(index));
        }
    }
}