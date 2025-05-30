using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //Spawn 
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // RefRW<> and RefRO<> lets us use the structs (such as LocalTransform) as a reference, and not a value-copy
        foreach (var (localTransform, velocity ,moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<MoveSpeed>>())
        {
            var targetPos = localTransform.ValueRO.Position + new float3(100, 0, 0);
            var direction = targetPos - localTransform.ValueRO.Position;
            direction = math.normalize(direction);
            
            localTransform.ValueRW.Rotation = quaternion.LookRotation(direction, Vector3.up);
            velocity.ValueRW.Linear = direction * moveSpeed.ValueRO.WalkSpeed; 
            velocity.ValueRW.Angular = float3.zero; 
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
