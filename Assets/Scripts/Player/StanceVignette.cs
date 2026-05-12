using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StanceVignette : MonoBehaviour {
    [SerializeField] private float min = 0.1f;
    [SerializeField] private float max = 0.35f;
    [SerializeField] private float response = 10f;
    
    private VolumeProfile profile;
    private Vignette vignette;
    
    public void Initialize(VolumeProfile prf) {
        profile = prf;
        
        if (!prf.TryGet(out vignette)) {
            vignette = profile.Add<Vignette>();
        }
        vignette.intensity.Override(min);
    }

    public void UpdateVignette(float deltaTime, Stance stance) {
        float targetIntensity = stance is Stance.Stand ? min : max;
        vignette.intensity.value = Mathf.Lerp(
            a: vignette.intensity.value,
            b: targetIntensity,
            t: 1f - Mathf.Exp(-response * deltaTime)
        );
    }
}