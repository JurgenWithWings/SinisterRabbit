using System;
using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    [SerializeField] private CamCanvasController camCanvas;
    [Space]
    [SerializeField] private Camera officeCamera;
    [SerializeField] private Animation cameraAnimator;
    [SerializeField] private float cameraFlipSpeed = 8f;
    
    private SecurityCamera[] cameras;
    private int currentIndex = 0;

    public bool IsOpen { get; private set; }
    
    private PlayerOfficeController owningController;
    public void Init(PlayerOfficeController controller) {
        owningController = controller;
    }
    
    private void Start() {
        cameras = FindObjectsOfType<SecurityCamera>(true);
        DisableAllSecurityCameras();
        
        camCanvas.OnButtonPressed += CamCanvasOnOnButtonPressed;
    }

    private void OnDestroy() {
        camCanvas.OnButtonPressed -= CamCanvasOnOnButtonPressed;
    }
    
    public bool ToggleCams() {
        if (camFlipCoroutine == null) {
            camFlipCoroutine = StartCoroutine(ToggleCameraCoroutine());
            return true;
        }
        return false;
    }
    
    private Coroutine camFlipCoroutine;
    private IEnumerator ToggleCameraCoroutine() {
        IsOpen = !IsOpen;
        owningController.isFlipping = true;

        if (IsOpen) {
            cameraAnimator.Play("OpenCam");
            while (cameraAnimator.isPlaying) {
                yield return null;
            }
            cameras[currentIndex].cam.enabled = true;
            camCanvas.SetCamera(cameras[currentIndex]);
            camCanvas.ToggleCanvas(true);
        }
        else {
            cameras[currentIndex].cam.enabled = false;
            cameraAnimator.Play("CloseCam");
            camCanvas.SetCamera(null);
            camCanvas.ToggleCanvas(false);
        }
        
        owningController.isFlipping = false;
        camFlipCoroutine = null;
    }

    private void CamCanvasOnOnButtonPressed(SecurityCamera cam) {
        for (int i = 0; i < cameras.Length; i++) {
            if (cameras[i] == cam) {
                cameras[currentIndex].cam.enabled = false;
                
                currentIndex = i;
                
                cameras[currentIndex].cam.enabled = true;
                return;
            }
        }
    }

    void DisableAllSecurityCameras() {
        foreach (var cam in cameras) {
            cam.cam.enabled = false;
        }
    }
}