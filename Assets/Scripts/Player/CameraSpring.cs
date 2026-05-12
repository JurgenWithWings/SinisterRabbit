using UnityEngine;

public class CameraSpring : MonoBehaviour {
    [SerializeField, Min(0.01f)] private float halfLife = 0.075f;
    [Space]
    [SerializeField] private float frequency = 18f;
    [Space]
    [SerializeField] private float angularDisplacement = 2f;
    [SerializeField] private float linearDisplacement = 0.05f;
    
    private Vector3 springPosition;
    private Vector3 springVelocity;
    
    public void Initialize() {
        springPosition = transform.position;
        springVelocity = Vector3.zero;
    }

    public void UpdateSpring(float deltaTime, Vector3 up) {
        transform.localPosition = Vector3.zero;
        
        Spring(ref springPosition, ref springVelocity, transform.position, halfLife, frequency, deltaTime);

        Vector3 relativeSpringPosition = springPosition - transform.position;
        float springHeight = Vector3.Dot(relativeSpringPosition, up);
        
        transform.localEulerAngles = new Vector3(-springHeight * angularDisplacement, 0f, 0f);
        transform.position += relativeSpringPosition * linearDisplacement;
    }
    
    private void Spring(ref Vector3 current, ref Vector3 velocity, Vector3 target, float halfLife, float angularFrequency, float timeStep) {
        float dampingRatio = -Mathf.Log(0.5f) / (angularFrequency * halfLife);
        float f = 1.0f + 2.0f * timeStep * dampingRatio * angularFrequency;
        float oo = angularFrequency * angularFrequency;
        float hoo = timeStep * oo;
        float hhoo = timeStep * hoo;
        float detInv = 1.0f / (f + hhoo);
        Vector3 detX = f * current + timeStep * velocity + hhoo * target;
        Vector3 detV = velocity + hoo * (target - current);
        current = detX * detInv;
        velocity = detV * detInv;
    }
}