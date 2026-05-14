using UnityEngine;

public class TestPlayerPhysics : MonoBehaviour {
    [SerializeField] private Vector3 velocity = Vector3.up * 10f;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out PlayerCharacter player)) {
            player.AddVelocity(velocity);
        }
    }
}