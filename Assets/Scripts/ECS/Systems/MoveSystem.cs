using Monobehaviour;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public partial struct UnitMoveSystem : ISystem
{
    private float3 _targetPosition;
    private float _closeEnoughDistance;
    private float _separationRange;
    private float _separationWeight;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MouseWorldPositionData>();
        state.RequireForUpdate<MovementSettingsData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _closeEnoughDistance = SystemAPI.GetSingleton<MovementSettingsData>().CloseEnoughDistance;
        _separationRange = SystemAPI.GetSingleton<MovementSettingsData>().SeparationRange;
        _separationWeight = SystemAPI.GetSingleton<MovementSettingsData>().SeparationWeight;
        
        EntityQuery unitQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<LocalTransform>());

        NativeArray<Entity> allUnits = unitQuery.ToEntityArray(Allocator.Temp);
        NativeArray<LocalTransform> allTransforms = unitQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

        foreach (var (transform, velocity, moveSpeed, moveTag, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<MoveSpeed>, RefRW<MoveStateData>>()
                          .WithEntityAccess())
        {
            _targetPosition= SystemAPI.GetSingleton<MouseWorldPositionData>().Value;
            float3 unitPos = transform.ValueRO.Position;
            float3 toTarget = _targetPosition - unitPos;
            float distance = math.length(toTarget);

            if (distance <= _closeEnoughDistance)
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

            // Apply local separation
            float3 separation = float3.zero;
            int neighborCount = 0;

            for (int i = 0; i < allUnits.Length; i++)
            {
                Entity other = allUnits[i];
                if (other == entity) continue;

                float3 otherPos = allTransforms[i].Position;
                float3 offset = unitPos - otherPos;
                float dist = math.length(offset);

                if (dist > 0 && dist < _separationRange)
                {
                    separation += offset / dist;
                    neighborCount++;
                }
            }

            if (neighborCount > 0)
            {
                separation = separation / neighborCount;
                moveDir = math.normalize(moveDir + separation * _separationWeight);
            }
        }

        allUnits.Dispose();
        allTransforms.Dispose();
    }
}
