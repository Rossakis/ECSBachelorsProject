using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Assets.Scripts.ECS.Systems.Events
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    partial struct ResetEventsSystem : ISystem {
    
        private NativeArray<JobHandle> jobHandleNativeArray;
        private NativeList<Entity> onHealthDeadEntityList;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            jobHandleNativeArray = new NativeArray<JobHandle>(3, Allocator.Persistent);
            onHealthDeadEntityList = new NativeList<Entity>(64, Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state) {
        
            jobHandleNativeArray[0] = new ResetSelectedEventsJob().ScheduleParallel(state.Dependency);
            jobHandleNativeArray[1] = new ResetFireballAttackEventsJob().ScheduleParallel(state.Dependency);
        
            onHealthDeadEntityList.Clear();
            new ResetHealthEventsJob() {
                onHealthDeadEntityList = onHealthDeadEntityList.AsParallelWriter(),
            }.ScheduleParallel(state.Dependency).Complete();

            DOTSEventsManager.Instance?.HealthDepleted(onHealthDeadEntityList);
            state.Dependency = JobHandle.CombineDependencies(jobHandleNativeArray);
        }

        public void OnDestroy(ref SystemState state) {
            jobHandleNativeArray.Dispose();
            onHealthDeadEntityList.Dispose();
        }
    }


    [BurstCompile]
    public partial struct ResetFireballAttackEventsJob : IJobEntity {

        public void Execute(ref CastFireball castFireball) {
            castFireball.onShoot.isTriggered = false;
        }
    }


    [BurstCompile]
    public partial struct ResetHealthEventsJob : IJobEntity {
    
        public NativeList<Entity>.ParallelWriter onHealthDeadEntityList;

        public void Execute(ref Health health, Entity entity) {
            if (health.onDead) {
                onHealthDeadEntityList.AddNoResize(entity);
            }

            health.onHealthChanged = false;
            health.onDead = false;
        }

    }


    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct ResetSelectedEventsJob : IJobEntity {
        
        public void Execute(ref Selected selected) {
            selected.onSelected = false;
            selected.onDeselected = false;
        }

    }
}