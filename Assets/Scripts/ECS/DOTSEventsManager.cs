using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class DOTSEventsManager : MonoBehaviour {
    
        public static DOTSEventsManager Instance { get; private set; }
        public event EventHandler OnHealthDepleted;

        private void Awake() {
            Instance = this;
        }

        public void HealthDepleted(NativeList<Entity> entityNativeList) {
            foreach (Entity entity in entityNativeList) {
                OnHealthDepleted?.Invoke(entity, EventArgs.Empty);
            }
        }

    }
}