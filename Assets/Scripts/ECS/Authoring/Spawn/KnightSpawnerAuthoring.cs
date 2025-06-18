using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class KnightSpawnerAuthoring : MonoBehaviour
{

    [SerializeField] private float timer;
    [SerializeField] private float timerMax;
    [SerializeField] private int nearbyKnightAmountMax;
    [SerializeField] private float nearbyKnightAmountDistance;
    
    [SerializeField] private float minDistanceBetweenUnits;
    [SerializeField] private float spawnRadiusMultiplier = 10;
    [SerializeField] private float minSpawnRadius = 1;
    
    public class Baker : Baker<KnightSpawnerAuthoring> {

        public override void Bake(KnightSpawnerAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new KnightSpawner {
                timer = authoring.timer,
                timerMax = authoring.timerMax,
                nearbyKnightAmountMax = authoring.nearbyKnightAmountMax,
                nearbyKnightAmountDistance = authoring.nearbyKnightAmountDistance,
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
    public int nearbyKnightAmountMax;
    public float nearbyKnightAmountDistance;
    
    //One - time spawn
    public bool hasSpawned;
    public float minDistanceBetweenUnits;
    public float spawnRadiusMultiplier;
    public float minSpawnRadius;
}