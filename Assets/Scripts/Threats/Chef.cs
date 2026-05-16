using UnityEngine;

public class Chef : Threat {
    [SerializeField] private float powerDrain = 4f;
    
    private void Start() {
        agent.SetDestination(states[currentState].transform.position);
    }
    
    protected override void Tick() {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        
        if (!isMoving && timer > movementInterval) {
            AttemptAdvance();
        }
        
        if (isMoving) {
            if (Vector3.Distance(agent.transform.position, states[currentState].transform.position) < 0.55f) {
                OnDestinationReached();
            }
        }

        if (!isMoving && currentState.Contains("Conveyor")) {
            NightLogicManager.Instance.DecreasePower(powerDrain * Time.deltaTime, CauseOfDeath.Chef);
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
        
        //EnterState
        switch (nextState) {
            case "chStage1":
                animator.SetInteger("Stage", 1);
                break;
            case "chStage2":
                animator.SetInteger("Stage", 2);
                break;
            case "chStage3":
                animator.SetInteger("Stage", 3);
                break;
            
            case "chWestConveyor":
            case "chCentralConveyor":
            case "chEastConveyor":
                animator.SetInteger("Stage", 0);
                break;
            
            default:
                return;
        }
        
        TryMoveTo(nextState);
    }
    
    protected override string GetNextState() {
        float randomValue = Random.value;
        string nextState = currentState switch {
            "chStage1" => "chStage2",
            "chStage2" => "chStage3",
            "chStage3" => randomValue < 1/3f ? "chWestConveyor" : randomValue < 2/3f ? "chCentralConveyor" : "chEastConveyor",
            
            _ => currentState
        };
        return nextState;
    }

    public override void OfficeButtonPressed(string key) {
        if (isMoving || key != "chef" || !currentState.Contains("Conveyor")) return;
        
        TryMoveTo("chStage1");
    }
}