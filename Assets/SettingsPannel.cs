using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPannel : MonoBehaviour {
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider effectVolume;

    [SerializeField] private AudioMixer mixer;

    private void Awake() {
        masterVolume.onValueChanged.AddListener(SetMasterVolume);
        musicVolume.onValueChanged.AddListener(SetMusicVolume);
        effectVolume.onValueChanged.AddListener(SetEffectVolume);
    }

    private void OnDestroy() {
        masterVolume.onValueChanged.RemoveAllListeners();
        masterVolume.onValueChanged.RemoveAllListeners();
        effectVolume.onValueChanged.RemoveAllListeners();
    }

    private void OnEnable() {
        mixer.GetFloat("MasterVolume", out float master);
        masterVolume.value = DBToValue(master);
        mixer.GetFloat("MusicVolume", out float music);
        musicVolume.value = DBToValue(music);
        mixer.GetFloat("EffectVolume", out float effect);
        effectVolume.value = DBToValue(effect);
    }

    private void SetMasterVolume(float value) {
        mixer.SetFloat("MasterVolume", ValueToDB(value));
    }

    private void SetMusicVolume(float value) {
        mixer.SetFloat("MusicVolume", ValueToDB(value));
    }

    private void SetEffectVolume(float value) {
        mixer.SetFloat("EffectVolume", ValueToDB(value));
    }

    private float ValueToDB(float value) {
        return ((value * -1) + 1) * -40;
    }

    private float DBToValue(float db) {
        return ((db / -40) * -1) + 1;
    }
}