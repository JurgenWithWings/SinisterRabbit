using UnityEngine;

public class CameraFovSpeed : MonoBehaviour {
    [SerializeField] private Camera camera;
    [Space] 
    [SerializeField] private float min = 70f;
    [SerializeField] private float max = 90f;
    [SerializeField] private float response = 15f;
    [Space] 
    [SerializeField] private float maximumSpeed = 45f;
    
    public void Initialize() {
        
    }

    public void UpdateFoV(float deltaTime, float speed) {
        float targetFov = Mathf.Lerp(min, max, speed / maximumSpeed);
        camera.fieldOfView = Mathf.Lerp(
            a: camera.fieldOfView, 
            b: targetFov,
            t: 1f - Mathf.Exp(-response * deltaTime)
        );
    }
}