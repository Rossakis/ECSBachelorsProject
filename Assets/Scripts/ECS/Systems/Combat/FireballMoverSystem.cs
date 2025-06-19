using ECS.Authoring.Reference;
using ECS.Utility;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Systems.Combat
{
    partial struct FireballMoverSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SceneDataReference>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();
            var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach ((
                         RefRW<LocalTransform> localTransform,
                         RefRW<Fireball> fireball,
                         RefRO<Target> target,
                         Entity entity)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<Fireball>,
                         RefRO<Target>>().WithEntityAccess()) {

                float3 targetPosition;

                if (target.ValueRO.targetEntity == Entity.Null) {
                    // Has no target
                    targetPosition = fireball.ValueRO.lastTargetPosition;
                } else {
                    // Has target
                    LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                    ShootVictim targetShootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);
                    targetPosition = targetLocalTransform.TransformPoint(targetShootVictim.hitLocalPosition);
                }

                fireball.ValueRW.lastTargetPosition = targetPosition;

                float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

                float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
                moveDirection = math.normalize(moveDirection);

                localTransform.ValueRW.Position += moveDirection * fireball.ValueRO.speed * SystemAPI.Time.DeltaTime;

                float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

                if (distanceAfterSq > distanceBeforeSq) {
                    // Overshot
                    localTransform.ValueRW.Position = targetPosition;
                }

                float destroyDistanceSq = .2f;
                if (math.distancesq(localTransform.ValueRO.Position, targetPosition) < destroyDistanceSq) {
                    
                    // Close enough to damage target
                    if (target.ValueRO.targetEntity != Entity.Null) {
                        RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                        targetHealth.ValueRW.healthAmount -= fireball.ValueRO.damageAmount;
                        targetHealth.ValueRW.onHealthChanged = true;
                    }
                    
                    if(sceneData.IsObjectPoolingOn)
                        FireballPoolUtility.ResetAndDisableFireball(state.EntityManager, entity);
                    else 
                        entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }


    }
}
