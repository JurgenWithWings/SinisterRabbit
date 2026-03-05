using System;
using UnityEngine;

public class OfficeInteractionController : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 5f;
    
    private IInteractable currentInteractable;

    private float interactableHoverDuration;
    private float interactableInteractDuration;
    
    private PlayerOfficeController owningController;
    public void Init(PlayerOfficeController controller) {
        owningController = controller;

        owningController.InputManager.OfficeLeftClick.Event += HandleInput;
    }

    private void OnDestroy() {
        owningController.InputManager.OfficeLeftClick.Event -= HandleInput;
    }

    void Update() {
        HandleRaycast();
        HandleHoldInput(owningController.InputManager.OfficeLeftClick);

        currentInteractable?.OnHoverHold(interactableHoverDuration);

        interactableHoverDuration += Time.deltaTime;
        interactableInteractDuration += Time.deltaTime;
    }

    void HandleRaycast() {
        Ray ray = playerCamera.ScreenPointToRay(owningController.InputManager.OfficeMouse.Value);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)) {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

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
        if (currentInteractable == null) return;

        if (input.Triggered) {
            currentInteractable.OnInteractStart();
            interactableInteractDuration = 0f;
        }
        
        if (input.Context.canceled) {
            currentInteractable.OnInteractEnd();
        }
    }

    private void HandleHoldInput(InputEvent<bool> input) {
        if (currentInteractable == null) return;
        
        if (input.Value) {
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
