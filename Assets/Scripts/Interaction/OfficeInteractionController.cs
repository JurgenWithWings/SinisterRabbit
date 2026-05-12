using System;
using UnityEngine;

public class OfficeInteractionController : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 15f;

    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Texture2D holdCursor;
    
    private IInteractable currentInteractable;

    private float interactableHoverDuration;
    private float interactableInteractDuration;

    private InputEvent<bool> inputBuffer;
    
    public void Start(){
        InputManager.Instance.OfficeLeftClick.Event += HandleInput;
    }

    private void OnDestroy() {
        InputManager.Instance.OfficeLeftClick.Event -= HandleInput;
    }

    void Update() {
        HandleRaycast();
        HandleHoldInput();

        currentInteractable?.OnHoverHold(interactableHoverDuration);

        interactableHoverDuration += Time.deltaTime;
        interactableInteractDuration += Time.deltaTime;
        
        // Cursor State
        if (currentInteractable == null) {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }
        else {
            if (inputBuffer.Triggered) {
                Cursor.SetCursor(holdCursor, Vector2.zero, CursorMode.Auto);
            }
            else {
                Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    void HandleRaycast() {
        Ray ray = playerCamera.ScreenPointToRay(InputManager.Instance.OfficeMouse.Value);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)) {
            IInteractable interactable;
            if (hit.collider.attachedRigidbody != null) {
                interactable = hit.collider.attachedRigidbody.GetComponent<IInteractable>();
            }
            else {
                interactable = hit.collider.GetComponent<IInteractable>();
            }

            if (interactable != currentInteractable) {
                ClearCurrent();
                SetCurrent(interactable);
            }
        }
        else {
            ClearCurrent();
        }
    }

    private void HandleInput(InputEvent<bool> input) {
        inputBuffer = input;
        if (currentInteractable == null) return;

        if (input.Triggered) {
            currentInteractable.OnInteractStart();
            interactableInteractDuration = 0f;
        }
        
        if (input.Context.canceled) {
            currentInteractable.OnInteractEnd();
        }
    }

    private void HandleHoldInput() {
        if (currentInteractable == null) return;
        
        if (InputManager.Instance.OfficeLeftClick.Value) {
            currentInteractable.OnInteractHold(interactableInteractDuration);
        }
    }

    void SetCurrent(IInteractable interactable) {
        currentInteractable = interactable;
        currentInteractable?.OnHoverEnter();
        interactableHoverDuration = 0f;
    }

    void ClearCurrent() {
        if (currentInteractable != null) {
            currentInteractable.OnHoverExit();
            currentInteractable = null;
        }
    }
}
