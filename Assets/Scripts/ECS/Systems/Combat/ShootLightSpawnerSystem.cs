using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS.Systems.Combat
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ShootLightSpawnerSystem : ISystem {


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (RefRO<CastFireball> shootAttack in SystemAPI.Query<RefRO<CastFireball>>()) {

                if (shootAttack.ValueRO.onShoot.isTriggered) {
                    Entity shootLightEntity = state.EntityManager.Instantiate(entitiesReferences.fireballLightPrefabEntity);
                    SystemAPI.SetComponent(shootLightEntity, LocalTransform.FromPosition(shootAttack.ValueRO.onShoot.shootFromPosition));
                }

            }
        }


    }
}