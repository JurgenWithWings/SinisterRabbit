using System;
using UnityEngine;

public class RabbitGoombaTrigger : MonoBehaviour {
    [SerializeField] private Vector3 velocity = Vector3.up * 10f;
    
    public event Action OnGoombaHit;
    
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out PlayerCharacter player)) {
            if (player.State.Velocity.y < -0.05f) {
                player.AddVelocity(velocity + (Vector3.up * -player.State.Velocity.y));
                OnGoombaHit?.Invoke();
            }
        }
    }
}