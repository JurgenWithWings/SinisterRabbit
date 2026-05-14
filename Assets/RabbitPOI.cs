using UnityEngine;

public enum RabbitPOIType {
    Working,
    EggPickUp,
    EggDropOff,
}
public class RabbitPOI : MonoBehaviour {
    [SerializeField] private RabbitPOIType type;
    public RabbitPOIType PoiType => type;
}