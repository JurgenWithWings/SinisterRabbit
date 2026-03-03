using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable] public struct CamButtonPair {
    public SecurityCamera cam;
    public Button button;
}

public class CamCanvasController : MonoBehaviour {
    [SerializeField] private TMP_Text camName;

    [SerializeField] private List<CamButtonPair> camButtons = new();

    private Canvas canvas;
    
    public event Action<SecurityCamera> OnButtonPressed;

    private void Start() {
        canvas = GetComponent<Canvas>();
        
        SetCamera(null);

        foreach (CamButtonPair pair in camButtons) {
            pair.button.onClick.AddListener(() => SetCamera(pair.cam));
        }
    }

    private void OnDestroy() {
        foreach (CamButtonPair pair in camButtons) {
            pair.button.onClick.RemoveAllListeners();
        }
    }

    public void ToggleCanvas(bool enabled) {
        //canvas.enabled = enabled;
    }

    public void SetCamera(SecurityCamera cam) {
        if (cam == null) {
            return;
        }
        
        OnButtonPressed?.Invoke(cam);
        
        camName.text = cam.camName;
    }

    public void ButtonPressed() {
        Debug.Log("Button pressed");
    }
}
