using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Reference;
using Assets.Scripts.ECS.Authoring.Spawn;
using Assets.Scripts.Monobehaviour.Assets;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Assets.Scripts.ECS.Systems.Spawn
{
    partial struct KnightSpawnerSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
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

                foreach ((RefRO<LocalTransform> localTransform, RefRW<KnightSpawner> knightSpawner)
                         in SystemAPI.Query<RefRO<LocalTransform>, RefRW<KnightSpawner>>())
                {
                    knightSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                    if (knightSpawner.ValueRO.timer > 0f)
                        continue;

                    knightSpawner.ValueRW.timer = knightSpawner.ValueRO.timerMax;

                    float3 origin = localTransform.ValueRO.Position;
                    Random random = Random.CreateFromIndex((uint)origin.GetHashCode() + (uint)UnityEngine.Random.Range(1, 100000));

                    NativeList<float3> existingPositions = new NativeList<float3>(Allocator.Temp);
                    int attempts = 0;
                    float3 spawnPosition = float3.zero;
                    bool positionFound = false;

                    while (attempts < 100)
                    {
                        float radius = knightSpawner.ValueRO.spawnRadiusMultiplier * sceneData.KnightsAmountToSpawn / 100f; 
                        if (radius < knightSpawner.ValueRO.minSpawnRadius)
                        {
                            radius = knightSpawner.ValueRO.minSpawnRadius;
                        }
                        
                        float2 offset2D = random.NextFloat2Direction() * random.NextFloat(0, radius);
                        float3 candidate = origin + new float3(offset2D.x, 0f, offset2D.y);

                        bool isFarEnough = true;
                        foreach (float3 pos in existingPositions)
                        {
                            if (math.distance(candidate, pos) < knightSpawner.ValueRO.minDistanceBetweenUnits)
                            {
                                isFarEnough = false;
                                break;
                            }
                        }

                        if (isFarEnough)
                        {
                            spawnPosition = candidate;
                            existingPositions.Add(candidate);
                            positionFound = true;
                            break;
                        }

                        attempts++;
                    }

                    if (positionFound)
                    {
                        Entity knightEntity = ecb.Instantiate(entitiesReferences.knightPrefabEntity);
                        ecb.SetComponent(knightEntity, LocalTransform.FromPosition(spawnPosition));
                    }

                    existingPositions.Dispose();
                }
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
                    spawnPositions.Dispose();

                    ecb.DestroyEntity(spawnerEntity); // cleanup
                }
            }
        }
    }
}
