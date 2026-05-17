using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : Threat {
    [SerializeField] private List<Chicken> chickens = new();
    private List<Chicken> activeChickens = new();
    private List<Chicken> inactiveChickens = new();
    [Space]
    [SerializeField] private float[] counterIncreaseLevels = { -1.5f, 2.5f, 7f, 14f, 20f };
    [SerializeField] private float chickenPopCounterDecrease = 15f;
    
    private float currentCounter;

    private void Start() {
        states["Flock"].RegisterThreat(this);
        
        foreach (Chicken chicken in chickens) {
            chicken.OnChickenClicked += OnChickenClicked;
            inactiveChickens.Add(chicken);
        }
    }

    private void OnDestroy() {
        foreach (Chicken chicken in chickens) {
            chicken.OnChickenClicked -= OnChickenClicked;
        }
    }
    
    public override void UpdateAILevel(int newLevel) {
        if (newLevel == 0) {
            foreach (Chicken chicken in chickens) {
                if (activeChickens.Contains(chicken)) {
                    activeChickens.Remove(chicken);
                    inactiveChickens.Add(chicken);
                }
                chicken.gameObject.SetActive(false);
            }
        }
        else {
            foreach (Chicken chicken in chickens) {
                chicken.gameObject.SetActive(true);
            }
        }
        base.UpdateAILevel(newLevel);
    }

    private void OnChickenClicked(Chicken chicken) {
        activeChickens.Remove(chicken);
        inactiveChickens.Add(chicken);

        currentCounter -= chickenPopCounterDecrease;
    }

    protected override void Tick() {
        float increment = counterIncreaseLevels[Mathf.Min(chickens.Count, activeChickens.Count)];
        currentCounter += increment * Time.deltaTime;
            
        currentCounter = Mathf.Clamp(currentCounter, 0, 100);

        if (currentCounter >= 99.99f) {
            TriggerGameOver();
        }
    }

    public override void CameraSystemStateUpdate(bool isOpen) {
        if (isOpen) {
            if (RollLevel()) {
                SpawnChicken();
            }
        }
    }

    private void SpawnChicken() {
        if (inactiveChickens.Count == 0) return;
        int random = Random.Range(0, inactiveChickens.Count);
        Chicken chicken = inactiveChickens[random];
        activeChickens.Add(chicken);
        inactiveChickens.Remove(chicken);
        chicken.Activate();
    }
}