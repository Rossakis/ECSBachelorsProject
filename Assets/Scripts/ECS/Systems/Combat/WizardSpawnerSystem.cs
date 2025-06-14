using ECS.Authoring.Combat;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine.InputSystem;

namespace ECS.Systems.Combat
{
    public partial struct WizardSpawnerSystem : ISystem
    {
        [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
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
            RefRW<WizardSpawner> wizardSpawner)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<WizardSpawner>>()) {

            wizardSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (wizardSpawner.ValueRO.timer > 0f) {
                continue;
            }
            wizardSpawner.ValueRW.timer = wizardSpawner.ValueRO.timerMax;

            distanceHitList.Clear();
            CollisionFilter collisionFilter = new CollisionFilter {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.PATHFINDING_WALLS,
                GroupIndex = 0,
            };

            int nearbyWizardAmount = 0;
            if (collisionWorld.OverlapSphere(
                localTransform.ValueRO.Position,
                wizardSpawner.ValueRO.nearbyWizardAmountDistance,
                ref distanceHitList,
                collisionFilter)) {

                foreach (DistanceHit distanceHit in distanceHitList) {
                    if (!SystemAPI.Exists(distanceHit.Entity)) {
                        continue;
                    }
                    if (SystemAPI.HasComponent<Unit>(distanceHit.Entity) && SystemAPI.HasComponent<Wizard>(distanceHit.Entity)) {
                        nearbyWizardAmount++;
                    }
                }
            }

            if (nearbyWizardAmount >= wizardSpawner.ValueRO.nearbyWizardAmountDistance) {
                continue;
            }

            Entity wizardEntity = state.EntityManager.Instantiate(entitiesReferences.wizardPrefabGameObject);
            SystemAPI.SetComponent(wizardEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            
            // entityCommandBuffer.AddComponent(wizardEntity, new RandomWalking {
            //     originPosition = localTransform.ValueRO.Position,
            //     targetPosition = localTransform.ValueRO.Position,
            //     distanceMin = wizardSpawner.ValueRO.randomWalkingDistanceMin,
            //     distanceMax = wizardSpawner.ValueRO.randomWalkingDistanceMax,
            //     random = new Unity.Mathematics.Random((uint)wizardEntity.Index),
            // });
        }
    }
    }
}