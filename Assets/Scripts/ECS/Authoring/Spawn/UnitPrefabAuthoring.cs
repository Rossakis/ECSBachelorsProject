using Unity.Entities;
using UnityEngine;

namespace ECS.Components.Spawn
{
    public struct UnitPrefabReference : IComponentData
    {
        public Entity Prefab;
    }
    
    public class UnitPrefabAuthoring : MonoBehaviour
    {
        public GameObject UnitPrefab;

        public class Baker : Baker<UnitPrefabAuthoring>
        {
            public override void Bake(UnitPrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var prefabEntity = GetEntity(authoring.UnitPrefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new UnitPrefabReference
                {
                    Prefab = prefabEntity
                });
            }
        }
    }
}