using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS.Authoring.Input
{
    public struct MouseWorldPositionData : IComponentData
    {
        public float3 Value;
    }
}