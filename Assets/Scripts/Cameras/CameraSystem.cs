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
    private bool inCameraMode = false;
    
    private float camTargetRotation;

    private void Start() {
        cameras = FindObjectsOfType<SecurityCamera>(true);
        DisableAllSecurityCameras();
        
        camCanvas.OnButtonPressed += CamCanvasOnOnButtonPressed;
    }

    private void OnDestroy() {
        camCanvas.OnButtonPressed -= CamCanvasOnOnButtonPressed;
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

    void SmoothMove() {
        cameraAnimator.transform.rotation = Quaternion.Lerp(cameraAnimator.transform.rotation, 
            Quaternion.Euler(new Vector3(camTargetRotation, cameraAnimator.transform.rotation.eulerAngles.y, 
                cameraAnimator.transform.rotation.eulerAngles.z)), Time.deltaTime * cameraFlipSpeed);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.S)) {
            if (camFlipCoroutine == null) {
                camFlipCoroutine = StartCoroutine(ToggleCameraMode());
            }
        }
        
        //SmoothMove();
    }

    private Coroutine camFlipCoroutine;
    private IEnumerator ToggleCameraMode() {
        inCameraMode = !inCameraMode;
        PlayerOfficeController.IsBusy = inCameraMode;

        if (inCameraMode) {
            camTargetRotation = -90f;
            cameraAnimator.Play("OpenCam");
            while (cameraAnimator.isPlaying) {
                print("Waiting...");
                yield return null;
            }
            cameras[currentIndex].cam.enabled = true;
            camCanvas.SetCamera(cameras[currentIndex]);
            camCanvas.ToggleCanvas(true);
        }
        else {
            cameras[currentIndex].cam.enabled = false;
            camTargetRotation = 0f;
            cameraAnimator.Play("CloseCam");
            camCanvas.SetCamera(null);
            camCanvas.ToggleCanvas(false);
        }

        yield return null;
        
        camFlipCoroutine = null;
    }

    void DisableAllSecurityCameras() {
        foreach (var cam in cameras) {
            cam.cam.enabled = false;
        }
    }
}