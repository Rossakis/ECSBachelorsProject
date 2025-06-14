using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FireballAuthoring : MonoBehaviour {


    [SerializeField] private float speed;
    [SerializeField] private int damageAmount;


    public class Baker : Baker<FireballAuthoring> {


        public override void Bake(FireballAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Fireball {
                speed = authoring.speed,
                damageAmount = authoring.damageAmount,
            });
        }

    }

}



public struct Fireball : IComponentData {
    
    public float speed;
    public int damageAmount;
    public float3 lastTargetPosition;


}