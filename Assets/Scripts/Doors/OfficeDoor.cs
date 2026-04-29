using System;
using System.Collections;
using UnityEngine;

public class OfficeDoor : MonoBehaviour, IInteractable {
    [SerializeField] private InteractionInfo interactionInfo;
    [Header("Rotation Settings")]
    [SerializeField] private float openAngle = -20f;   // Slightly open
    [SerializeField] private float closedAngle = 0f;   // Fully closed
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float fullOpenAngle = 85f;

    private float targetAngle;
    private float currentAngle;

    private int lockCounter;
    private bool IsLocked => lockCounter > 0;
    public bool IsOpen { get; private set; } = true;
    
    public event Action OnDoorOpen;
    public event Action OnDoorClosed;
    

    private void Start() {
        currentAngle = openAngle;
        targetAngle = openAngle;
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    private void Update() {
        SmoothRotate();
    }

    private void SmoothRotate() {
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotateSpeed);
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    public void SetDoorLocked(bool locked) {
        lockCounter += locked ? 1 : -1;
    }

    public void FullOpenDoor(float holdDuration) {
        if (fullOpenCoroutine != null) {
            fullOpenTime += holdDuration;
        }
        else {
            fullOpenTime = holdDuration;
            fullOpenCoroutine = StartCoroutine(FullOpen());
        }
    }

    private float fullOpenTime;
    private Coroutine fullOpenCoroutine;

    private IEnumerator FullOpen() {
        SetDoorLocked(true);
        targetAngle = fullOpenAngle;

        while (fullOpenTime > 0) {
            fullOpenTime -= Time.deltaTime;
            yield return null;
        }
        
        targetAngle = openAngle;
        SetDoorLocked(false);
        fullOpenTime = 0;
        fullOpenCoroutine = null;
    }
    
    
    // IInteractable Implementation
    public InteractionInfo GetInteractionInfo() => interactionInfo;
    public void OnHoverEnter() { }
    public void OnHoverHold(float duration) { }

    public void OnHoverExit() {
        if (IsLocked) return;
        IsOpen = true;
        OnDoorOpen?.Invoke();
        targetAngle = openAngle;
    }

    public void OnInteractStart() {
        if (IsLocked) return;
        IsOpen = false;
        OnDoorClosed?.Invoke();
        targetAngle = closedAngle;
    }

    public void OnInteractHold(float duration) { }

    public void OnInteractEnd() {
        if (IsLocked) return;
        IsOpen = true;
        OnDoorOpen?.Invoke();
        targetAngle = openAngle;
    }
}
