using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chickens : Threat {
    [SerializeField] private List<Chicken> chickens = new();
    private List<Chicken> activeChickens = new();
    private List<Chicken> inactiveChickens = new();

    private void Start() {
        states["Office"].RegisterThreat(this);
        
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

    private void OnChickenClicked(Chicken chicken) {
        activeChickens.Remove(chicken);
        inactiveChickens.Add(chicken);
        
        if (killTimer != null) {
            StopCoroutine(killTimer);
            killTimer = null;
        }
    }

    public override void Tick() { }

    public override void CameraSystemStateUpdate(bool isOpen) {
        if (isOpen) {
            if (RollLevel()) {
                SpawnChicken();
            }
        }
    }

    private void SpawnChicken() {
        int random = Random.Range(0, inactiveChickens.Count);
        Chicken chicken = inactiveChickens[random];
        activeChickens.Add(chicken);
        inactiveChickens.Remove(chicken);
        chicken.gameObject.SetActive(true);
        
        if (activeChickens.Count == chickens.Count) {
            killTimer = StartCoroutine(KillTimer());
        }
    }

    private Coroutine killTimer;
    private IEnumerator KillTimer() {
        yield return new WaitForSeconds(5);
        
        TriggerGameOver();
    }
}