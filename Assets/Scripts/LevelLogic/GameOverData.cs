using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameOverData", menuName = "Game Over Data")]
public class GameOverData : ScriptableObject {
    [Serializable] public struct DeathInfo {
        public CauseOfDeath cause;
        public string deathText;
        [TextArea] public string tooltip;
        public Sprite image;
    }

    [Header("Cause of Death Info")]
    public DeathInfo doormanDeathInfo;
    public DeathInfo flockDeathInfo;
    public DeathInfo technicianDeathInfo;
    public DeathInfo chefDeathInfo;
    public DeathInfo thiefDeathInfo;
    public DeathInfo timerDeathInfo;
    public DeathInfo powerDeathInfo;
    public DeathInfo sixAMDeathInfo;
    public DeathInfo repairedDeathInfo;
}