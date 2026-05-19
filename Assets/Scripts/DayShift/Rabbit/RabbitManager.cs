using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RabbitManager : MonoBehaviour {
    public static RabbitManager instance;
    
    [SerializeField] private List<Rabbit> rabbits = new();
    private List<RabbitPOI> availablePois = new();
    private List<RabbitPOI> unavailablePois = new();
    
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Multiple RabbitManagers in scene!");
            Destroy(this);
            return;
        }
        instance = this;
        
        availablePois = FindObjectsByType<RabbitPOI>(FindObjectsSortMode.None).ToList();
    }
    
    public void RegisterRabbit(Rabbit rabbit, RabbitPOI poi) {
        rabbits.Add(rabbit);
        if (poi.PoiType == RabbitPOIType.Working) {
            unavailablePois.Add(poi);
            availablePois.Remove(poi);
        }
    }

    public RabbitPOI GetPoi(RabbitPOI oldPoi, RabbitPOIType newPoiType) {
        RabbitPOI newPoi = null;
        bool foundMatch = false;
        for (int i = 0; i < 20; i++) {
            int index = Random.Range(0, availablePois.Count);
            if (availablePois[index].PoiType == newPoiType) {
                newPoi = availablePois[index];
                foundMatch = true;
                break;
            }
        }
        // If no mach was found after 20 tries, just get the first one that matches
        if (!foundMatch) {
            newPoi = availablePois.Find(t => t.PoiType == newPoiType);
         
            // if no poi of the same type is available, just pick up an egg
            if (newPoi == null) {
                newPoi = availablePois.Find(t => t.PoiType == RabbitPOIType.EggPickUp);
            }
            
            // Move it to the back of the list so the rabbits don't swarm it.
            availablePois.Remove(newPoi);
            availablePois.Add(newPoi);
        }
        
        if (oldPoi.PoiType == RabbitPOIType.Working) {
            unavailablePois.Remove(oldPoi);
            availablePois.Add(oldPoi);
        }

        if (newPoi.PoiType == RabbitPOIType.Working) {
            unavailablePois.Add(newPoi);
            availablePois.Remove(newPoi);
        }
        return newPoi;
    }
}