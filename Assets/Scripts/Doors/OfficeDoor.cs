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
    [Space]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private float targetAngle;
    private float currentAngle;

    public float TimeClosed { get; private set; }
    
    private int lockCounter;
    private bool IsLocked => lockCounter > 0;
    public bool IsOpen { get; private set; } = true;

    private void Start() {
        currentAngle = openAngle;
        targetAngle = openAngle;
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }

    private void Update() {
        SmoothRotate();
    }

    private void SmoothRotate() {
        if (Mathf.Approximately(currentAngle, targetAngle)) {
            transform.localRotation = Quaternion.Euler(0f, targetAngle, 0f);
            return;
        }

        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        float step = rotateSpeed * Time.deltaTime;
        if (Mathf.Abs(angleDifference) <= step) {
            currentAngle = targetAngle;
        }
        else {
            currentAngle += Mathf.Sign(angleDifference) * step;
        }
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
        audioSource.clip = openSound;
        audioSource.Play();
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
        if (!IsOpen) {
            audioSource.clip = openSound;
            audioSource.Stop();
            audioSource.Play();
        }
        IsOpen = true;
        targetAngle = openAngle;
    }

    public void OnInteractStart() {
        if (IsLocked) return;
        IsOpen = false;
        targetAngle = closedAngle;
        audioSource.clip = closeSound;
        audioSource.Stop();
        audioSource.Play();
    }

    public void OnInteractHold(float duration) {
        TimeClosed = duration;
    }

    public void OnInteractEnd() {
        if (IsLocked) return;
        IsOpen = true;
        targetAngle = openAngle;
        TimeClosed = 0f;
        audioSource.clip = openSound;
        audioSource.Stop();
        audioSource.Play();
    }
}
