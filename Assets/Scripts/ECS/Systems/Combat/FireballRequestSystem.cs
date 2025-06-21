using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Movement;
using Assets.Scripts.ECS.Authoring.Reference;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.ECS.Systems.Combat
{
    public partial struct FireballRequestSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<SceneDataReference>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SceneDataReference sceneData = SystemAPI.GetSingleton<SceneDataReference>();
        
        if(!sceneData.IsObjectPoolingOn)
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var fireballQuery = SystemAPI.QueryBuilder()
            .WithAll<FireballPool>()
            .WithDisabled<Fireball>() // Only inactive fireballs
            .Build();

        var fireballEntities = fireballQuery.ToEntityArray(Allocator.Temp);

        int poolIndex = 0;

        foreach (var (transform, cast, target, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<CastFireball>, RefRO<Target>>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null || poolIndex >= fireballEntities.Length)
                continue;

            cast.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (cast.ValueRO.timer > 0f)
                continue;

            cast.ValueRW.timer = cast.ValueRO.timerMax;

            Entity fireball = fireballEntities[poolIndex++];
            float3 spawnPos = transform.ValueRO.TransformPoint(cast.ValueRO.fireballSpawnLocalPosition);

            ecb.SetComponent(fireball, LocalTransform.FromPosition(spawnPos));
            ecb.SetComponentEnabled<Fireball>(fireball, true); // Activate
            ecb.SetComponent(fireball, new Target { targetEntity = target.ValueRO.targetEntity });

            if (SystemAPI.HasComponent<Fireball>(fireball))
            {
                var fireballData = SystemAPI.GetComponentRW<Fireball>(fireball);
                fireballData.ValueRW.damageAmount = cast.ValueRO.damageAmount;
                cast.ValueRW.onShoot.isTriggered = true;
                cast.ValueRW.onShoot.shootFromPosition = spawnPos;
            }
        }

        ecb.Playback(state.EntityManager);
    }
}

}