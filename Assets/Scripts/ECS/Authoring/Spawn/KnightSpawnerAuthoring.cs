using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Combat
{
    public class KnightSpawnerAuthoring : MonoBehaviour
    {
        public EcsSceneDataSO sceneData;
        public float spawnRadius;
        public float minDistanceBetweenUnits;
        
        private class KnightSpawnerAuthoringBaker : Baker<KnightSpawnerAuthoring>
        {
            public override void Bake(KnightSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WizardSpawner() {
                    maxUnitsToSpawn = authoring.sceneData.KnightsAmountToSpawn,
                    spawnRadius = authoring.spawnRadius,
                    minDistanceBetweenUnits = authoring.minDistanceBetweenUnits,
                    hasSpawned = false
                });
            }
        }
    }
    
    
    public struct KnightSpawner : IComponentData {
        
        public int maxUnitsToSpawn;
        public float spawnRadius;
        public float minDistanceBetweenUnits;
        public bool hasSpawned;
        
    }
}