using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CamCanvasController : MonoBehaviour {
    [SerializeField] private TMP_Text camName;

    [SerializeField] private List<KeyCapButton> camButtons = new();

    private Canvas canvas;
    
    public event Action<SecurityCamera> OnButtonPressed;

    private void Start() {
        canvas = GetComponent<Canvas>();
        
        SetCamera(null);

        SecurityCamera[] cams = FindObjectsOfType<SecurityCamera>();
        
        foreach (KeyCapButton button in camButtons) {
            foreach (SecurityCamera cam in cams) {
                if (cam.gameObject.name == button.name) {
                    button.OnButtonPressed.AddListener(() => SetCamera(cam));
                }
            }
        }
    }

    private void OnDestroy() {
        foreach (KeyCapButton button in camButtons) {
            button.OnButtonPressed.RemoveAllListeners();
        }
    }

    public void ToggleCanvas(bool enabled) {
        canvas.enabled = enabled;
    }

    public void SetCamera(SecurityCamera cam) {
        if (cam == null) {
            return;
        }
        
        OnButtonPressed?.Invoke(cam);
        
        camName.text = cam.gameObject.name;
    }
}
