using UnityEngine;

public class OfficeDoor : MonoBehaviour, IInteractable {
    [Header("Rotation Settings")]
    [SerializeField] private float openAngle = -20f;   // Slightly open
    [SerializeField] private float closedAngle = 0f;   // Fully closed
    [SerializeField] private float rotateSpeed = 5f;

    private float targetAngle;
    private float currentAngle;

    private bool isHeld;

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

    
    // IInteractable Implementation
    public void OnHoverEnter() { }

    public void OnHoverHold(float duration) { }

    public void OnHoverExit() {
        isHeld = false;
        targetAngle = openAngle;
    }

    public void OnInteractStart() {
        isHeld = true;
        targetAngle = closedAngle;
    }

    public void OnInteractHold(float duration) { }

    public void OnInteractEnd() {
        isHeld = false;
        targetAngle = openAngle;
    }
}
