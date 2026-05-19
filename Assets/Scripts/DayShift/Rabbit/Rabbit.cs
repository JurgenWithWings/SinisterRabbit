using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Rabbit : MonoBehaviour {
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private RabbitPOI startingPoi;
    [SerializeField] private bool hasEgg;
    [Space] 
    [SerializeField] private Vector2 workingTimeRange = new(5, 20);
    [SerializeField] private float walkingSpeed = 3.5f;
    [SerializeField] private float eggRunSpeed = 5f;
    [Space]
    [SerializeField] private MeshRenderer egg;
    [SerializeField] private Material[] eggMaterials;

    private RabbitPOI currentPoi;
    private float workingTimeRemaining;
    private bool atDestination;

    private void Awake() {
        currentPoi = startingPoi;
        agent.speed = hasEgg ? eggRunSpeed : walkingSpeed;
    }
    
    private void Start() {
        RabbitManager.instance.RegisterRabbit(this, currentPoi);
    }

    private void Update() {
        animator.SetFloat("Speed", agent.velocity.magnitude);
        
        if (!atDestination) {
            if (Vector3.Distance(agent.transform.position, currentPoi.transform.position) < 0.45f) {
                OnDestinationReached();
            }
        }
        
        if (currentPoi.PoiType == RabbitPOIType.Working) {
            WorkingUpdate();
        }
    }
    
    private void WorkingUpdate(){
        if (atDestination) {
            workingTimeRemaining -= Time.deltaTime;
        }
        
        if (workingTimeRemaining <= 0) {
            if (GetNewJob() == RabbitPOIType.Working) {
                workingTimeRemaining = Random.Range(workingTimeRange.x, workingTimeRange.y);
            }
        }
    }
    
    private RabbitPOIType GetNewJob() {
        RabbitPOIType newPoiType;
        if (hasEgg) {
            newPoiType = RabbitPOIType.EggDropOff;
        }
        else {
            newPoiType = Random.value < 0.5f ? RabbitPOIType.Working : RabbitPOIType.EggPickUp;
        }
        
        currentPoi = RabbitManager.instance.GetPoi(currentPoi, newPoiType);
        agent.SetDestination(currentPoi.transform.position);
        atDestination = false;
        
        return currentPoi.PoiType;
    }
    
    private void OnDestinationReached() {
        atDestination = true;

        switch (currentPoi.PoiType) {
            case RabbitPOIType.Working:
                transform.rotation = currentPoi.transform.rotation;
                break;
            case RabbitPOIType.EggPickUp:
                hasEgg = true;
                animator.SetBool("HasEgg", true);
                agent.speed = eggRunSpeed;
                egg.transform.parent.gameObject.SetActive(true);
                egg.material = eggMaterials[Random.Range(0, eggMaterials.Length)];
                GetNewJob();
                break;
            case RabbitPOIType.EggDropOff:
                hasEgg = false;
                animator.SetBool("HasEgg", false);
                egg.transform.parent.gameObject.SetActive(false);
                agent.speed = walkingSpeed;
                GetNewJob();
                break;
        }
    }
}