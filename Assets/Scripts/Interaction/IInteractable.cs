using System;
using UnityEngine;

public enum PointerType {
    None,
    Open,
    Closed,
    Point,
}

[Serializable] public struct InteractionInfo {
    public string interactionText;
    public PointerType pointerType;
}

public interface IInteractable
{
    InteractionInfo GetInteractionInfo();
    
    /// <summary>
    /// Called when the mouse starts to hover over this object.
    /// </summary>
    void OnHoverEnter();
    
    /// <summary>
    /// Called on Update when the mouse is hovering over this object.
    /// </summary>
    /// <param name="duration">How long the mouse has been hovering over this object.</param>
    void OnHoverHold(float duration);
    
    /// <summary>
    /// Called when the mouse is no longer hovering over this object.
    /// </summary>
    void OnHoverExit();
    
    /// <summary>
    /// Called when the mouse has started interacting with this object.
    /// </summary>
    void OnInteractStart();
    
    /// <summary>
    /// Called on Update when the mouse is currently interacting with this object.
    /// </summary>
    /// <param name="duration">How long the mouse has been interacting with this object.</param>
    void OnInteractHold(float duration);
    
    /// <summary>
    /// Called when the mouse is no longer interacting with this object.
    /// </summary>
    void OnInteractEnd();
}
