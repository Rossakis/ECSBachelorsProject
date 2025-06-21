using Assets.Scripts.ECS.Authoring.Movement;
using Assets.Scripts.ECS.Authoring.Reference;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.ECS.Systems.Combat
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    partial struct ResetTargetSystem : ISystem {
        private ComponentLookup<LocalTransform> localTransformComponentLookup;
        private EntityStorageInfoLookup entityStorageInfoLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            localTransformComponentLookup = state.GetComponentLookup<LocalTransform>(true);
            entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
            state.RequireForUpdate<SceneDataReference>();
        }
     
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();

            localTransformComponentLookup.Update(ref state);
            entityStorageInfoLookup.Update(ref state);

            if (sceneData.IsJobSystemOn)
            {
                ResetTargetJob resetTargetJob = new ResetTargetJob {
                    localTransformComponentLookup = localTransformComponentLookup,
                    entityStorageInfoLookup = entityStorageInfoLookup
                };
                resetTargetJob.ScheduleParallel();

                ResetTargetOverrideJob resetTargetOverrideJob = new ResetTargetOverrideJob {
                    localTransformComponentLookup = localTransformComponentLookup,
                    entityStorageInfoLookup = entityStorageInfoLookup
                };
                resetTargetOverrideJob.ScheduleParallel();
            }
            else
            {
                foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>()) {
                    if (target.ValueRW.targetEntity != Entity.Null) {
                        if (!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity)) {
                            target.ValueRW.targetEntity = Entity.Null;
                        }
                    }
                }
                foreach (RefRW<TargetOverride> targetOverride in SystemAPI.Query<RefRW<TargetOverride>>()) {
                    if (targetOverride.ValueRW.targetEntity != Entity.Null) {
                        if (!SystemAPI.Exists(targetOverride.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(targetOverride.ValueRO.targetEntity)) {
                            targetOverride.ValueRW.targetEntity = Entity.Null;
                        }
                    }
                }
            }
        }


    }

    [BurstCompile]
    public partial struct ResetTargetJob : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;

        public void Execute(ref Target target) {
            if (target.targetEntity != Entity.Null) {
                if (!entityStorageInfoLookup.Exists(target.targetEntity) || !localTransformComponentLookup.HasComponent(target.targetEntity)) {
                    target.targetEntity = Entity.Null;
                }
            }
        }

    }

    [BurstCompile]
    public partial struct ResetTargetOverrideJob : IJobEntity {
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformComponentLookup;
        [ReadOnly] public EntityStorageInfoLookup entityStorageInfoLookup;
        
        public void Execute(ref TargetOverride targetOverride) {
            if (targetOverride.targetEntity != Entity.Null) {
                if (!entityStorageInfoLookup.Exists(targetOverride.targetEntity) || !localTransformComponentLookup.HasComponent(targetOverride.targetEntity)) {
                    targetOverride.targetEntity = Entity.Null;
                }
            }
        }

    }
}