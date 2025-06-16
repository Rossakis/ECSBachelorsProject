using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct KnightSpawnerSystem : ISystem {


    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<KnightSpawner> knightSpawner)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<KnightSpawner>>()) {

            knightSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (knightSpawner.ValueRO.timer > 0f) {
                continue;
            }
            knightSpawner.ValueRW.timer = knightSpawner.ValueRO.timerMax;

            distanceHitList.Clear();
            CollisionFilter collisionFilter = new CollisionFilter {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER,
                GroupIndex = 0,
            };

            int nearbyKnightAmount = 0;
            if (collisionWorld.OverlapSphere(
                localTransform.ValueRO.Position,
                knightSpawner.ValueRO.nearbyKnightAmountDistance,
                ref distanceHitList,
                collisionFilter)) {

                foreach (DistanceHit distanceHit in distanceHitList) {
                    if (!SystemAPI.Exists(distanceHit.Entity)) {
                        continue;
                    }
                    if (SystemAPI.HasComponent<Unit>(distanceHit.Entity) && SystemAPI.HasComponent<Knight>(distanceHit.Entity)) {
                        nearbyKnightAmount++;
                    }
                }
            }

            if (nearbyKnightAmount >= knightSpawner.ValueRO.nearbyKnightAmountMax) {
                continue;
            }

            Entity knightEntity = state.EntityManager.Instantiate(entitiesReferences.knightPrefabEntity);
            SystemAPI.SetComponent(knightEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            entityCommandBuffer.AddComponent(knightEntity, new RandomWalking {
                originPosition = localTransform.ValueRO.Position,
                targetPosition = localTransform.ValueRO.Position,
                distanceMin = knightSpawner.ValueRO.randomWalkingDistanceMin,
                distanceMax = knightSpawner.ValueRO.randomWalkingDistanceMax,
                random = new Unity.Mathematics.Random((uint)knightEntity.Index),
            });
            
        }
    }

}