using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Spawn
{
    public class KnightSpawnerAuthoring : MonoBehaviour
    {

        [Header("Infinite Spawn Settings")]
        [SerializeField] private float timerMax;
    
        [Header("One-shot Spawn Settings")]
        [SerializeField] private float minDistanceBetweenUnits;
        [SerializeField] private float spawnRadiusMultiplier = 10;
        [SerializeField] private float minSpawnRadius = 1;
    
        public class Baker : Baker<KnightSpawnerAuthoring> {

            public override void Bake(KnightSpawnerAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new KnightSpawner {
                    timer = authoring.timerMax,
                    timerMax = authoring.timerMax,
                    hasSpawned = false,
                    minDistanceBetweenUnits = authoring.minDistanceBetweenUnits,
                    spawnRadiusMultiplier = authoring.spawnRadiusMultiplier,
                    minSpawnRadius = authoring.minSpawnRadius,
                });
            }
        }
    }


    public struct KnightSpawner : IComponentData {

        // Infinite spawn
        public float timer;
        public float timerMax;
    
        //One - time spawn
        public bool hasSpawned;
        public float minDistanceBetweenUnits;
        public float spawnRadiusMultiplier;
        public float minSpawnRadius;
    }
}