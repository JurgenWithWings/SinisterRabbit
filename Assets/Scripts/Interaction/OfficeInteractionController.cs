using UnityEngine;

public class OfficeInteractionController : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 5f;

    private InputManager inputManager;
    
    private IInteractable currentInteractable;

    private float interactableHoverDuration;
    private float interactableInteractDuration;
    
    private PlayerOfficeController owningController;
    public void Init(PlayerOfficeController controller) {
        owningController = controller;
    }

    private void Awake() {
        inputManager = GetComponent<InputManager>();
    }
    
    void Update() {
        HandleRaycast();
        HandleInput();

        currentInteractable?.OnHoverHold(interactableHoverDuration);

        interactableHoverDuration += Time.deltaTime;
        interactableInteractDuration += Time.deltaTime;
    }

    void HandleRaycast() {
        Ray ray = playerCamera.ScreenPointToRay(inputManager.OfficeMouse.Value);

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

    void HandleInput() {
        if (currentInteractable == null) return;

        if (inputManager.OfficeLeftClick.Triggered) {
            currentInteractable.OnInteractStart();
            interactableInteractDuration = 0f;
        }

        if (inputManager.OfficeLeftClick.Value) {
            currentInteractable.OnInteractHold(interactableInteractDuration);
        }

        if (inputManager.OfficeLeftClick.Context.canceled) {
            currentInteractable.OnInteractEnd();
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
