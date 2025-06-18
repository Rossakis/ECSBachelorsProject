using ECS.Authoring.Combat;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS
{
    using Unity.Entities;

    namespace ECS.Utility
    {
        using Unity.Entities;

        namespace ECS.Utility
        {
            public static class FireballPoolUtility
            {
                /// <summary>
                /// Returns an inactive (disabled) fireball entity from the pool, or Entity.Null if none is available.
                /// </summary>
                public static Entity GetInactiveFireball(EntityManager entityManager, EntityQuery fireballQuery)
                {
                    using var entities = fireballQuery.ToEntityArray(Unity.Collections.Allocator.Temp);

                    foreach (var entity in entities)
                    {
                        if (!entityManager.IsComponentEnabled<Fireball>(entity))
                            return entity;
                    }
                    return Entity.Null;
                }
                
                /// <summary>
                /// Disables the fireball and moves it to the offscreen position (0, -1000, 0).
                /// </summary>
                public static void ReturnToPool(EntityCommandBuffer ecb, Entity fireball)
                {
                    ecb.SetComponentEnabled<Fireball>(fireball, false);
                    ecb.SetComponent(fireball, LocalTransform.FromPosition(new float3(0f, -1000f, 0f)));
                }
            }
        }

    }

}