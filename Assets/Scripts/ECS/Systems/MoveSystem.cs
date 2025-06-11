using Monobehaviour;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct UnitMoveSystem : ISystem
{
    // Cached query to get all unit transforms in the world.
    private EntityQuery _unitQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MouseWorldPositionData>();
        state.RequireForUpdate<MovementSettingsData>();

        _unitQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<LocalTransform>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<MovementSettingsData>(out var settings) ||
            !SystemAPI.TryGetSingleton<MouseWorldPositionData>(out var mouse))
            return;

        float deltaTime = SystemAPI.Time.DeltaTime;

        float3 targetPosition = mouse.Value;
        float closeEnoughDistance = settings.CloseEnoughDistance;
        float separationRange = settings.SeparationRange;
        float separationWeight = settings.SeparationWeight;

        NativeArray<LocalTransform> allTransforms = _unitQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        NativeArray<Entity> allEntities = _unitQuery.ToEntityArray(Allocator.TempJob);

        bool useJob = UIManager.isJobEnabled;
        
        if (useJob)
        {
            // Run job-based system
            var job = new MoveJob
            {
                DeltaTime = deltaTime,
                TargetPosition = targetPosition,
                CloseEnoughDistance = closeEnoughDistance,
                SeparationRange = separationRange,
                SeparationWeight = separationWeight,
                AllEntities = allEntities,
                AllTransforms = allTransforms
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
            state.Dependency.Complete();
        }
        else
        {// Run traditional foreach approach
            foreach (var (transform, velocity, moveSpeed, moveTag, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<MoveSpeed>, RefRW<MoveStateData>>()
                              .WithEntityAccess())
            {
                float3 unitPos = transform.ValueRO.Position;
                float3 toTarget = targetPosition - unitPos;
                float distance = math.length(toTarget);

                if (distance <= closeEnoughDistance)
                {
                    moveTag.ValueRW.State = MoveStateData.MoveState.Arrived;
                    velocity.ValueRW.Linear = float3.zero;
                    continue;
                }

                float3 moveDir = math.normalize(toTarget);

                transform.ValueRW.Rotation = math.slerp(
                    transform.ValueRO.Rotation,
                    quaternion.LookRotation(moveDir, math.up()),
                    deltaTime * moveSpeed.ValueRO.RotateSpeed);

                velocity.ValueRW.Linear = moveDir * moveSpeed.ValueRO.WalkSpeed;
                velocity.ValueRW.Angular = float3.zero;

                float3 separation = float3.zero;
                int neighborCount = 0;

                for (int i = 0; i < allEntities.Length; i++)
                {
                    if (allEntities[i] == entity) continue;

                    float3 otherPos = allTransforms[i].Position;
                    float3 offset = unitPos - otherPos;
                    float dist = math.length(offset);

                    if (dist > 0 && dist < separationRange)
                    {
                        separation += offset / dist;
                        neighborCount++;
                    }
                }

                if (neighborCount > 0)
                {
                    separation = separation / neighborCount;
                    moveDir = math.normalize(moveDir + separation * separationWeight);
                }
            }
            
        }
    }
}
