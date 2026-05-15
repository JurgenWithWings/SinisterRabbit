using UnityEngine;

public enum RabbitPOIType {
    Working,
    EggPickUp,
    EggDropOff,
}
public class RabbitPOI : MonoBehaviour {
    [SerializeField] private RabbitPOIType type;
    public RabbitPOIType PoiType => type;


#if UNITY_EDITOR
    [Space]
    [SerializeField] private float gizmoLength = 0.67f;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField, Range(0.05f, 0.5f)] private float headLengthRatio = 0.5f;

    private void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        Vector3 start = transform.position;
        Vector3 dir = transform.forward.normalized;
        float len = Mathf.Max(0.001f, gizmoLength);
        Vector3 end = start + dir * len;

        // main shaft
        Gizmos.DrawLine(start, end);

        // arrow head
        float headLen = len * headLengthRatio;
        Quaternion baseRot = Quaternion.LookRotation(dir);
        Vector3 right = baseRot * Quaternion.Euler(0f, 180f + 20, 0f) * Vector3.forward;
        Vector3 left = baseRot * Quaternion.Euler(0f, 180f - 20, 0f) * Vector3.forward;
        Gizmos.DrawLine(end, end + right * headLen);
        Gizmos.DrawLine(end, end + left * headLen);

        // optional origin marker
        Gizmos.DrawWireSphere(start, 0.25f);
    }
#endif
}