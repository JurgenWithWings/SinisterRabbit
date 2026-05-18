using System;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Chicken : MonoBehaviour, IInteractable {
    [SerializeField] private InteractionInfo interactionInfo;
    [SerializeField] private GameObject chicken;
    [SerializeField] private VisualEffect featherEffect;
    [Space]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioLoopSource;
    [SerializeField] private AudioArray clickedSounds;
    
    public event Action<Chicken> OnChickenClicked;

    private void Awake() {
        featherEffect.gameObject.SetActive(false);
        featherEffect.Stop();
    }

    public void Activate() {
        chicken.SetActive(true);
        audioLoopSource.time = Random.Range(0, audioLoopSource.clip.length);
        audioLoopSource.Play();
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
        clickedSounds.PlayRandomSound(audioSource);
        audioLoopSource.Stop();
    }

    public void OnInteractHold(float duration) { }
    public void OnInteractEnd() { }
}