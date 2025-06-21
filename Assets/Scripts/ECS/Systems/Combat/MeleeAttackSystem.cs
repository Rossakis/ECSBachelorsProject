using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Movement;
using Assets.Scripts.Monobehaviour.Assets;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Assets.Scripts.ECS.Systems.Combat
{
    partial struct MeleeAttackSystem : ISystem {

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            NativeList<RaycastHit> raycastHitList = new NativeList<RaycastHit>(Allocator.Temp);

            foreach ((
                         RefRO<LocalTransform> localTransform,
                         RefRW<MeleeAttack> meleeAttack,
                         RefRO<Target> target,
                         RefRW<TargetPositionPathQueued> targetPositionPathQueued,
                         EnabledRefRW<TargetPositionPathQueued> targetPositionPathQueuedEnabled)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<MeleeAttack>,
                         RefRO<Target>,
                         RefRW<TargetPositionPathQueued>,
                         EnabledRefRW<TargetPositionPathQueued>>().WithDisabled<MoveOverride>().WithPresent<TargetPositionPathQueued>()) {

                if (target.ValueRO.targetEntity == Entity.Null) {
                    continue;
                }
                
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                bool isCloseEnoughToAttack = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) < meleeAttack.ValueRO.attackDistance;
                bool isTouchingTarget = false;
                
                if (!isCloseEnoughToAttack) 
                {
                    float3 dirToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                    dirToTarget = math.normalize(dirToTarget);
                    float distanceExtraToTestRaycast = .4f;

                    RaycastInput raycastInput = new RaycastInput {
                        Start = localTransform.ValueRO.Position,
                        End = localTransform.ValueRO.Position + dirToTarget * (meleeAttack.ValueRO.colliderSize + distanceExtraToTestRaycast),
                        Filter = new CollisionFilter {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << GameAssets.UNITS_LAYER,
                            GroupIndex = 0,
                        },
                    };

                    raycastHitList.Clear();
                    if (collisionWorld.CastRay(raycastInput, ref raycastHitList)) {
                        foreach (RaycastHit raycastHit in raycastHitList) {
                            if (raycastHit.Entity == target.ValueRO.targetEntity) {
                                // Raycast hit target, close enough to attack this entity
                                isTouchingTarget = true;
                                break;
                            }
                        }
                    }
                }

                // Target is too far
                if (!isCloseEnoughToAttack && !isTouchingTarget) 
                {
                    targetPositionPathQueued.ValueRW.targetPosition = targetLocalTransform.Position;
                    targetPositionPathQueuedEnabled.ValueRW = true;
                } 
                // Target is close enough to attack
                else 
                {
                    targetPositionPathQueued.ValueRW.targetPosition = localTransform.ValueRO.Position;
                    targetPositionPathQueuedEnabled.ValueRW = true;

                    meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                    if (meleeAttack.ValueRO.timer > 0) {
                        continue;
                    }
                    meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;

                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;
                    targetHealth.ValueRW.onHealthChanged = true;

                    meleeAttack.ValueRW.OnAttack = true;
                }
            }
        }



    }
}