using UnityEngine;

public class Doorman : Threat {
    private void Start() {
        agent.SetDestination(states[currentState].transform.position);
    }
    
    public override void UpdateAILevel(int newLevel) {
        if (newLevel == 0) {
            animator.transform.parent.gameObject.SetActive(false);
        }
        else {
            animator.transform.parent.gameObject.SetActive(true);
        }
        base.UpdateAILevel(newLevel);
    }

    protected override void Tick() {
        if (level == 0) return;
        animator.SetFloat("Speed", agent.velocity.magnitude);
        
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
            case "dmStart":
            case "dmReception":
            case "dmOffice1":
            case "dmHall":
            case "dmCafeteria":
            case "dmFactoryFloor":
            case "dmLeftStairs":
            case "dmRightStairs":
            case "uLeftDoor":
            case "uRightDoor":
                //if (states[currentState].IsCameraActive()) return; // Camera stalling
                break;
            
            case "dmOffice":
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
        float randomValue = Random.value;
        string nextState = currentState switch {
            "dmStart" => "dmReception",
            "dmReception" => randomValue < 0.5f ? "dmOffice1" : "dmHall",
            
            "dmOffice1" => randomValue < 0.5f ? "dmFactoryFloor" : "dmHall",
            "dmHall" => randomValue < 1/3f ? "dmFactoryFloor" : randomValue < 2/3f ? "dmCafeteria" : "dmRightStairs",
            "dmCafeteria" => "dmRightStairs",
            "dmFactoryFloor" => "dmLeftStairs",
            
            // Right Side
            "dmRightStairs" => "uRightDoor",
            "uRightDoor" => "dmOffice",
            
            // Left Side
            "dmLeftStairs" => "uLeftDoor",
            "uLeftDoor" => "dmOffice",
            
            _ => currentState
        };
        return nextState;
    }

    

    private float doorWaitTime;
    private void DoorStateUpdate() {
        if (currentState != "uRightDoor" && currentState != "uLeftDoor") return;
        ThreatStatePoint state = states[currentState];

        if (!isMoving && !state.OfficeDoor.IsOpen) {
            doorWaitTime += Time.deltaTime;
        }
        else {
            doorWaitTime = 0f;
        }
        
        if (!state.OfficeDoor.IsOpen && doorWaitTime > 2f) {
            TryMoveTo("dmReception");
        }
    }
}