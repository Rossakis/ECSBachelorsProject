using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Combat
{
    public class UnitCountAuthoring : MonoBehaviour
    {
        private class UnitCountAuthoringBaker : Baker<UnitCountAuthoring>
        {
            public override void Bake(UnitCountAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitCount());
            }
        }
    }
    
    public struct UnitCount : IComponentData
    {
        public int KnightCount;
        public int WizardCount;
    }
}