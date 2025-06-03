using Unity.Entities;
using UnityEngine;

namespace Monobehaviour
{
    public class MovementSettingsAuthoring : MonoBehaviour
    {
        public float CloseEnoughDistance = 1.5f;
        public float SeparationRange = 2.5f;
        public float SeparationWeight = 2.0f;
        
        private class Baker : Baker<MovementSettingsAuthoring>
        {
            public override void Bake(MovementSettingsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic); //dynamic = entity will move during runtime
                AddComponent(entity, new MovementSettingsData()
                {
                    CloseEnoughDistance = authoring.CloseEnoughDistance,
                    SeparationRange = authoring.SeparationRange,
                    SeparationWeight = authoring.SeparationWeight,
                });
            }
        }
    }
    
    public struct MovementSettingsData : IComponentData
    {
        public float CloseEnoughDistance;
        public float SeparationRange;
        public float SeparationWeight;
    }
}