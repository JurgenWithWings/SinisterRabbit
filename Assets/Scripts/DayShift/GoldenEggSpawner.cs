using System;
using UnityEngine;

public class GoldenEggSpawner : MonoBehaviour {
    [SerializeField] private GameObject goldenEggPrefab;
    [SerializeField] private float timeValue = 15f;
    [SerializeField] private AudioSource audioSource;

    private bool spawned;
    public bool Spawned => spawned;
    
    public event Action OnEggCollected;

    public void Spawn() {
        goldenEggPrefab.SetActive(true);
        spawned = true;
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out TriggerEfffector effector)) {
            DayLogicManager.Instance.AddTime(timeValue);
            OnEggCollected?.Invoke();
            audioSource.Play();
            goldenEggPrefab.SetActive(false);
        }
    }
}