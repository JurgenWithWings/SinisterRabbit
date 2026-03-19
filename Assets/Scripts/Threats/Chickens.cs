using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chickens : Threat {
    [SerializeField] private Vector2 progressFillRate = new Vector2(5f, 25f);
    [SerializeField] private float killThreshold = 100;

    [SerializeField] private List<Chicken> chickens = new();
    [SerializeField] private List<float> spawnIntervals = new();

    private List<Chicken> inactiveChickens = new();
    private List<Chicken> activeChickens = new();
    private float progress;
    
    private float FillRate => Mathf.Lerp(progressFillRate.x, progressFillRate.y, progress / killThreshold);

    private void Start() {
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
        if (activeChickens.Count > 1) {
            progress = spawnIntervals[activeChickens.Count - 2];
        }
        else {
            progress = 0;
        }
        
        inactiveChickens.Add(chicken);
        activeChickens.Remove(chicken);
    }

    public override void Tick() {
        progress += FillRate * Time.deltaTime;

        if (activeChickens.Count != chickens.Count && progress > spawnIntervals[activeChickens.Count]) {
            int random = Random.Range(0, inactiveChickens.Count);
            activeChickens.Add(inactiveChickens[random]);
            inactiveChickens[random].gameObject.SetActive(true);
            inactiveChickens.RemoveAt(random);
        }

        if (progress >= killThreshold) {
            TriggerGameOver();
        }
    }
}