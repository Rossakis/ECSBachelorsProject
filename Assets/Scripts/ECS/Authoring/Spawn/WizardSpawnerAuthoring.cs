using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace ECS.Authoring.Combat
{
    public class WizardSpawnerAuthoring : MonoBehaviour
    {
        public bool HasSpawned;
        
        [SerializeField] private int maxUnitsToSpawn;
        [SerializeField] private float spawnRadius;
        [SerializeField] private float minDistanceBetweenUnits;
        
        private class WizardSpawnerAuthoringBaker : Baker<WizardSpawnerAuthoring>
        {
            public override void Bake(WizardSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WizardSpawner() {
                    maxUnitsToSpawn = authoring.maxUnitsToSpawn,
                    spawnRadius = authoring.spawnRadius,
                    minDistanceBetweenUnits = authoring.minDistanceBetweenUnits,
                    hasSpawned = authoring.HasSpawned
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