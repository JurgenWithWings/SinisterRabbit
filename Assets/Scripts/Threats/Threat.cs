using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum ThreatType {
    None,
    Doorman,
    Flock,
    Technician,
    Chef,
    Sheep,
    Thief
}

public abstract class Threat : MonoBehaviour {
    [SerializeField] protected ThreatType threatType;
    [SerializeField] protected CauseOfDeath deathCause;
    public ThreatType ThreatType => threatType;
    public CauseOfDeath DeathCause => deathCause;
    [Space]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private List<ThreatStatePoint> availableStates = new();
    protected Dictionary<string, ThreatStatePoint> states = new();
    [Space]
    [SerializeField] protected float movementInterval = 5f;
    protected float timer;
    protected string currentState;

    [SerializeField] protected Animator animator;
    
    protected bool isMoving;
    
    protected int level;

    protected ThreatManager manager;

    public virtual void Init(ThreatManager manager) {
        this.manager = manager;
        
        if (availableStates.Count != 0) {
            currentState = availableStates[0].name;
            foreach (ThreatStatePoint state in availableStates) {
                states[state.gameObject.name] = state;
            }
        }
    }
    
    public virtual void UpdateAILevel(int newLevel) {
        level = newLevel;
        TryMoveTo(availableStates[0].name);
    }

    // ~~ Movement and States ~~
    protected virtual string GetNextState() {
        return null;
    }
    
    protected void OnDestinationReached() {
        isMoving = false;
        transform.rotation = states[currentState].transform.rotation;
    }
    
    protected bool TryMoveTo(string state) {
        if (manager.TryEnterState(state, this)) {
            states[state].RegisterThreat(this);
            states[currentState].RegisterThreat(null);
            LeaveState(currentState);
            currentState = state;
            if (agent != null) {
                agent.SetDestination(states[currentState].transform.position);
            }
            isMoving = true;
            return true;
        }
        return false;
    }

    protected bool CanMoveTo(string state) {
        return manager.CanMoveTo(state);
    }

    protected void LeaveState(string state) {
        manager.LeaveState(state, this);
    }
    
    // ~~ Enemy Managment ~~
    protected bool RollLevel(int maxRoll = 20) {
        float random = Random.value * maxRoll;
        return random < level;
    }

    protected void TriggerGameOver() {
        manager.GameOver(this);
    }

    // ~~ Update ~~
    private void Update() {
        timer += Time.deltaTime;
        Tick();
    }

    protected abstract void Tick();

    
    // Update Events
    public virtual void CameraSystemStateUpdate(bool isOpen) { }
    
    public virtual void OfficeButtonPressed(string key) { }
}