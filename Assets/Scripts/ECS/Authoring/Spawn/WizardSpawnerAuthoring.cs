using Assets.Scripts.ScriptableObjects.Scene;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Spawn
{
    public class WizardSpawnerAuthoring : MonoBehaviour
    {
        public EcsSceneDataSO sceneData;
        public float spawnRadius;
        public float minDistanceBetweenUnits;
        
        private class WizardSpawnerAuthoringBaker : Baker<WizardSpawnerAuthoring>
        {
            public override void Bake(WizardSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WizardSpawner() {
                    maxUnitsToSpawn = authoring.sceneData.WizardsAmountToSpawn,
                    spawnRadius = authoring.spawnRadius,
                    minDistanceBetweenUnits = authoring.minDistanceBetweenUnits,
                    hasSpawned = false
                });
            }
        }
    }
    
    
    public struct WizardSpawner : IComponentData {
        
        public int maxUnitsToSpawn;
        public float spawnRadius;
        public float minDistanceBetweenUnits;
        public bool hasSpawned;

    }
}