using System;
using UnityEngine;

public class SecurityCamera : MonoBehaviour {
    public string camName;
    public Camera cam;

    public event Action<string> OnOfficeButton;
    
    private void Awake() {
        cam = GetComponent<Camera>();
    }
    
    public void OnButtonPressed(string key) {
        OnOfficeButton?.Invoke(key);
    }
}