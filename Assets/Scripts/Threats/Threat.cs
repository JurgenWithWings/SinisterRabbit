using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum ThreatType {
    None,
    Doorman,
    Flock,
    Technician,
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
    [SerializeField] protected SerializedDictionary<string, ThreatStatePoint> states = new();
    
    protected int level;

    protected ThreatManager manager;

    public virtual void Init(ThreatManager manager) {
        this.manager = manager;
    }
    
    public void UpdateAILevel(int newLevel) {
        level = newLevel;
    }

    // ~~ Helper Methods ~~
    protected bool RollLevel(int maxRoll = 20) {
        float random = Random.value * maxRoll;
        return random < level;
    }
    
    protected bool TryMoveTo(string state) {
        return manager.TryEnterState(state, this);
    }

    protected void LeaveState(string state) {
        manager.LeaveState(state, this);
    }

    protected void TriggerGameOver() {
        manager.GameOver(this);
    }

    
    private void Update() => Tick();
    public abstract void Tick();

    
    // Update Events
    public virtual void CameraSystemStateUpdate(bool isOpen) { }
}