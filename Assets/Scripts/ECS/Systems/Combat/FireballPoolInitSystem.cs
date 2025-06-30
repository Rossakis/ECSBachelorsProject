using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Reference;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS.Systems.Combat
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct FireballPoolInitSystem : ISystem
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
            var sceneData = SystemAPI.GetSingleton<SceneDataReference>();
            
            if(!sceneData.IsObjectPoolingOn)
                return;
            
            var entitiesRefs = SystemAPI.GetSingleton<EntitiesReferences>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var poolRoot = ecb.CreateEntity();
            ecb.AddComponent(poolRoot, new FireballPoolRoot());

            for (int i = 0; i < sceneData.WizardsAmountToSpawn; i++)
            {
                Entity fireball = ecb.Instantiate(entitiesRefs.fireballPrefabEntity);
                ecb.SetComponent(fireball, LocalTransform.FromPosition(new float3(0, 0, 0)));
                ecb.AddComponent(fireball, new FireballPool() { poolRoot = poolRoot });
                ecb.SetComponentEnabled<Fireball>(fireball, false); // Disabled initially
            }

            ecb.Playback(state.EntityManager);
            state.Enabled = false; // Run only once
        }
    }

}