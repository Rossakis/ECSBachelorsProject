using ECS.Authoring.Combat;
using ECS.Authoring.Reference;
using ECS.ECS.Utility.ECS.Utility;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Combat
{
    partial struct CastFireballSystem : ISystem {

        private bool isFirstUpdate;
        private EntityQuery fireballQuery;
        
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EntitiesReferences>();
            state.RequireForUpdate<SceneDataReference>();
            
            fireballQuery = state.GetEntityQuery(
                ComponentType.ReadOnly<Fireball>(),
                ComponentType.ReadOnly<FireballPool>());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();
            
            foreach ((
                         RefRW<LocalTransform> localTransform,
                         RefRW<CastFireball> shootAttack,
                         RefRO<Target> target,
                         RefRW<TargetPositionPathQueued> targetPositionPathQueued,
                         EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled,
                         RefRW <UnitMover> unitMover)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<CastFireball>,
                         RefRO<Target>,
                         RefRW<TargetPositionPathQueued>,
                         EnabledRefRW<TargetPositionPathQueued>,
                         RefRW<UnitMover>>().WithDisabled<MoveOverride>().WithPresent<TargetPositionPathQueued>()) {

                if (target.ValueRO.targetEntity == Entity.Null) {
                    continue;
                }

                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance) {
                    // Too far, move closer
                    targetPositionPathQueued.ValueRW.targetPosition = targetLocalTransform.Position;
                    targetPositionPathQueuedEnabled.ValueRW = true;
                    continue;
                } else {
                    // Close enough, stop moving and attack
                    targetPositionPathQueued.ValueRW.targetPosition = localTransform.ValueRO.Position;
                    targetPositionPathQueuedEnabled.ValueRW = true;
                }

                float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);

                quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
                localTransform.ValueRW.Rotation =
                    math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
            }

            foreach ((
                         RefRW<LocalTransform> localTransform,
                         RefRW <CastFireball> fireballAttack,
                         RefRO<Target> target,
                         Entity entity)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<CastFireball>,
                         RefRO<Target>>().WithEntityAccess()) {

                if (target.ValueRO.targetEntity == Entity.Null) {
                    continue;
                }

                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > fireballAttack.ValueRO.attackDistance) {
                    // Target is too far
                    continue;
                }

                if (SystemAPI.HasComponent<MoveOverride>(entity) && SystemAPI.IsComponentEnabled<MoveOverride>(entity)) {
                    // Move override is active
                    continue;
                }

                fireballAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (fireballAttack.ValueRO.timer > 0f) {
                    continue;
                }
                fireballAttack.ValueRW.timer = fireballAttack.ValueRO.timerMax;

                if (SystemAPI.HasComponent<TargetOverride>(target.ValueRO.targetEntity)) {
                    RefRW<TargetOverride> enemyTargetOverride = SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.targetEntity);
                    if (enemyTargetOverride.ValueRO.targetEntity == Entity.Null) {
                        enemyTargetOverride.ValueRW.targetEntity = entity;
                    }
                }

                Entity fireballEntity;
                float3 fireballSpawnPos; 
                
                if (sceneData.IsObjectPoolingOn) 
                {
                    fireballEntity = FireballPoolUtility.GetInactiveFireball(state.EntityManager, fireballQuery);
                    fireballSpawnPos = localTransform.ValueRO.TransformPoint(fireballAttack.ValueRO.fireballSpawnLocalPosition);
                    SystemAPI.SetComponent(fireballEntity, LocalTransform.FromPosition(fireballSpawnPos));
                }
                else
                {
                    fireballEntity = state.EntityManager.Instantiate(entitiesReferences.fireballPrefabEntity);
                    fireballSpawnPos = localTransform.ValueRO.TransformPoint(fireballAttack.ValueRO.fireballSpawnLocalPosition);
                    SystemAPI.SetComponent(fireballEntity, LocalTransform.FromPosition(fireballSpawnPos));
                }

                RefRW<Fireball> fireballObj = SystemAPI.GetComponentRW<Fireball>(fireballEntity);
                fireballObj.ValueRW.damageAmount = fireballAttack.ValueRO.damageAmount;

                RefRW<Target> fireballTarget = SystemAPI.GetComponentRW<Target>(fireballEntity);
                fireballTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;

                fireballAttack.ValueRW.onShoot.isTriggered = true;
                fireballAttack.ValueRW.onShoot.shootFromPosition = fireballSpawnPos;
            }
        }

    }
}