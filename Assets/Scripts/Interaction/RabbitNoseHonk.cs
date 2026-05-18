using UnityEngine;

public class RabbitNoseHonk : MonoBehaviour, IInteractable {
    [SerializeField] private Collider collider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioArray honkSound;

    public void OnInteractStart() {
        honkSound.PlayRandomSound(audioSource);
    }
    
    
    private InteractionInfo info;
    public InteractionInfo GetInteractionInfo() => info;
    public void OnHoverEnter() { }
    public void OnHoverHold(float duration) { }
    public void OnHoverExit() { }
    public void OnInteractHold(float duration) { }
    public void OnInteractEnd() { }
}
