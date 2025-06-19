using ECS.Authoring.Combat;
using ECS.Authoring.Reference;
using ECS.Utility;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Combat
{
    partial struct FireballInstantiateSystem : ISystem
    {
        private EntityQuery fireballQuery;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
            state.RequireForUpdate<SceneDataReference>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            fireballQuery = state.GetEntityQuery(
                ComponentType.ReadOnly<Fireball>(),
                ComponentType.ReadOnly<FireballPool>());
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();
            var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((
                         RefRW<LocalTransform> localTransform,
                         RefRW<CastFireball> shootAttack,
                         RefRO<Target> target,
                         RefRW<TargetPositionPathQueued> targetPositionPathQueued,
                         EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled,
                         RefRW<UnitMover> unitMover)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<CastFireball>,
                         RefRO<Target>,
                         RefRW<TargetPositionPathQueued>,
                         EnabledRefRW<TargetPositionPathQueued>,
                         RefRW<UnitMover>>().WithDisabled<MoveOverride>().WithPresent<TargetPositionPathQueued>())
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;

                var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                float dist = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

                if (dist > shootAttack.ValueRO.attackDistance)
                {
                    // Move closer
                    targetPositionPathQueued.ValueRW.targetPosition = targetLocalTransform.Position;
                    targetPositionPathQueuedEnabled.ValueRW = true;
                    continue;
                }
                else
                {
                    // Stop
                    targetPositionPathQueued.ValueRW.targetPosition = localTransform.ValueRO.Position;
                    targetPositionPathQueuedEnabled.ValueRW = true;
                }

                float3 aimDirection = math.normalize(targetLocalTransform.Position - localTransform.ValueRO.Position);
                quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
            }

            foreach ((
                         RefRW<LocalTransform> localTransform,
                         RefRW<CastFireball> fireballAttack,
                         RefRO<Target> target,
                         Entity entity)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<CastFireball>,
                         RefRO<Target>>().WithEntityAccess())
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;

                var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > fireballAttack.ValueRO.attackDistance)
                    continue;

                if (SystemAPI.HasComponent<MoveOverride>(entity) && SystemAPI.IsComponentEnabled<MoveOverride>(entity))
                    continue;

                fireballAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (fireballAttack.ValueRO.timer > 0f)
                    continue;

                fireballAttack.ValueRW.timer = fireballAttack.ValueRO.timerMax;

                if (SystemAPI.HasComponent<TargetOverride>(target.ValueRO.targetEntity))
                {
                    var enemyTargetOverride = SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.targetEntity);
                    if (enemyTargetOverride.ValueRO.targetEntity == Entity.Null)
                        enemyTargetOverride.ValueRW.targetEntity = entity;
                }

                float3 fireballSpawnPos = localTransform.ValueRO.TransformPoint(fireballAttack.ValueRO.fireballSpawnLocalPosition);
                Entity fireballEntity;

                if (sceneData.IsObjectPoolingOn)
                {
                    fireballEntity = FireballPoolUtility.GetInactiveFireball(state.EntityManager, fireballQuery);

                    if (!state.EntityManager.Exists(fireballEntity))
                        continue;

                    // Reactivate it
                    state.EntityManager.SetComponentEnabled<Fireball>(fireballEntity, true);
                }
                else
                {
                    fireballEntity = state.EntityManager.Instantiate(entitiesReferences.fireballPrefabEntity);
                }

                // Set position
                state.EntityManager.SetComponentData(fireballEntity, LocalTransform.FromPosition(fireballSpawnPos));

                // Set data
                var fireballObj = state.EntityManager.GetComponentData<Fireball>(fireballEntity);
                fireballObj.damageAmount = fireballAttack.ValueRO.damageAmount;
                state.EntityManager.SetComponentData(fireballEntity, fireballObj);

                var fireballTarget = state.EntityManager.GetComponentData<Target>(fireballEntity);
                fireballTarget.targetEntity = target.ValueRO.targetEntity;
                state.EntityManager.SetComponentData(fireballEntity, fireballTarget);

                fireballAttack.ValueRW.onShoot.isTriggered = true;
                fireballAttack.ValueRW.onShoot.shootFromPosition = fireballSpawnPos;
            }
        }
    }
}
