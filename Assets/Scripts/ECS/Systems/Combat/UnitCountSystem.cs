using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Units;
using Unity.Burst;
using Unity.Entities;

namespace Assets.Scripts.ECS.Systems.Combat
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitCountSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Entity unitCountEntity = SystemAPI.GetSingletonEntity<UnitCount>();
            RefRW<UnitCount> unitCount = SystemAPI.GetComponentRW<UnitCount>(unitCountEntity);

            // Count all entities with Knight and Wizard tags
            int knightCount = 0;
            int wizardCount = 0;

            foreach (var _ in SystemAPI.Query<RefRO<Knight>>())
                knightCount++;

            foreach (var _ in SystemAPI.Query<RefRO<Wizard>>())
                wizardCount++;

            unitCount.ValueRW.KnightCount = knightCount;
            unitCount.ValueRW.WizardCount = wizardCount;
        }
    }
}