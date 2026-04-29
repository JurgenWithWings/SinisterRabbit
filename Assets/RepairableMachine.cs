using UnityEngine;
using UnityEngine.VFX;

public class RepairableMachine : MonoBehaviour, IInteractable {
    [SerializeField] private VisualEffect[] brokenEffects;
    [SerializeField] private VisualEffect repairedEffect;

    private bool broken;

    public void SetBrokenState(bool state) {
        foreach (VisualEffect effect in brokenEffects) {
            if (state) {
                effect.Play();
            }
            else {
                effect.Stop();
            }
        }
        broken = state;
    }
    
    public void OnHoverEnter() { }
    public void OnHoverHold(float duration) { }
    public void OnHoverExit() { }

    public void OnInteractStart() {
        SetBrokenState(false);
        repairedEffect.enabled = true;
    }
    
    public void OnInteractHold(float duration) { }
    public void OnInteractEnd() { }
}