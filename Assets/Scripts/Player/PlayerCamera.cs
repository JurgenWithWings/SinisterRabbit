using System;
using UnityEngine;

public struct CameraInput {
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour {
    private Vector3 eulerAngles;
    
    public void Initialize(Transform target) {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    public void UpdateRotation(CameraInput input) {
        eulerAngles += new Vector3(-input.Look.y, input.Look.x);
        transform.eulerAngles = eulerAngles;
    }
    
    public void UpdatePosition(Transform target) {
        transform.position = target.position;
    }
}