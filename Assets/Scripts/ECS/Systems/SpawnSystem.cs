using ECS.Components.Spawn;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
public partial struct UnitSpawnSystem : ISystem
{
    private EntityQuery _spawnQuery;

    public void OnCreate(ref SystemState state)
    {
        // Create and cache the query to avoid building it every frame
        _spawnQuery = state.GetEntityQuery(ComponentType.ReadOnly<UnitSpawnData>());
        state.RequireForUpdate(_spawnQuery);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Exit early if there’s no spawn command
        if (_spawnQuery.IsEmptyIgnoreFilter)
            return;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity commandEntity = _spawnQuery.GetSingletonEntity();
        var command = state.EntityManager.GetComponentData<UnitSpawnData>(commandEntity);

        var rng = new Unity.Mathematics.Random((uint)(SystemAPI.Time.ElapsedTime * 1000) + 1);
        
        for (int i = 0; i < command.Count; i++)
        {
            Entity instance = ecb.Instantiate(command.Prefab);

            float angle = rng.NextFloat(0f, math.PI * 2f);
            float distance = rng.NextFloat(0f, command.Radius);

            float3 position = new float3(
                math.cos(angle) * distance,
                0f,
                math.sin(angle) * distance
            );

            ecb.SetComponent(instance, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            ecb.AddComponent(instance, new UnitTag());
        }
        
        ecb.DestroyEntity(commandEntity); // Prevent system from repeating every frame
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}