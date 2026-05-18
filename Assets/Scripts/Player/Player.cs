using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour {
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private float sensitivity = 0.3f;
    [Space]
    [SerializeField] private CameraSpring cameraSpring;
    [SerializeField] private CameraLean cameraLean;
    [Space]
    [SerializeField] private Volume volume;
    [SerializeField] private StanceVignette stanceVignette;
    [Space]
    [SerializeField] private CameraFovSpeed cameraFovSpeed;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        
        InputManager.Instance.EnableActionMap(InputMap.Player);
        
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.CameraTarget);
        
        cameraSpring.Initialize();
        cameraLean.Initialize();
        
        stanceVignette.Initialize(volume.profile);
        
        cameraFovSpeed.Initialize();
    }

    private void Update() {
        if (InputManager.Instance.PlayerPause.Triggered) {
            PauseScreen.OnPause?.Invoke();
        }
        if (Time.timeScale == 0) {
            return;
        }
        
        CameraInput cameraInput = new() {
            Look = InputManager.Instance.PlayerLook.Value * sensitivity,
        };
        playerCamera.UpdateRotation(cameraInput);

        CharacterInput characterInput = new() {
            Rotation       = playerCamera.transform.rotation,
            Move         = InputManager.Instance.PlayerMove,
            Jump           = InputManager.Instance.PlayerJump.Triggered,
            JumpSustain  = InputManager.Instance.PlayerJump,
            Crouch         = InputManager.Instance.PlayerCrouch.Value 
                ? CrouchInput.Held 
                : CrouchInput.NotHeld,
            Dash           = InputManager.Instance.PlayerDash.Triggered,
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(Time.deltaTime);
        
        cameraSpring.UpdateSpring(Time.deltaTime, playerCharacter.CameraTarget.up);
        cameraLean.UpdateLean(
            deltaTime: Time.deltaTime,
            sliding: playerCharacter.State.Stance is Stance.Slide,
            acceleration: playerCharacter.State.Acceleration,
            up: playerCharacter.CameraTarget.up
        );

        #if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame) {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit)) {
                Teleport(hit.point);
            }
        }
        #endif
    }

    private void LateUpdate() {
        playerCamera.UpdatePosition(playerCharacter.CameraTarget);
        
        stanceVignette.UpdateVignette(Time.deltaTime, playerCharacter.State.Stance);
        
        cameraFovSpeed.UpdateFoV(Time.deltaTime, playerCharacter.State.Velocity.magnitude);
    }

    public void Teleport(Vector3 position) {
        playerCharacter.SetPosition(position);
    }
}
