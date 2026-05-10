using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInteraction : MonoBehaviour {
    [SerializeField] private Image pointer;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float textDuration = 0.4f;
    [Space] 
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite pointSprint;

    public static Action<InteractionInfo> SetInteractionInfo;

    private float promptTimer;

    private void Start() {
        SetInteractionInfo += SetInteraction;
    }

    private void Update() {
        promptTimer += Time.deltaTime;

        if (promptTimer >= textDuration) {
            text.text = "";
        }
    }

    private void SetInteraction(InteractionInfo info) {
        promptTimer = 0f;
        text.text = info.interactionText;
        switch (info.pointerType) {
            case PointerType.None:
                pointer.gameObject.SetActive(false);
                break;
            case PointerType.Open:
                pointer.gameObject.SetActive(true);
                pointer.sprite = openSprite;
                break;
            case PointerType.Closed:
                pointer.gameObject.SetActive(true);
                pointer.sprite = closedSprite;
                break;
            case PointerType.Point:
                pointer.gameObject.SetActive(true);
                pointer.sprite = pointSprint;
                break;
        }
    }
}