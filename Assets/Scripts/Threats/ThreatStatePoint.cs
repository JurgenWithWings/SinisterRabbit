using System;
using UnityEngine;

public class ThreatStatePoint : MonoBehaviour {
    [SerializeField] private SecurityCamera securityCamera;
    public SecurityCamera SecurityCamera => securityCamera;
    
    [SerializeField] private OfficeDoor officeDoor;
    public OfficeDoor OfficeDoor => officeDoor;
    
    [SerializeField] private CameraSystem cameraSystem;
    public CameraSystem CameraSystem => cameraSystem;

    public Threat threat { get; private set; }
    public bool Occupied => threat != null;

    private void Start() {
        if (cameraSystem != null) {
            cameraSystem.OnCamsOpen += CameraSystemOnOpen;
            cameraSystem.OnCamsClosed += CameraSystemOnClosed;
        }
    }

    private void OnDestroy() {

        if (cameraSystem != null) {
            cameraSystem.OnCamsOpen -= CameraSystemOnOpen;
            cameraSystem.OnCamsClosed -= CameraSystemOnClosed;
        }
    }

    public void RegisterThreat(Threat threat) {
        this.threat = threat;
    }
    
    
    // ~~ Event Handles ~~
    // Cam System
    private void CameraSystemOnClosed() {
        threat?.CameraSystemStateUpdate(false);
    }
    private void CameraSystemOnOpen() {
        threat?.CameraSystemStateUpdate(true);
    }
}
