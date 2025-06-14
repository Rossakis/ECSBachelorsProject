using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring.Combat
{
    public class WizardAuthoring : MonoBehaviour
    {
        private class WizardAuthoringBaker : Baker<WizardAuthoring>
        {
            public override void Bake(WizardAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Wizard());
            }
        }
    }
    
    public struct Wizard : IComponentData {
    }
}