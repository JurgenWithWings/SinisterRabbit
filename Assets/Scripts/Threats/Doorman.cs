using System.Collections;
using UnityEngine;

public class Doorman : Threat {
    private void Start() {
        agent.SetDestination(states[currentState].transform.position);
    }

    protected override void Tick() {
        if (!isMoving && timer > movementInterval) {
            AttemptAdvance();
        }

        DoorStateUpdate();

        if (isMoving) {
            if (Vector3.Distance(agent.transform.position, states[currentState].transform.position) < 0.55f) {
                OnDestinationReached();
            }
        }
        
        // Reset timer at end of Update
        if (timer > movementInterval) {
            timer = 0f;
        }
    }

    private void AttemptAdvance() {
        if (!RollLevel()) return;
        
        string nextState = GetNextState();

        if (!CanMoveTo(nextState)) return;
        
        switch (nextState) {
            case "Outside":
            case "Main":
            case "RightHallway":
            case "LeftHallway":
            case "RightDoor":
            case "LeftDoor":
                //if (states[currentState].IsCameraActive()) return; // Camera stalling
                break;
            
            case "Office":
                if (states[currentState].OfficeDoor.IsOpen) { // If door is open, move into office
                    TriggerGameOver();
                }
                return;
            
            default:
                return;
        }

        TryMoveTo(nextState);
    }

    protected override string GetNextState() {
        string nextState = currentState switch {
            "Outside" => "Main",
            "Main" => Random.value < 0.5f ? "RightHallway" : "LeftHallway",
            
            // Right Side
            "RightHallway" => "RightDoor",
            "RightDoor" => "Office",
            
            // Left Side
            "LeftHallway" => "LeftDoor",
            "LeftDoor" => "Office",
            
            _ => currentState
        };
        return nextState;
    }

    

    private float doorWaitTime;
    private void DoorStateUpdate() {
        if (currentState != "RightDoor" && currentState != "LeftDoor") return;
        ThreatStatePoint state = states[currentState];

        if (!isMoving && !state.OfficeDoor.IsOpen) {
            doorWaitTime += Time.deltaTime;
        }
        else {
            doorWaitTime = 0f;
        }
        
        if (!state.OfficeDoor.IsOpen && doorWaitTime > 2f) {
            TryMoveTo("Main");
        }
    }
}