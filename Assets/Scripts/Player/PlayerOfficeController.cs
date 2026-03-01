using UnityEngine;

public class PlayerOfficeController : MonoBehaviour {
    public enum CameraState { Center, Left, Right }

    [Header("Snap Points")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    [Header("Edge Zone Size")]
    [SerializeField, Range(0f, 0.5f)] private float edgeZoneSize = 0.15f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f;

    private CameraState currentState = CameraState.Center;
    private Transform currentTarget;

    private bool edgeLocked; // prevents overshooting move back to center

    void Start() {
        currentTarget = centerPoint;
        transform.position = centerPoint.position;
        transform.rotation = centerPoint.rotation;
    }

    void Update() {
        HandleTransitions();
        SmoothMove();
    }

    void HandleTransitions() {
        float mouseX = Input.mousePosition.x / Screen.width;

        bool inLeftEdge = mouseX < edgeZoneSize;
        bool inRightEdge = mouseX > 1f - edgeZoneSize;

        if (!edgeLocked) {
            switch (currentState) {
                case CameraState.Center:
                    if (inLeftEdge) {
                        SetState(CameraState.Left, leftPoint);
                        edgeLocked = true;
                    }
                    else if (inRightEdge) {
                        SetState(CameraState.Right, rightPoint);
                        edgeLocked = true;
                    }
                    break;

                case CameraState.Left:
                    if (inRightEdge) {
                        SetState(CameraState.Center, centerPoint);
                        edgeLocked = true;
                    }
                    break;

                case CameraState.Right:
                    if (inLeftEdge) {
                        SetState(CameraState.Center, centerPoint);
                        edgeLocked = true;
                    }
                    break;
            }
        }

        // Unlock mouse once it has left edge zones.
        if (!inLeftEdge && !inRightEdge) {
            edgeLocked = false;
        }
    }

    void SetState(CameraState newState, Transform newTarget) {
        currentState = newState;
        currentTarget = newTarget;
    }

    void SmoothMove() {
        transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime * smoothSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, Time.deltaTime * smoothSpeed);
    }
}