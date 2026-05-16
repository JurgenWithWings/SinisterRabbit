using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable] public struct AudioArrayElement {
    public AudioClip Clip;
    public float Volume;
    public Vector2 PitchRange;
    
    public AudioArrayElement(AudioClip clip) {
        Clip = clip;
        Volume = 1;
        PitchRange = Vector2.one;
    }
}

[CreateAssetMenu(fileName = "AudioArray", menuName = "ScriptableObjects/AudioArray")]
public class AudioArray : ScriptableObject {
    [SerializeField] private AudioArrayElement[] sounds;

    public void PlayRandomSound(AudioSource source) {
        if (sounds.Length == 0) return;
        
        source.clip = RandomClip(out int i);
        source.volume = sounds[i].Volume;
        source.pitch = RandomPitch(i);
        source.Play();
    }
    
    public AudioClip RandomClip() {
        return RandomClip(out int _);
    }
    
    public AudioClip RandomClip(out int index) {
        index = Random.Range(0, sounds.Length);
        return sounds[index].Clip;
    }

    public float RandomPitch(int index) {
        return Random.Range(sounds[index].PitchRange.x, sounds[index].PitchRange.y);
    }
}