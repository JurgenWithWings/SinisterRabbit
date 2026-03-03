using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOfficeController : MonoBehaviour {
    public enum State { Center, Left, Right, Top, Bottom, }

    [Serializable] public struct OfficeState {
        public State state;
        public Transform transform;
    }

    public bool IsBusy => isMoving || isFlipping;
    public bool isMoving;
    public bool isFlipping;

    [Header("Snap Points")] 
    [SerializeField] private float transitionDuration = 0.8f;
    [SerializeField] private List<OfficeState> officeStates = new(new [] {
        new OfficeState { state = State.Center },
        new OfficeState { state = State.Left },
        new OfficeState { state = State.Right },
        new OfficeState { state = State.Top },
        new OfficeState { state = State.Bottom },
    });

    [Header("Edge Zone Size")]
    [SerializeField, Range(0f, 0.5f)] private float sideEdgeZoneSize = 0.15f;
    [SerializeField, Range(0f, 0.5f)] private float vertEdgeZoneSize = 0.10f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f;
    
    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CameraSystem cameraSystem;
    [SerializeField] private OfficeInteractionController interactionController;

    // State Changing
    private State currentState = State.Center;
    private float elapsedAnimTime;
    private State previousState;

    private bool edgeLocked = true; // prevents overshooting move back to center

    private void Awake() {
        if (inputManager == null) {
            inputManager = GetComponent<InputManager>();
        }
        
        if (cameraSystem == null) {
            cameraSystem = GetComponent<CameraSystem>();
        }
        cameraSystem.Init(this);

        if (interactionController == null) {
            interactionController = GetComponent<OfficeInteractionController>();
        }
        interactionController.Init(this);
    }
    
    void Start() {
        transform.position = officeStates[0].transform.position;
        transform.rotation = officeStates[0].transform.rotation;
        
        inputManager.OfficeCameraSystem.Event += OnOfficeCams;
    }

    private void OnDestroy() {
        inputManager.OfficeCameraSystem.Event -= OnOfficeCams;
    }

    private void OnOfficeCams(InputEvent<bool> input) {
        if (input.Context.performed && !IsBusy) {
            cameraSystem.ToggleCams();
        }
    }

    void Update() {
        if (!IsBusy && !cameraSystem.IsOpen) {
            HandleTransitions();
        }

        if (isMoving) {
            SmoothMove();
        }
    }

    void HandleTransitions() {
        float mouseX = inputManager.OfficeMouse.Value.x / Screen.width;
        float mouseY = inputManager.OfficeMouse.Value.y / Screen.height;
        
        print($"mouseX: {mouseX}, mouseY: {mouseY}");

        bool inLeftEdge = mouseX < sideEdgeZoneSize;
        bool inRightEdge = mouseX > 1f - sideEdgeZoneSize;
        
        bool inBottomEdge = mouseY < vertEdgeZoneSize;
        bool inTopEdge = mouseY > 1f - vertEdgeZoneSize;

        if (!edgeLocked) {
            switch (currentState) {
                case State.Center:
                    if (inLeftEdge) {
                        SetState(State.Left);
                    }
                    else if (inRightEdge) {
                        SetState(State.Right);
                    }
                    else if (inTopEdge) {
                        SetState(State.Top);
                    }
                    else if (inBottomEdge) {
                        SetState(State.Bottom);
                    }
                    break;

                case State.Left:
                    if (inRightEdge) {
                        SetState(State.Center);
                    }
                    break;

                case State.Right:
                    if (inLeftEdge) {
                        SetState(State.Center);
                    }
                    break;
                
                case State.Top:
                case State.Bottom:
                    if (inBottomEdge) {
                        SetState(State.Center);
                    }
                    break;
            }
        }

        // Unlock mouse once it has left edge zones.
        if (!isFlipping && !inLeftEdge && !inRightEdge && !inTopEdge && !inBottomEdge) {
            edgeLocked = false;
        }
    }

    void SetState(State newState) {
        previousState = currentState;
        currentState = newState;
        
        edgeLocked = true;
        
        isMoving = true;
    }

    void SmoothMove() {
        float t = elapsedAnimTime / transitionDuration;
        
        t = Mathf.Clamp01(t);
        t = t * t * (3f - 2f * t);
        
        transform.position = Vector3.Lerp(officeStates[(int)previousState].transform.position, officeStates[(int)currentState].transform.position, t);

        transform.rotation = Quaternion.Lerp(officeStates[(int)previousState].transform.rotation, officeStates[(int)currentState].transform.rotation, t);
        
        elapsedAnimTime += Time.deltaTime;

        print(t);
        
        if (t >= 1) {
            isMoving = false;
            elapsedAnimTime = 0f;
        }
    }
}