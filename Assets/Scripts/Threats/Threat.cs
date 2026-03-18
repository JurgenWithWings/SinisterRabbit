using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class Threat : MonoBehaviour {
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected SerializedDictionary<string, ThreatStatePoint> states = new();
    [Space] 
    [SerializeField] protected int level = 5;

    protected ThreatManager manager;

    public virtual void Init(ThreatManager manager) {
        this.manager = manager;
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
    public virtual void DoorStateUpdate(bool isOpen) { }
    public virtual void CameraSystemStateUpdate(bool isOpen) { }
}