using UnityEngine;

public class OfficeInteractionController : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 5f;

    private IInteractable currentInteractable;

    private float interactableHoverDuration;
    private float interactableInteractDuration;

    void Update() {
        HandleRaycast();
        HandleInput();

        currentInteractable?.OnHoverHold(interactableHoverDuration);

        interactableHoverDuration += Time.deltaTime;
        interactableInteractDuration += Time.deltaTime;
    }

    void HandleRaycast() {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

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

        if (Input.GetMouseButtonDown(0)) {
            currentInteractable.OnInteractStart();
            interactableInteractDuration = 0f;
        }

        if (Input.GetMouseButton(0)) {
            currentInteractable.OnInteractHold(interactableInteractDuration);
        }

        if (Input.GetMouseButtonUp(0)) {
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
