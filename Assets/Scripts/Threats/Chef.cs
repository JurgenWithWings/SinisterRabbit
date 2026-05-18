using UnityEngine;

public class Chef : Threat {
    [SerializeField] private float normalMoveSpeed = 7f;
    [SerializeField] private float sadMoveSpeed = 3f;
    [SerializeField] private float powerDrain = 4f;
    
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
        
        float distanceToTarget = Vector3.Distance(agent.transform.position, states[currentState].transform.position);
        if (distanceToTarget < 0.55f) {
            OnDestinationReached();
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
        if (!RollLevel(35)) return;
        
        string nextState = GetNextState();

        if (!CanMoveTo(nextState)) return;
        
        //EnterState
        switch (nextState) {
            case "chStage1":
                agent.speed = sadMoveSpeed;
                animator.SetBool("IsSad", true);
                animator.SetInteger("Stage", 1);
                break;
            case "chStage2":
                agent.speed = normalMoveSpeed;
                animator.SetBool("IsSad", false);
                animator.SetInteger("Stage", 2);
                break;
            case "chStage3":
                agent.speed = normalMoveSpeed;
                animator.SetBool("IsSad", false);
                animator.SetInteger("Stage", 3);
                break;
            
            case "chWestConveyor":
            case "chCentralConveyor":
            case "chEastConveyor":
                agent.speed = normalMoveSpeed;
                animator.SetBool("IsSad", false);
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
        
        agent.speed = sadMoveSpeed;
        animator.SetBool("IsSad", true);
        TryMoveTo("chStage1");
    }
}