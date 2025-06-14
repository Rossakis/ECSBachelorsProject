using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FireballVictimAuthoring : MonoBehaviour {
    
    public Transform hitPositionTransform;


    public class Baker : Baker<FireballVictimAuthoring> {

        public override void Bake(FireballVictimAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootVictim {
                hitLocalPosition = authoring.hitPositionTransform.localPosition
            });
        }

    }

}


public struct ShootVictim : IComponentData {

    public float3 hitLocalPosition;
}