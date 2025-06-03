using Unity.Entities;
using UnityEngine;

namespace Monobehaviour
{
    /// <summary>
    /// Should only be placed on a single entity only (a type of manager) 
    /// </summary>
    public class MovementSettingsAuthoring : MonoBehaviour
    {
        [Tooltip("Distance from the target at which a unit will consider itself arrived and stop moving.")]
        public float CloseEnoughDistance = 1.5f;
        
        [Tooltip("Radius within which a unit will try to avoid other nearby units to prevent clumping.")]
        public float SeparationRange = 2.5f;
        
        [Tooltip("Strength of the separation force applied to avoid overlapping with nearby units.")]
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