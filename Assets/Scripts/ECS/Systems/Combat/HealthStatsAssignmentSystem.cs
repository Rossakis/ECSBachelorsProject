using ECS.Authoring.Reference;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Systems.Combat
{
    /// <summary>
    /// Because Knights and Wizard have different HP (while having same Health component), we need to assign it with a system's logic at runtime
    /// </summary>
    public partial struct HealthStatsAssignmentSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Health>();
            state.RequireForUpdate<Faction>();
            state.RequireForUpdate<SceneDataReference>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Only run for entities that have Faction and Health, but not the initialization tag
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();
            
            foreach (var (health, faction, entity) in SystemAPI
                         .Query<RefRW<Health>, RefRO<Faction>>()
                         .WithNone<HealthInitializedTag>()
                         .WithEntityAccess())
            {
                switch (faction.ValueRO.factionType)
                {
                    case FactionType.Knight:
                        health.ValueRW.healthAmountMax = sceneData.KnightMaxHealth;
                        health.ValueRW.healthAmount = sceneData.KnightMaxHealth;
                        break;
                    case FactionType.Wizard:
                        health.ValueRW.healthAmountMax = sceneData.WizardMaxHealth;
                        health.ValueRW.healthAmount = sceneData.WizardMaxHealth;
                        break;
                }

                health.ValueRW.onHealthChanged = true;

                // Prevent future re-initialization
                ecb.AddComponent<HealthInitializedTag>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    public struct HealthInitializedTag : IComponentData { }
    
}