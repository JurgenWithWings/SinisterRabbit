using System;
using UnityEngine;
using UnityEngine.VFX;

public class RepairableMachine : MonoBehaviour, IInteractable {
    [SerializeField] private string repairedInteractionText;
    [SerializeField] private string brokenInteractionText;
    [SerializeField] private VisualEffect[] brokenEffects;
    [SerializeField] private VisualEffect repairedEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioArray repairedSound;
    [SerializeField] private AudioSource loopSource;

    [SerializeField] private bool broken;
    public bool Broken => broken;
    
    private InteractionInfo interactionInfo;
    
    public event Action OnRepairedMachine;

    private void Start() {
        SetBrokenState(broken);
        interactionInfo.pointerType = PointerType.Open;
    }
    
    public void SetBrokenState(bool state) {
        foreach (VisualEffect effect in brokenEffects) {
            if (state) {
                effect.Play();
                loopSource.Stop();
            }
            else {
                effect.Stop();
                loopSource.Play();
            }
        }
        broken = state;

        if (broken) {
            interactionInfo.interactionText = brokenInteractionText;
        }
        else {
            interactionInfo.interactionText = repairedInteractionText;
            OnRepairedMachine?.Invoke();
        }
    }

    public InteractionInfo GetInteractionInfo() => interactionInfo;
    
    public void OnHoverEnter() { }
    public void OnHoverHold(float duration) { }
    public void OnHoverExit() { }

    public void OnInteractStart() {
        if (broken) {
            SetBrokenState(false);
            repairedEffect.enabled = true;
            repairedSound.PlayRandomSound(audioSource);
        }
    }

    public void OnInteractHold(float duration) { }
    public void OnInteractEnd() { }
}