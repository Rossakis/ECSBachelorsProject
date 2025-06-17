using Unity.Burst;
using Unity.Entities;

namespace ECS.Systems.Combat
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthDeadTestSystem : ISystem {


        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntityCommandBuffer entityCommandBuffer =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach ((
                         RefRW<Health> health,
                         Entity entity) 
                     in SystemAPI.Query<
                         RefRW<Health>>().WithEntityAccess()) {

                if (health.ValueRO.healthAmount <= 0) {
                    // This entity is dead
                    health.ValueRW.onDead = true;

                    entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }


    }
}