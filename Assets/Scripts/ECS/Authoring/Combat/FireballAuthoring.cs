using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FireballAuthoring : MonoBehaviour {

    public EcsSceneDataSO sceneData;
    public float speed;

    public class Baker : Baker<FireballAuthoring> {
        
        public override void Bake(FireballAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Fireball {
                speed = authoring.speed,
                damageAmount = authoring.sceneData.WizardDamage,
            });
        }
    }
}



public struct Fireball : IComponentData {
    
    public float speed;
    public int damageAmount;
    public float3 lastTargetPosition;


}