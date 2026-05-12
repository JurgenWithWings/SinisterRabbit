using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using static PlayerControls;

public enum InputMap {
    Player,
    Office,
    UI
}

public enum ControlType {
    Keyboard,
    Controller
}

public struct InputEvent<T>{
    public T Value;
    public T RawValue;
    public bool Triggered;
    public bool preformedLastFrame;
    
    public event Action<InputEvent<T>> Event;
    
    public InputAction.CallbackContext Context;
    
    public void Invoke() {
        Event?.Invoke(this);
    }

    public void Set(T value, T rawValue, InputAction.CallbackContext context) {
        Triggered = !preformedLastFrame && context.performed;
        preformedLastFrame = context.performed;
        Value = value;
        RawValue = rawValue;
        Context = context;
    }
    
    public void SetAndInvoke(T value, T rawValue, InputAction.CallbackContext context) {
        Set(value, rawValue, context);
        Invoke();
    }

    public void ResetState() {
        Triggered = false;
    }

    public static implicit operator T(InputEvent<T> inputEvent) {
        return inputEvent.Value;
    }
}

public class InputManager : MonoBehaviour, IPlayerActions, IOfficeActions, IUIActions {
    public static InputManager Instance { get; private set; }

    public PlayerControls controls { get; private set; }
    
    [SerializeField] private InputMap defaultMap = InputMap.Office;

    private List<InputEvent<Type>> inputFields;

    public static event Action<ControlType> OnControlTypeChanged;
    public static ControlType CurrentControlDevice { get; private set; }

    private void Awake() {
        Instance = this;
        
        if (controls == null) {
            controls = new PlayerControls();
            
            controls.Player.SetCallbacks(this);
            controls.Office.SetCallbacks(this);
            controls.UI.SetCallbacks(this);
        }
    }

    private void Start() => StartCoroutine(EnablePlayerActions());
    
    private void OnDestroy() {
        controls.Disable();
    }

    private IEnumerator EnablePlayerActions() {
        SetUIModuleAsset();
        
        yield return null;
        
        EnableActionMap(defaultMap);
        controls.UI.Enable();
    }

    private void SetUIModuleAsset() {
        var eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem == null) {
            return;
        }

        var uiModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        if (uiModule == null) {
            return;
        }

        if (uiModule.actionsAsset != controls.asset) {
            uiModule.actionsAsset = controls.asset;
        }
    }

    public void EnableActionMap(InputMap map) {
        switch (map) {
            case InputMap.Player:
                controls.Player.Enable();
                break;
            case InputMap.Office:
                controls.Office.Enable();
                break;
            case InputMap.UI:
                controls.UI.Enable();
                break;
            default:
                controls.Enable();
                break;
        }
    }
    
    public void DisableActionMap(InputMap map) {
        switch (map) {
            case InputMap.Player:
                controls.Player.Disable();
                break;
            case InputMap.Office:
                controls.Office.Disable();
                break;
            case InputMap.UI:
                controls.UI.Disable();
                break;
            default:
                controls.Disable();
                break;
        }
    }

    private void GetCurrentInputDevice(InputAction.CallbackContext context) {
        ControlType controlType = CurrentControlDevice;
        switch (context.control.device.displayName) {
            case "Gamepad" or "Wireless Controller" or "VirtualMouse":
                CurrentControlDevice = ControlType.Controller; break;
            
            case "Mouse" or "Keyboard":
                CurrentControlDevice = ControlType.Keyboard; break;
            
            default: // Fallback to device types if name is not found.
                switch (context.control.device) {
                    case Gamepad:
                        CurrentControlDevice = ControlType.Controller; break;
                    
                    case Mouse:
                        CurrentControlDevice = ControlType.Keyboard; break;
                    
                    default:
                        CurrentControlDevice = ControlType.Keyboard; break;
                }
                break;
        }

        if (controlType != CurrentControlDevice) {
            OnControlTypeChanged?.Invoke(CurrentControlDevice);
        }
    }

    // All Inputs Must be added to here to reset properly
    private void LateUpdate() {
        // Player
        PlayerMove.ResetState();
        PlayerLook.ResetState();
        PlayerInteract.ResetState();
        PlayerJump.ResetState();
        PlayerSprint.ResetState();
        PlayerCrouch.ResetState();
        
        // Office
        OfficeMouse.ResetState();
        OfficeLeftClick.ResetState();
        OfficeCameraSystem.ResetState();
        
        // UI
    }

    public void DefaultHandle(InputAction.CallbackContext context) {
        GetCurrentInputDevice(context);
    }

    // Player
    public InputEvent<Vector2> PlayerMove;

    public void OnMove(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            Vector2 value = context.ReadValue<Vector2>();
            PlayerMove.SetAndInvoke(value, value, context);
        }
    }
    
    public InputEvent<Vector2> PlayerLook;
    public void OnLook(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            Vector2 value = context.ReadValue<Vector2>();
            PlayerLook.SetAndInvoke(value, value, context);
        }
    }
    
    public InputEvent<bool> PlayerInteract;
    public void OnInteract(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            PlayerInteract.SetAndInvoke(context.performed, context.performed, context);
        }
    }
    
    public InputEvent<bool> PlayerJump;
    public void OnJump(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            PlayerJump.SetAndInvoke(context.performed, context.performed, context);
        }
    }
    
    public InputEvent<bool> PlayerSprint;
    public void OnSprint(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            PlayerSprint.SetAndInvoke(context.performed, context.performed, context);
        }
    }

    public InputEvent<bool> PlayerCrouch;
    public void OnCrouch(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            PlayerCrouch.SetAndInvoke(context.performed, context.performed, context);
        }
    }
    
    
    // Office
    public InputEvent<Vector2> OfficeMouse;
    public void OnMouse(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            Vector2 value = context.ReadValue<Vector2>();
            OfficeMouse.SetAndInvoke(value, value, context);
        }
    }
    
    public InputEvent<bool> OfficeLeftClick;
    public void OnLeftClick(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            OfficeLeftClick.SetAndInvoke(context.performed, context.performed, context);
        }
    }

    public InputEvent<bool> OfficeCameraSystem;
    public void OnCameraSystem(InputAction.CallbackContext context) {
        DefaultHandle(context);
        if (context.performed || context.canceled) {
            OfficeCameraSystem.SetAndInvoke(context.performed, context.performed, context);
        }
    }
    
    
    // UI
    public void OnNavigate(InputAction.CallbackContext context) { }
    public void OnSubmit(InputAction.CallbackContext context) { }
    public void OnCancel(InputAction.CallbackContext context) { }
    public void OnPoint(InputAction.CallbackContext context) { }
    public void OnClick(InputAction.CallbackContext context) { }
    public void OnRightClick(InputAction.CallbackContext context) { }
    public void OnMiddleClick(InputAction.CallbackContext context) { }
    public void OnScrollWheel(InputAction.CallbackContext context) { }
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
}
