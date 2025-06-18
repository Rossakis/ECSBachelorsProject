using ECS.Authoring.Reference;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ECS.Systems.Spawn
{
    partial struct KnightSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
            state.RequireForUpdate<SceneDataReference>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                               .CreateCommandBuffer(state.WorldUnmanaged);

            if (sceneData.IsKnightInfiniteSpawnOn)
            {
                PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

                foreach ((RefRO<LocalTransform> localTransform, RefRW<KnightSpawner> knightSpawner)
                         in SystemAPI.Query<RefRO<LocalTransform>, RefRW<KnightSpawner>>())
                {
                    knightSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                    if (knightSpawner.ValueRO.timer > 0f)
                        continue;

                    knightSpawner.ValueRW.timer = knightSpawner.ValueRO.timerMax;
                    distanceHitList.Clear();

                    CollisionFilter collisionFilter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.UNITS_LAYER,
                        GroupIndex = 0,
                    };

                    int nearbyKnightAmount = 0;

                    if (collisionWorld.OverlapSphere(
                            localTransform.ValueRO.Position,
                            knightSpawner.ValueRO.nearbyKnightAmountDistance,
                            ref distanceHitList,
                            collisionFilter))
                    {
                        foreach (DistanceHit hit in distanceHitList)
                        {
                            if (!SystemAPI.Exists(hit.Entity)) continue;
                            if (SystemAPI.HasComponent<Unit>(hit.Entity) && SystemAPI.HasComponent<Knight>(hit.Entity))
                                nearbyKnightAmount++;
                        }
                    }

                    if (nearbyKnightAmount >= knightSpawner.ValueRO.nearbyKnightAmountMax)
                        continue;

                    Entity knightEntity = state.EntityManager.Instantiate(entitiesReferences.knightPrefabEntity);
                    SystemAPI.SetComponent(knightEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                }

                distanceHitList.Dispose();
            }
            else
            {
                foreach ((RefRW<KnightSpawner> spawner, RefRO<LocalTransform> transform, Entity spawnerEntity)
                         in SystemAPI.Query<RefRW<KnightSpawner>, RefRO<LocalTransform>>().WithEntityAccess())
                {
                    if (spawner.ValueRO.hasSpawned)
                        continue;

                    float3 origin = transform.ValueRO.Position;
                    Random random = Random.CreateFromIndex((uint)origin.GetHashCode());

                    NativeList<float3> spawnPositions = new NativeList<float3>(Allocator.Temp);
                    int attempts = 0;
                    int spawnedCount = 0;

                    while (spawnedCount < sceneData.KnightsAmountToSpawn && attempts < 1000)
                    {
                        float radius = spawner.ValueRO.spawnRadiusMultiplier * sceneData.KnightsAmountToSpawn / 100f; // make radius progressively bigger for bigger amount of knights
                        if (radius < spawner.ValueRO.minSpawnRadius)
                        {
                            radius = spawner.ValueRO.minSpawnRadius; // make sure radius is not smaller than minSpawnRadius
                        }
                        
                        float2 offset2D = random.NextFloat2Direction() * random.NextFloat(0, radius);
                        float3 candidate = origin + new float3(offset2D.x, 0f, offset2D.y);

                        bool isFarEnough = true;
                        foreach (float3 pos in spawnPositions)
                        {
                            if (math.distance(candidate, pos) < spawner.ValueRO.minDistanceBetweenUnits)
                            {
                                isFarEnough = false;
                                break;
                            }
                        }

                        if (isFarEnough)
                        {
                            Entity knight = ecb.Instantiate(entitiesReferences.knightPrefabEntity);
                            ecb.SetComponent(knight, LocalTransform.FromPosition(candidate));
                            spawnPositions.Add(candidate);
                            spawnedCount++;
                        }

                        attempts++;
                    }

                    spawner.ValueRW.hasSpawned = true;
                    ecb.DestroyEntity(spawnerEntity); // cleanup

                    spawnPositions.Dispose();
                }
            }
        }
    }
}
