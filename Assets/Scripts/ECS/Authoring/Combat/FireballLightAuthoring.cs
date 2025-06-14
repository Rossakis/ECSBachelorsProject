using Unity.Entities;
using UnityEngine;

public class FireballLightAuthoring : MonoBehaviour {


    public float timer;


    public class Baker : Baker<FireballLightAuthoring> {
        
        public override void Bake(FireballLightAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FireballLight {
                timer = authoring.timer,
            });
        }
    }


}



public struct FireballLight : IComponentData {

    public float timer;

}