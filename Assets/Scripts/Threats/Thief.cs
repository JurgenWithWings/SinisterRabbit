using System.Collections;
using UnityEngine;

public class Thief : Threat {
    [SerializeField] private GameObject rat;
    [SerializeField] private GameObject truck;
    [Space]
    [SerializeField] private MeshRenderer truckRenderer;
    [SerializeField] private Material truckNormalMat;
    [SerializeField] private Material truckRatMat;
    [Space] 
    [SerializeField] private float truckMoveSpeed = 2f;
    [SerializeField] private Animation shutterAnim;
    [Space]
    [SerializeField] private float truckIntervalDecreasePerLevel = 1.5f;
    [SerializeField] private float truckTimer = 5f;
    [SerializeField] private float powerPenalty = 30f;
    [Space]
    [SerializeField] private AudioSource audioSource;
    
    private Coroutine timerCoroutine;
    private float currentTimer;
    
    private bool isRat;

    private float stateDistance;
    private Vector3 truckStartMovePos;

    private void Start() {
        states["thStart"].RegisterThreat(this);
        
        stateDistance = Vector3.Distance(states["thStart"].transform.position, states["thDepot"].transform.position);
        truckStartMovePos = truck.transform.position;
    }
    
    public override void UpdateAILevel(int newLevel) {
        if (newLevel == 0) {
            truck.SetActive(false);
            rat.SetActive(false);
            if (timerCoroutine != null) {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }
        else {
            truck.SetActive(true);
        }
        base.UpdateAILevel(newLevel);
    }

    private float GetInterval() {
        return movementInterval - (truckIntervalDecreasePerLevel * level);
    }

    protected override void Tick() {
        if (level == 0) return;
        if (timer > GetInterval()) {
            AttemptSendTruck();
        }

        LerpMoveTruck();

        if (currentState == "thDepot" && Vector3.Distance(truck.transform.position, states[currentState].transform.position) < 0.01f && timerCoroutine == null) {
            timerCoroutine = StartCoroutine(Timer());
        }
        
        // Reset timer at end of Update
        if (timer > GetInterval()) {
            timer = 0f;
        }
    }
    
    private void AttemptSendTruck() {
        if (level == 0) return;
        if (currentState == "thDepot") return;
        
        isRat = RollLevel(30);
        
        rat.gameObject.SetActive(isRat);
        truckRenderer.material = isRat ? truckRatMat : truckNormalMat;

        currentTimer = 0;
        
        TryMoveTo("thDepot");
        truckStartMovePos = truck.transform.position;
    }
    
    private void LerpMoveTruck() {
        float distance = Vector3.Distance(truckStartMovePos, states[currentState].transform.position);
        if ((states[currentState].transform.position - truck.transform.position).magnitude < 0.1f) {
            truck.transform.position = states[currentState].transform.position;
            return;
        }
        Vector3 direction = (states[currentState].transform.position - truck.transform.position).normalized;
        float t = distance / stateDistance;
        truck.transform.position += direction * t * truckMoveSpeed * Time.deltaTime;
    }
    
    public override void OfficeButtonPressed(string key) {
        if (key != "thief" || shutterAnim.isPlaying) return;

        shutterAnim.Play();
        TryMoveTo("thStart");
        truckStartMovePos = truck.transform.position;
        if (timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        if (!isRat) {
            NightLogicManager.Instance.DecreasePower(powerPenalty);
        }
    }

    private IEnumerator Timer() {
        if (GameOverManager.Instance.IsGameOver) {
            yield break;
        }
        audioSource.Play();
        
        while (currentTimer < truckTimer) {
            currentTimer += Time.deltaTime;
            yield return null;
        }

        if (currentTimer >= truckTimer) {
            if (isRat) {
                TriggerGameOver();
            }
            else {
                TryMoveTo("thStart");
                truckStartMovePos = truck.transform.position;
            }
        }
        
        timerCoroutine = null;
    }
}