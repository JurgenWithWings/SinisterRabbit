using System.Collections;
using UnityEngine;

public class Doorman : Threat {
    [SerializeField] private float movementInterval = 5f;
    
    private string currentState = "Outside";
    private float timer;
    
    private bool isMoving;

    private void Start() {
        agent.SetDestination(states[currentState].transform.position);
    }
    
    public override void Tick() {
        timer += Time.deltaTime;
        
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

    void AttemptAdvance() {
        if (!RollLevel()) return;
        
        string nextState = currentState switch {
            "Outside" => "Main",
            "Main" => Random.value < 0.5f ? "RightHallway" : "LeftHallway",
            
            // Right Side
            "RightHallway" => "RightDoor",
            "RightDoor" => "Office",
            
            // Left Side
            "LeftHallway" => "LeftDoor",
            "LeftDoor" => "Office",
            
            // Office
            "Office" => "GameOver",
            "GameOver" => "Main",
            
            _ => currentState
        };

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
                    states[currentState].OfficeDoor.FullOpenDoor(0.5f);
                }
                else return;
                break;
            
            default:
                return;
        }
        
        TryMove(nextState);
    }

    private void TryMove(string nextState) {
        if (TryMoveTo(nextState)) {
            states[nextState].RegisterThreat(this);
            states[currentState].RegisterThreat(null);
            LeaveState(currentState);
            currentState = nextState;
            agent.SetDestination(states[currentState].transform.position);
            isMoving = true;
        }
    }

    private void OnDestinationReached() {
        isMoving = false;
        
        transform.rotation = states[currentState].transform.rotation;
    }

    private float doorWaitTime;
    public void DoorStateUpdate() {
        if (currentState != "RightDoor" && currentState != "LeftDoor") return;
        ThreatStatePoint state = states[currentState];

        if (!isMoving && !state.OfficeDoor.IsOpen) {
            doorWaitTime += Time.deltaTime;
        }
        else {
            doorWaitTime = 0f;
        }
        
        if (!state.OfficeDoor.IsOpen && doorWaitTime > 2f) {
            TryMove("Main");
        }
    }

    public override void CameraSystemStateUpdate(bool isOpen) {
        if (isOpen && currentState == "Office" && Random.value < 0.5f) { // 50% chance when opening cams that doorman kills you
            TriggerGameOver();
        }
    }
}