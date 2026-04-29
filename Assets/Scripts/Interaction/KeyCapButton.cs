using System;
using UnityEngine;
using UnityEngine.Events;

public class KeyCapButton : MonoBehaviour, IInteractable {
    [SerializeField] private InteractionInfo interactionInfo;
    
    public UnityEvent OnButtonPressed;

    private Animation animation;

    private void Awake() {
        animation = GetComponent<Animation>();
    }

    public InteractionInfo GetInteractionInfo() => interactionInfo;
    public void OnHoverEnter() { }
    public void OnHoverHold(float duration) { }
    public void OnHoverExit() { }

    public void OnInteractStart() {
        if (animation.isPlaying) {
            return;
        }
        animation.Play("KeyCapClick");
        OnButtonPressed.Invoke();
    }

    public void OnInteractHold(float duration) { }
    public void OnInteractEnd() { }
}
