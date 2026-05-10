using System;
using TMPro;
using UnityEngine;

public class UIDayTimer : MonoBehaviour {
    [SerializeField] private TMP_Text timer;
    private float currentTime;
    [Header("Time")]
    [SerializeField] private float panicStartTime = 30f;
    [SerializeField] private float panicEndTime = 10f;
    [Header("Scale")]
    [SerializeField] private float scaleSpeed = 5f;
    [SerializeField] private float scaleIntensity = 0.05f;
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationIntensity = 2f;
    [Header("Colour")]
    [SerializeField] private Color baseColour = Color.white;
    [SerializeField] private Color pulseColour = Color.red;
    [SerializeField] private float colourPulseSpeed = 5f;
    [SerializeField] private float colourSwapSpeed = 1f;

    public static Action<float> OnTimerUpdate;

    private void Awake() {
        OnTimerUpdate = TimerUpdate;
    }

    private void TimerUpdate(float time) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timer.text = timeSpan.ToString(@"mm\:ss");
        currentTime = time;
    }

    private void Update() {
        //Map panic start and end to a 0-1 range for intensity control.
        float intensityMultiplier = (panicStartTime - currentTime) / (panicStartTime - panicEndTime);
        intensityMultiplier = Mathf.Clamp01(intensityMultiplier);
        
        // Animate timer scale fluctuation to create a pulsing effect.
        float scale = 1f + (scaleIntensity * intensityMultiplier) * Mathf.Sin(Time.time * scaleSpeed);
        timer.transform.localScale = new Vector3(scale, scale, 1f);
        
        // Animate timer rotation to create a subtle shaking effect.
        float rotationAngle = rotationIntensity * Mathf.Sin(Time.time * rotationSpeed);
        timer.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle * intensityMultiplier);
        
        // Animate timer color fluctuation to create a glowing effect.
        float colorIntensity = colourSwapSpeed * Mathf.Sin(Time.time * colourPulseSpeed);
        timer.color = Color.Lerp(baseColour, pulseColour, colorIntensity * intensityMultiplier);
    }
}