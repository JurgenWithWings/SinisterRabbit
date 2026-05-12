using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINightTimer : MonoBehaviour {
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Image powerFillImage;
    [SerializeField] private Gradient powerGradient;

    private int prevHour;

    public static Action<float> OnTimerUpdate;
    public static Action<float> OnPowerUpdate;
    
    public static event Action<int> OnHourPassed;

    private void Awake() {
        OnTimerUpdate += UpdateTimer;
        OnPowerUpdate += UpdatePower;
    }

    private void OnDestroy() {
        OnTimerUpdate -= UpdateTimer;
        OnPowerUpdate -= UpdatePower;
    }
    
    private void UpdateTimer(float currentTime) {
        float percentTime = currentTime / NightLogicManager.Instance.NightDuration;
        float  percentPerHour = 1f / 6f;
        int hour = Mathf.FloorToInt(percentTime / percentPerHour);
        timerText.text = hour == 0 ? "12am" : $"{hour}am";

        if (hour != prevHour) {
            OnHourPassed?.Invoke(hour);
            prevHour = hour;
        }
    }
    
    private void UpdatePower(float power) {
        powerSlider.value = Mathf.Ceil(power * powerSlider.maxValue / 100f);
        powerFillImage.color = powerGradient.Evaluate(power / 100f);
    }
}