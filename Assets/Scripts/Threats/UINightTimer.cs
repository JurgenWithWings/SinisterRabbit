using System;
using TMPro;
using UnityEngine;

public class UINightTimer : MonoBehaviour {
    [SerializeField] private TMP_Text timerText;

    private int prevHour;

    public static Action<float> OnTimerUpdate;
    public static event Action<int> OnHourPassed;

    private void Awake() {
        OnTimerUpdate += UpdateTimer;
    }
    
    private void OnDestroy() {
        OnTimerUpdate -= UpdateTimer;
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
}