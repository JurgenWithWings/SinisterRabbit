using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfficeUINavigationController : MonoBehaviour {
    public static Action<PlayerOfficeController.MouseRegion> OnStartStateChange;
    public static Action<List<PlayerOfficeController.AvailableTransition>> OnStateChange;
    
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button bottomButton;

    public void Awake() {
        OnStartStateChange += OnStartStateChanged;
        OnStateChange += OnStateChanged;
    }

    private void OnDestroy() {
        OnStartStateChange -= OnStartStateChanged;
        OnStateChange -= OnStateChanged;
    }

    private void DisableAllButtons() {
        leftButton.interactable = false;
        rightButton.interactable = false;
        bottomButton.interactable = false;
    }
    
    private void OnStartStateChanged(PlayerOfficeController.MouseRegion direction) {
        DisableAllButtons();

        switch (direction) {
            case PlayerOfficeController.MouseRegion.Left:
                leftButton.interactable = true; break;
                
            case PlayerOfficeController.MouseRegion.Right:
                rightButton.interactable = true; break;
                
            case PlayerOfficeController.MouseRegion.Bottom:
                bottomButton.interactable = true; break;
        }
    }

    private void OnStateChanged(List<PlayerOfficeController.AvailableTransition> transitions) {
        DisableAllButtons();

        foreach (PlayerOfficeController.AvailableTransition transition in transitions) {
            switch (transition.mouseRegion) {
                case PlayerOfficeController.MouseRegion.Left:
                    leftButton.interactable = true; break;
                
                case PlayerOfficeController.MouseRegion.Right:
                    rightButton.interactable = true; break;
                
                case PlayerOfficeController.MouseRegion.Bottom:
                    bottomButton.interactable = true; break;
            }
        }
    }
}
