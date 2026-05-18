using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOfficeController : MonoBehaviour {
    public enum State { Center, Left, Right, Camera, GameOver, }
    
    public enum MouseRegion { Left, Right, Bottom }
    private bool[] mouseRegionStates = new bool[3] { false, false, false };

    [Serializable] public struct OfficeState {
        public State state;
        public Transform transform;
        public List<AvailableTransition> transitions;
    }

    [Serializable] public struct AvailableTransition {
        public MouseRegion mouseRegion;
        public State targetState;
    }
    
    [Serializable] public struct OfficeTransition {
        public State state1;
        public State state2;
        public float duration;
    }

    [HideInInspector] public bool IsBusy => isMoving || isFlipping || busyLevel > 0;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isFlipping;
    [HideInInspector] public int busyLevel;

    [Header("States and Transitions")]
    [SerializeField] private List<OfficeState> officeStates = new(new [] {
        new OfficeState { state = State.Center },
        new OfficeState { state = State.Left },
        new OfficeState { state = State.Right },
        new OfficeState { state = State.Camera },
    });
    [SerializeField] private List<OfficeTransition> stateTransitions = new();

    [Header("Edge Zone Size")]
    [SerializeField, Range(0f, 0.5f)] private float sideEdgeZoneSize = 0.12f;
    [SerializeField, Range(0f, 0.5f)] private float vertEdgeZoneSize = 0.10f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f;
    
    [Header("References")]
    [SerializeField] private CameraSystem cameraSystem;
    [SerializeField] private OfficeInteractionController interactionController;

    // State Changing
    private State currentState = State.Center;
    private float elapsedAnimTime;
    private float currentTransitionDuration;
    private State previousState;
    public event Action<State, State> OnStateChange; // New State, Old State

    private bool edgeLocked = true; // prevents overshooting move back to center

    private void Awake() {
        if (cameraSystem == null) {
            cameraSystem = GetComponent<CameraSystem>();
        }
        cameraSystem.Init(this);

        if (interactionController == null) {
            interactionController = GetComponent<OfficeInteractionController>();
        }
    }
    
    private void Start() {
        transform.position = officeStates[0].transform.position;
        transform.rotation = officeStates[0].transform.rotation;
        
        OfficeUINavigationController.OnStateChange?.Invoke(officeStates[(int)currentState].transitions);
        
        GameOverManager.OnGameOver += OnGameOver;
    }

    private void OnDestroy() {
        GameOverManager.OnGameOver -= OnGameOver;
    }

    private void OnGameOver() {
        SetState(State.GameOver, MouseRegion.Bottom);
    }

    void Update() {
        if (InputManager.Instance.OfficePause) {
            PauseScreen.OnPause?.Invoke();
        }

        if (Time.timeScale == 0) {
            return;
        }
        
        if (!IsBusy) {
            HandleTransitions();
        }

        if (isMoving) {
            SmoothMove();
        }
    }

    void HandleTransitions() {
        float mouseX = InputManager.Instance.OfficeMouse.Value.x / Screen.width;
        float mouseY = InputManager.Instance.OfficeMouse.Value.y / Screen.height;

        
        mouseRegionStates[(int)MouseRegion.Left] = mouseX < sideEdgeZoneSize;
        bool anyMouseRegion = mouseRegionStates[(int)MouseRegion.Left];
        
        mouseRegionStates[(int)MouseRegion.Right] = mouseX > 1f - sideEdgeZoneSize;
        anyMouseRegion = anyMouseRegion || mouseRegionStates[(int)MouseRegion.Right];
        
        mouseRegionStates[(int)MouseRegion.Bottom] = mouseY < vertEdgeZoneSize;
        anyMouseRegion = anyMouseRegion || mouseRegionStates[(int)MouseRegion.Bottom];

        
        // Check if current mouse region has a transition and move there
        if (!edgeLocked && anyMouseRegion) {
            foreach (AvailableTransition transition in officeStates[(int)currentState].transitions) {
                if (mouseRegionStates[(int)transition.mouseRegion]) {
                    SetState(transition.targetState, transition.mouseRegion);
                }
            }
        }

        // Unlock mouse once it has left edge zones.
        if (!isFlipping && !anyMouseRegion) {
            edgeLocked = false;
        }
    }

    void SetState(State newState, MouseRegion direction) {
        if (!TryGetTransition(currentState, newState, out OfficeTransition transition)) {
            Debug.LogWarning($"No transition with {currentState} and {newState}");
            return;
        }
        
        previousState = currentState;
        currentState = newState;
        
        currentTransitionDuration = transition.duration;
        
        edgeLocked = true;
        isMoving = true;
        OnStateChange?.Invoke(currentState, previousState);
        OfficeUINavigationController.OnStartStateChange?.Invoke(direction);
    }
    
    bool TryGetTransition(State a, State b, out OfficeTransition transition) {
        foreach (var t in stateTransitions) {
            if ((t.state1 == a && t.state2 == b) || (t.state2 == a && t.state1 == b)) {
                transition = t;
                return true;
            }
        }

        transition = default;
        return false;
    }

    void SmoothMove() {
        float t = elapsedAnimTime / currentTransitionDuration;
        t = Mathf.Clamp01(t);
        t = t * t * (3f - 2f * t);
        
        transform.position = Vector3.Lerp(officeStates[(int)previousState].transform.position, officeStates[(int)currentState].transform.position, t);
        transform.rotation = Quaternion.Lerp(officeStates[(int)previousState].transform.rotation, officeStates[(int)currentState].transform.rotation, t);
        
        elapsedAnimTime += Time.deltaTime;
        
        if (t >= 1) {
            isMoving = false;
            elapsedAnimTime = 0f;
            OfficeUINavigationController.OnStateChange?.Invoke(officeStates[(int)currentState].transitions);
        }
    }
}