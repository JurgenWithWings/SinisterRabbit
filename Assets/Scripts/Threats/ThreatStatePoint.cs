using System;
using UnityEngine;

public class ThreatStatePoint : MonoBehaviour {
    [SerializeField] private SecurityCamera securityCamera;
    [SerializeField] private OfficeDoor officeDoor;
    [SerializeField] private CameraSystem cameraSystem;

    public Threat threat { get; private set; }
    public bool Occupied => threat != null;

    private void Start() {
        if (officeDoor != null) {
            officeDoor.OnDoorOpen += OfficeDoorOnOpen;
            officeDoor.OnDoorClosed += OfficeDoorOnClosed;
        }

        if (cameraSystem != null) {
            cameraSystem.OnCamsOpen += CameraSystemOnOpen;
            cameraSystem.OnCamsClosed += CameraSystemOnClosed;
        }
    }

    private void OnDestroy() {
        if (officeDoor != null) {
            officeDoor.OnDoorOpen -= OfficeDoorOnOpen;
            officeDoor.OnDoorClosed -= OfficeDoorOnClosed;
        }

        if (cameraSystem != null) {
            cameraSystem.OnCamsOpen -= CameraSystemOnOpen;
            cameraSystem.OnCamsClosed -= CameraSystemOnClosed;
        }
    }

    public void RegisterThreat(Threat threat) {
        this.threat = threat;
    }
    

    public bool IsCameraActive() {
        return securityCamera?.cam.enabled ?? false;
    }

    public bool IsDoorOpen() {
        return officeDoor?.IsOpen ?? false;
    }

    public bool AreCamsOpen() {
        return cameraSystem?.IsOpen ?? false;
    }

    public void FullOpenDoor(float holdDuration) {
        officeDoor.FullOpenDoor(holdDuration);
    }
    
    
    // ~~ Event Handles ~~
    // Door
    private void OfficeDoorOnClosed() {
        threat?.DoorStateUpdate(false);
    }
    private void OfficeDoorOnOpen() {
        threat?.DoorStateUpdate(true);
    }
    
    // Cam System
    private void CameraSystemOnClosed() {
        threat?.CameraSystemStateUpdate(false);
    }
    private void CameraSystemOnOpen() {
        threat?.CameraSystemStateUpdate(true);
    }
}
