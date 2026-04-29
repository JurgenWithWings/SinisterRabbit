using UnityEngine;

public class PlayerInteractionController : MonoBehaviour {
    [SerializeField] private float interactDistance = 15f;
    [Space]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Texture2D holdCursor;
    
    private enum CursorState { Default, Hover, Hold }
    private CursorState cursorState = CursorState.Default;
    
    private IInteractable currentInteractable;

    private float interactableHoverDuration;
    private float interactableInteractDuration;
    
    private InputEvent<bool> inputBuffer;
    
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