using System;
using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    [SerializeField] private CamCanvasController camCanvas;
    [Space]
    [SerializeField] private SecurityCamera[] cameras;
    [Space]
    [SerializeField] private Camera bootUpCam;
    [SerializeField] private Camera officeCamera;
    [SerializeField] private Animation cameraAnimator;
    [SerializeField] private float cameraFlipSpeed = 8f;
    
    private int currentIndex;
    
    public bool IsOpen { get; private set; }
    
    public event Action OnCamsOpen;
    public event Action OnCamsClosed;
    
    private PlayerOfficeController owningController;
    public void Init(PlayerOfficeController controller) {
        owningController = controller;
        
        owningController.OnStateChange += OnStateChange;
    }
    
    private void Start() {
        DisableAllSecurityCameras();

        StartCoroutine(CamsBootUp());
        
        camCanvas.OnButtonPressed += CamCanvasOnOnButtonPressed;
        
        GameOverManager.OnGameOver += OnGameOver;
    }

    private void OnGameOver() {
        cameraAnimator.Play("CloseCam");
    }

    private void OnDestroy() {
        camCanvas.OnButtonPressed -= CamCanvasOnOnButtonPressed;
        owningController.OnStateChange -= OnStateChange;
        
        GameOverManager.OnGameOver -= OnGameOver;
    }

    private void OnStateChange(PlayerOfficeController.State newState, PlayerOfficeController.State oldState) {
        if (newState == PlayerOfficeController.State.Camera || oldState == PlayerOfficeController.State.Camera) {
            ToggleCams();
        }
    }

    public bool ToggleCams() {
        if (!GameOverManager.Instance.IsGameOver && camFlipCoroutine == null) {
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
        }
        else {
            cameraAnimator.Play("CloseCam");
        }

        while (cameraAnimator.isPlaying) {
            yield return null;
        }
        
        if (IsOpen) {
            OnCamsOpen?.Invoke();
        }
        else {
            OnCamsClosed?.Invoke();
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

    private IEnumerator CamsBootUp() {
        bootUpCam.enabled = true;
        camCanvas.ToggleCanvas(false);
        
        Animation anim = bootUpCam.GetComponent<Animation>();
        anim.Play("BootUp");
        
        yield return new WaitForSeconds(anim.clip.length);
        
        bootUpCam.enabled = false;
        camCanvas.ToggleCanvas(true);
        camCanvas.SetCamera(cameras[currentIndex]);
        
        cameras[currentIndex].cam.enabled = true;
    }
}