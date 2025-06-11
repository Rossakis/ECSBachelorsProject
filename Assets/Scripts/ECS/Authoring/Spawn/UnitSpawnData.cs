using Unity.Entities;
using UnityEngine;

namespace ECS.Components.Spawn
{
    public struct UnitSpawnData : IComponentData
    {
        public Entity Prefab;
        public int Count;
        public float Radius;
    }
    
    public struct UnitTag : IComponentData {}
}