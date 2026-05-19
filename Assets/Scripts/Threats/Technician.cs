using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Technician : Threat {
    [SerializeField] private float doorKillTime = 4;
    [SerializeField] private float doorPatience = 3f;
    [SerializeField] private float doorPatiencePenalty = 0.4f;
    private int doorPatienceCount;
    private float defMoveInterval;

    [Serializable] private struct TechnicianButton {
        public string code;
        public KeyCapButton button;
        public Sprite shape;
        public Color color;
    }

    [Header("Minigame")] 
    [SerializeField] private TechnicianButton[] technicianButtons;
    [SerializeField] private SpriteRenderer[] clipboardCodePlacements;
    [SerializeField] private float minigameTimeLimit = 8f;
    [Space]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorKnock;
    [SerializeField] private AudioClip leaveDoor;
    [SerializeField] private AudioClip minigameStart;
    [SerializeField] private AudioClip minigameComplete;
    
    private string[] currentCode = new string[4];
    private float minigameTime;
    private int minigameProgress;
    private Coroutine minigameCoroutine;
    
    private void Start() {
        defMoveInterval = movementInterval;
        agent.SetDestination(states[currentState].transform.position);
    }

    public override void UpdateAILevel(int newLevel) {
        if (newLevel == 0) {
            animator.transform.parent.gameObject.SetActive(false);
            if (minigameCoroutine != null) {
                StopCoroutine(minigameCoroutine);
                minigameCoroutine = null;
            }
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
            if (isMoving && minigameCoroutine == null && currentState == "teOffice") {
                minigameCoroutine = StartCoroutine(Minigame());
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
            case "teStart":
            case "teEastWing":
            case "teCafeteria":
            case "teReception":
            case "teCentralFactory":
            case "teWestWing":
                break;
            
            case "uRightDoor":
            case "uLeftDoor":
                movementInterval = doorPatience - (doorPatienceCount * doorPatiencePenalty);
                break;
            
            case "teOffice":
                if (currentState == "teOffice") return; // Already in office, do not move
                movementInterval = defMoveInterval;
                if (states[currentState].OfficeDoor.IsOpen) { // If door is open, move into office
                    states[currentState].OfficeDoor.FullOpenDoor(0.5f);
                }
                else return;
                break;
            
            default:
                return;
        }
        
        TryMoveTo(nextState);
    }
    
    protected override string GetNextState() {
        float randomValue = Random.value;
        string nextState = currentState switch {
            "teStart" => "teEastWing",
            "teEastWing" => "teCafeteria",
            "teCafeteria" => randomValue < 0.5f ? "teReception" : "teCentralFactory",
            "teReception" => "teWestWing",
            
            //Middle
            "teCentralFactory" => randomValue < 0.5f ? "teWestWing" : "uRightDoor",
            "teWestWing" => randomValue < 0.5f ? "teCentralFactory" : "uLeftDoor",
            
            _ => currentState
        };
        return nextState;
    }
    
    private float doorWaitTime;
    private float doorKillTimer;
    private void DoorStateUpdate() {
        if (currentState != "uRightDoor" && currentState != "uLeftDoor") return;
        ThreatStatePoint state = states[currentState];

        if (!isMoving && !state.OfficeDoor.IsOpen) {
            doorWaitTime += Time.deltaTime;
        }
        else {
            doorWaitTime = 0f;
        }
        if (!state.OfficeDoor.IsOpen && doorWaitTime > 4f) {
            doorPatienceCount++;
            audioSource.clip = leaveDoor;
            audioSource.volume = 0.5f;
            audioSource.Play();
            doorWaitTime = 0f;
            doorKillTimer = 0f;
            TryMoveTo(currentState == "uRightDoor" ? "teCentralFactory" : "teWestWing");
        }
        
        // Office Entering
        if (!isMoving && state.OfficeDoor.IsOpen) {
            doorKillTimer += Time.deltaTime;
        }

        if (doorKillTimer >= doorKillTime) {
            doorWaitTime = 0f;
            doorKillTimer = 0f;
            TryMoveTo("teOffice");
        }
    }
    
    // ~~ Minigame ~~
    private IEnumerator Minigame() {
        // Generate code
        int i = 0;
        while (i < 4) {
            int index = Random.Range(0, technicianButtons.Length);
            currentCode[i] = technicianButtons[index].code;
            clipboardCodePlacements[i].color = technicianButtons[index].color;
            clipboardCodePlacements[i].sprite = technicianButtons[index].shape;
            i++;
        }

        animator.SetBool("Clipboard", true);
        minigameProgress = 0;
        minigameTime = 0;

        audioSource.clip = minigameStart;
        audioSource.Play();
        
        // Wait for player to input
        while (minigameTime < minigameTimeLimit) {
            // check for win
            if (minigameProgress >= 4) {
                minigameTime += minigameTimeLimit;
            }
            minigameTime += Time.deltaTime;
            yield return null;
        }

        if (minigameProgress >= 4) {
            audioSource.clip = minigameComplete;
            audioSource.Play();
            TryMoveTo("teStart");
        }
        else {
            TriggerGameOver();
        }
        
        animator.SetBool("Clipboard", false);
        minigameCoroutine = null;
    }
    
    public void ButtonPressed(string code) {
        if (minigameCoroutine == null) return;
        
        if (code == currentCode[minigameProgress]) {
            minigameProgress++;
        }
        else {
            TriggerGameOver();
        }
    }
}