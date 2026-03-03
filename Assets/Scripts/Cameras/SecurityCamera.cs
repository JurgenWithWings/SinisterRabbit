using UnityEngine;

public class SecurityCamera : MonoBehaviour {
    public string camName;
    public Camera cam;

    private void Awake() {
        cam = GetComponent<Camera>();
    }
}