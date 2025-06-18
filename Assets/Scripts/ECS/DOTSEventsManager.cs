using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DOTSEventsManager : MonoBehaviour {
    
    public static DOTSEventsManager Instance { get; private set; }

    public event EventHandler OnGameWin;
    public event EventHandler OnGameOver;
    public event EventHandler OnHealthDepleted;
    
    //TODO:
    public GameObject GameWinPanel;
    public GameObject GameOverPanel;


    private void Awake() {
        Instance = this;
    }

    public void GameOver() {
        OnGameOver?.Invoke(this, EventArgs.Empty);
    }

    public void HealthDepleted(NativeList<Entity> entityNativeList) {
        foreach (Entity entity in entityNativeList) {
            OnHealthDepleted?.Invoke(entity, EventArgs.Empty);
        }
    }
    
    public void GameWin() {
        OnGameWin?.Invoke(this, EventArgs.Empty);
    }

}