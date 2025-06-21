using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Combat
{
    public class KnightAuthoring : MonoBehaviour {
    
        public class Baker : Baker<KnightAuthoring> {
        
            public override void Bake(KnightAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Knight());
            }
        }
    }




    public struct Knight : IComponentData {
    }
}