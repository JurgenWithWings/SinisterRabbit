using UnityEngine;

public class CameraLean : MonoBehaviour {
    [SerializeField] private float attackDamping = 0.5f;
    [SerializeField] private float decayDamping = 0.3f;
    [SerializeField] private float walkStrength = 0.075f;
    [SerializeField] private float slideStrength = 0.2f;
    [SerializeField] private float strengthResponse = 5f;

    private Vector3 dampedAcceleration;
    private Vector3 dampedAccelerationVelocity;

    private float smoothStrength;
    public void Initialize() {
        smoothStrength = walkStrength;
    }

    public void UpdateLean(float deltaTime, bool sliding, Vector3 acceleration, Vector3 up) {
        Vector3 planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);
        float damping = planarAcceleration.magnitude > dampedAcceleration.magnitude
            ? attackDamping
            : decayDamping;

        dampedAcceleration = Vector3.SmoothDamp(
            current: dampedAcceleration,
            target: planarAcceleration,
            currentVelocity: ref dampedAccelerationVelocity,
            smoothTime: damping,
            maxSpeed: float.PositiveInfinity,
            deltaTime: deltaTime
        );
        
        // Get the rotation axis based on acceleration vector.
        Vector3 leanAxis = Vector3.Cross(dampedAcceleration.normalized, up).normalized;
        
        // Reset the rotation to that if its parent.
        transform.localRotation = Quaternion.identity;
        
        // Smoothly change between the strength of the lean based on character state.
        float targetStrength = sliding 
            ? slideStrength 
            : walkStrength;
        smoothStrength = Mathf.Lerp(smoothStrength, targetStrength, 1f - Mathf.Exp(-strengthResponse * deltaTime));
        
        // Rotate around the lean axis.
        transform.rotation = Quaternion.AngleAxis(-dampedAcceleration.magnitude * smoothStrength, leanAxis) * transform.rotation;
    }
}