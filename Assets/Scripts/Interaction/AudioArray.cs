using UnityEngine;

[CreateAssetMenu(fileName = "AudioArray", menuName = "ScriptableObjects/AudioArray")]
public class AudioArray : ScriptableObject {
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private Vector2 pitchRange = new(0.9f, 1.1f);

    public AudioClip RandomClip() {
        int i = Random.Range(0, sounds.Length);
        return sounds[i];
    }

    public float RandomPitch() {
        return Random.Range(pitchRange.x, pitchRange.y);
    }
}