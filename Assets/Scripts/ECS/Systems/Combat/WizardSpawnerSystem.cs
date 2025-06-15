using ECS.Authoring.Combat;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct UnitSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WizardSpawner>();
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = 
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRW<WizardSpawner> spawner, RefRO<LocalTransform> transform, Entity spawnerEntity) in
                 SystemAPI.Query<RefRW<WizardSpawner>, RefRO<LocalTransform>>().WithEntityAccess())
        {
            if (spawner.ValueRO.hasSpawned)
                continue;

            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            float3 origin = transform.ValueRO.Position;
            Random random = Random.CreateFromIndex((uint)origin.GetHashCode());

            NativeList<float3> spawnPositions = new NativeList<float3>(Allocator.Temp);

            int attempts = 0;
            int spawnedCount = 0;

            while (spawnedCount < spawner.ValueRO.maxUnitsToSpawn && attempts < 1000)
            {
                float2 randomOffset2D = random.NextFloat2Direction() * random.NextFloat(0, spawner.ValueRO.spawnRadius);
                float3 candidatePos = origin + new float3(randomOffset2D.x, 0, randomOffset2D.y);

                bool isFarEnough = true;
                foreach (float3 existing in spawnPositions)
                {
                    if (math.distance(candidatePos, existing) < spawner.ValueRO.minDistanceBetweenUnits)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                {
                    Entity unit = ecb.Instantiate(entitiesReferences.wizardPrefabGameObject);
                    ecb.SetComponent(unit, LocalTransform.FromPosition(candidatePos));
                    spawnPositions.Add(candidatePos);
                    spawnedCount++;
                }

                attempts++;
            }

            spawner.ValueRW.hasSpawned = true;

            // Destroy spawner entity after its job is completed
            ecb.DestroyEntity(spawnerEntity); 
        }
    }
}
