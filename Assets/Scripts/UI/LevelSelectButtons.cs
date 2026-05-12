using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButtons : MonoBehaviour {
    [SerializeField] private Button[] buttons;

    private void Start() {
        for (int i = 0; i < buttons.Length; i++) {
            int index = i;
            buttons[i].onClick.AddListener(() => LevelLoading.LoadLevel(index));
        }
    }
}