using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DOTSEventsManager : MonoBehaviour {
    
    public static DOTSEventsManager Instance { get; private set; }

    public event EventHandler OnGameWin;
    public event EventHandler OnPlayerDefeat;
    public event EventHandler OnHealthDepleted;
    public event EventHandler OnKnightArmyStartedSpawning;
    public event EventHandler OnKnightArmyStartSpawningSoon;


    private void Awake() {
        Instance = this;
    }

    public void PlayerDefeat() {
        OnPlayerDefeat?.Invoke(this, EventArgs.Empty);
    }

    public void HealthDepleted(NativeList<Entity> entityNativeList) {
        foreach (Entity entity in entityNativeList) {
            OnHealthDepleted?.Invoke(entity, EventArgs.Empty);
        }
    }

    public void KnightArmyStartedSpawning(NativeList<Entity> entityNativeList) {
        foreach (Entity entity in entityNativeList) {
            OnKnightArmyStartedSpawning?.Invoke(entity, EventArgs.Empty);
        }
    }

    public void KnightArmyStartSpawningSoon(NativeList<Entity> entityNativeList) {
        foreach (Entity entity in entityNativeList) {
            OnKnightArmyStartSpawningSoon?.Invoke(entity, EventArgs.Empty);
        }
    }

    public void GameWin() {
        OnGameWin?.Invoke(this, EventArgs.Empty);
    }

}