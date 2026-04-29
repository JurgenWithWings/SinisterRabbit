using UnityEngine;

public class PlayerInteractionController : MonoBehaviour {
    [SerializeField] private float interactDistance = 7.5f;
    
    private IInteractable currentInteractable;

    private float interactableHoverDuration;
    private float interactableInteractDuration;
    
    public void Start() { 
        InputManager.Instance.PlayerInteract.Event += HandleInput;
    }

    private void OnDestroy() {
        InputManager.Instance.PlayerInteract.Event -= HandleInput;
    }

    void Update() {
        HandleRaycast();
        HandleHoldInput(InputManager.Instance.PlayerInteract);

        currentInteractable?.OnHoverHold(interactableHoverDuration);

        interactableHoverDuration += Time.deltaTime;
        interactableInteractDuration += Time.deltaTime;
    }

    private void LateUpdate() {
        InteractionInfo info = new();
        
        // Cursor State
        if (currentInteractable != null) {
            info.interactionText = currentInteractable.GetInteractionInfo().interactionText;
            info.pointerType = InputManager.Instance.PlayerInteract.Value ? PointerType.Open : PointerType.Closed;
        }

        UIInteraction.SetInteractionInfo?.Invoke(info);
    }

    void HandleRaycast() {
        if (Physics.Raycast(transform.position, transform.forward * interactDistance, out RaycastHit hit, interactDistance)) {
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