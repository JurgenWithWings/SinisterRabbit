using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {
    public static MusicManager instance;
    
    private AudioSource audioSource;

    private void Awake() {
        if (instance != null || instance == this) {
            Destroy(gameObject); 
        }
        instance = this;
        
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayMusic(AudioClip clip) {
        if (clip == null || audioSource.clip == clip) return;
        
        audioSource.clip = clip;
        audioSource.Play();
    }
}
