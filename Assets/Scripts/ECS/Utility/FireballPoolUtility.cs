using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Utility
{
    public static class FireballPoolUtility
    {
        public static Entity GetInactiveFireball(EntityManager entityManager, EntityQuery query)
        {
            using var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                if (!entityManager.IsComponentEnabled<Fireball>(entity))
                    return entity;
            }

            return Entity.Null;
        }

        public static void ResetAndDisableFireball(EntityManager entityManager, Entity fireballEntity)
        {
            if (!entityManager.Exists(fireballEntity))
                return;

            entityManager.SetComponentEnabled<Fireball>(fireballEntity, false);
            entityManager.SetComponentData(fireballEntity, LocalTransform.FromPosition(new float3(0f, -1000f, 0f)));
        }
    }
}