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
        if (!SystemAPI.TryGetSingleton<MovementSettingsData>(out var movementSettings))
            return;
        if (!SystemAPI.TryGetSingleton<MouseWorldPositionData>(out var mousePosition))
            return;

        // Unity DOTS recommends to allocate values at runtime due to Burst inline compilation (no heap allocation)
        float closeEnoughDistance = movementSettings.CloseEnoughDistance;
        float separationRange = movementSettings.SeparationRange;
        float separationWeight = movementSettings.SeparationWeight;
        float3 targetPosition = mousePosition.Value;

        // Get all units and their transforms to calculate local separation later
        NativeArray<Entity> allUnits = _unitQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> allTransforms = _unitQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

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
                SystemAPI.Time.DeltaTime * moveSpeed.ValueRO.RotateSpeed);

            velocity.ValueRW.Linear = moveDir * moveSpeed.ValueRO.WalkSpeed;
            velocity.ValueRW.Angular = float3.zero;

            // Avoid overlapping with nearby units
            float3 separation = float3.zero;
            int neighborCount = 0;

            for (int i = 0; i < allUnits.Length; i++)
            {
                Entity other = allUnits[i];
                if (other == entity) 
                    continue;

                float3 otherPos = allTransforms[i].Position;
                float3 offset = unitPos - otherPos;
                float dist = math.length(offset);

                // If another unit is within separation range, push away 
                if (dist > 0 && dist < separationRange)
                {
                    separation += offset / dist;
                    neighborCount++;
                }
            }

            // Apply average separation to movement direction
            if (neighborCount > 0)
            {
                separation = separation / neighborCount;
                moveDir = math.normalize(moveDir + separation * separationWeight);
            }
        }

        allUnits.Dispose();
        allTransforms.Dispose();
    }
}
