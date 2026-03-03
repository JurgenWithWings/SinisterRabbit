using UnityEngine;

public class PlayerOfficeController : MonoBehaviour {
    public enum CameraState { Center, Left, Right, Top, Bottom, }

    public static bool IsBusy = false;

    [Header("Snap Points")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;

    [Header("Edge Zone Size")]
    [SerializeField, Range(0f, 0.5f)] private float sideEdgeZoneSize = 0.15f;
    [SerializeField, Range(0f, 0.5f)] private float vertEdgeZoneSize = 0.10f;

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
        if (!IsBusy) {
            HandleTransitions();
        }
        SmoothMove();
    }

    void HandleTransitions() {
        float mouseX = Input.mousePosition.x / Screen.width;
        float mouseY = Input.mousePosition.y / Screen.height;

        bool inLeftEdge = mouseX < sideEdgeZoneSize;
        bool inRightEdge = mouseX > 1f - sideEdgeZoneSize;
        
        bool inBottomEdge = mouseY < vertEdgeZoneSize;
        bool inTopEdge = mouseY > 1f - vertEdgeZoneSize;

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
                    else if (inTopEdge) {
                        SetState(CameraState.Top, topPoint);
                        edgeLocked = true;
                    }
                    else if (inBottomEdge) {
                        SetState(CameraState.Bottom, bottomPoint);
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
                
                case CameraState.Top:
                    if (inBottomEdge) {
                        SetState(CameraState.Center, centerPoint);
                        edgeLocked = true;
                    }
                    break;
                
                case CameraState.Bottom:
                    if (inBottomEdge ) {
                        SetState(CameraState.Center, centerPoint);
                        edgeLocked = true;
                    }
                    break;
            }
        }

        // Unlock mouse once it has left edge zones.
        if (!inLeftEdge && !inRightEdge && !inTopEdge && !inBottomEdge) {
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