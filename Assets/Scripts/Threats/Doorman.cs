using UnityEngine;

public class Doorman : Threat {
    [SerializeField] private float doorKillTime = 4;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorKnock;
    [SerializeField] private AudioClip leaveDoor;
    
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

        float distanceToTarget = Vector3.Distance(agent.transform.position, states[currentState].transform.position);
        if (distanceToTarget < 0.55f) {
            if (isMoving && currentState.Contains("Door")) {
                audioSource.clip = doorKnock;
                audioSource.volume = 1f;
                audioSource.Play();
            }
            OnDestinationReached();
        }
        
        // Reset timer at end of Update
        if (timer > movementInterval) {
            timer = 0f;
        }
    }

    private void AttemptAdvance() {
        if (level == 0) return;
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
            
            // Left Side
            "dmLeftStairs" => "uLeftDoor",
            
            _ => currentState
        };
        return nextState;
    }

    

    private float doorWaitTime;
    private float doorKillTimer;
    private void DoorStateUpdate() {
        if (currentState != "uRightDoor" && currentState != "uLeftDoor") return;
        ThreatStatePoint state = states[currentState];

        // Door clearing
        if (!isMoving && !state.OfficeDoor.IsOpen) {
            doorWaitTime += Time.deltaTime;
        }
        else {
            doorWaitTime = 0f;
        }
        if (!state.OfficeDoor.IsOpen && doorWaitTime > 2f) {
            audioSource.clip = leaveDoor;
            audioSource.volume = 0.5f;
            audioSource.Play();
            doorWaitTime = 0f;
            doorKillTimer = 0f;
            TryMoveTo("dmReception");
        }

        // Door killing
        if (!isMoving && state.OfficeDoor.IsOpen) {
            doorKillTimer += Time.deltaTime;
        }

        if (doorKillTimer >= doorKillTime) {
            TriggerGameOver();
        }
    }
}