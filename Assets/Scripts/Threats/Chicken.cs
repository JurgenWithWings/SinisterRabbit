using System;
using UnityEngine;
using UnityEngine.VFX;

public class Chicken : MonoBehaviour, IInteractable {
    [SerializeField] private InteractionInfo interactionInfo;
    [SerializeField] private GameObject chicken;
    [SerializeField] private VisualEffect featherEffect;
    
    public event Action<Chicken> OnChickenClicked;

    private void Awake() {
        featherEffect.gameObject.SetActive(false);
        featherEffect.Stop();
    }

    public void Activate() {
        chicken.SetActive(true);
    }

    public InteractionInfo GetInteractionInfo() => interactionInfo;
    public void OnHoverEnter() { }
    public void OnHoverHold(float duration) { }
    public void OnHoverExit() { }

    public void OnInteractStart() {
        OnChickenClicked?.Invoke(this);
        chicken.SetActive(false);
        featherEffect.gameObject.SetActive(true);
        featherEffect.Play();
    }

    public void OnInteractHold(float duration) { }
    public void OnInteractEnd() { }
}