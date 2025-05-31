using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct MoveSystem : ISystem
{
    private float3 _mouseWorld;
    private float _closeEnoughDistance; // when this close to the target position, stop moving/rotating
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _mouseWorld = float3.zero;
        _closeEnoughDistance = 0.8f;
        state.RequireForUpdate<MouseWorldPosition>(); // make sure the singleton exists before we call it
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<MouseWorldPosition>())
            return;

        if(Input.GetMouseButtonDown(0))
            _mouseWorld = SystemAPI.GetSingleton<MouseWorldPosition>().Value;

        // RefRW<> and RefRO<> lets us use the structs (such as LocalTransform) as a reference, and not a value-copy
        foreach (var (localTransform, velocity ,moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRO<MoveSpeed>>())
        {
            float3 direction = math.normalize(new float3( // normalize so unit moves at same speed diagonally
                _mouseWorld.x - localTransform.ValueRO.Position.x,
                0,
                _mouseWorld.z - localTransform.ValueRO.Position.z
            )); 
            var distance = Vector3.Distance(_mouseWorld, localTransform.ValueRO.Position);
            velocity.ValueRW.Angular = float3.zero; // don't allow for physics-based rotations 
            
            if(distance <= _closeEnoughDistance)
            {
                velocity.ValueRW.Linear = float3.zero; 
                return;
            }
            
            localTransform.ValueRW.Rotation = quaternion.LookRotationSafe(direction, Vector3.up);
            velocity.ValueRW.Linear = direction * moveSpeed.ValueRO.WalkSpeed;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
