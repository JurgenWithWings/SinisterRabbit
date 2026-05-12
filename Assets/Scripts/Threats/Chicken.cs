using System;
using UnityEngine;

public class Chicken : MonoBehaviour, IInteractable {
    [SerializeField] private InteractionInfo interactionInfo;
    public event Action<Chicken> OnChickenClicked;

    public InteractionInfo GetInteractionInfo() => interactionInfo;
    public void OnHoverEnter() { }
    public void OnHoverHold(float duration) { }
    public void OnHoverExit() { }

    public void OnInteractStart() {
        OnChickenClicked?.Invoke(this);
        gameObject.SetActive(false);
    }

    public void OnInteractHold(float duration) { }
    public void OnInteractEnd() { }
}