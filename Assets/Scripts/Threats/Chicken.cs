using System;
using UnityEngine;

public class Chicken : MonoBehaviour, IInteractable {
    public event Action<Chicken> OnChickenClicked;
    
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