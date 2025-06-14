using Unity.Entities;
using UnityEngine;

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