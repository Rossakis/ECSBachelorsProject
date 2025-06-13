using Unity.Entities;
using UnityEngine;

public class UnitAnimationsAuthoring : MonoBehaviour {
    
    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;
    public AnimationDataSO.AnimationType castFireballAnimationType;
    public AnimationDataSO.AnimationType readySpellAnimationType;


    public class Baker : Baker<UnitAnimationsAuthoring> {


        public override void Bake(UnitAnimationsAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitAnimations {
                idleAnimationType = authoring.idleAnimationType,
                walkAnimationType = authoring.walkAnimationType,
                castFireballAnimationType = authoring.castFireballAnimationType,
                readySpellAnimationType = authoring.readySpellAnimationType,
            });
        }

    }
}


public struct UnitAnimations : IComponentData {

    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;
    public AnimationDataSO.AnimationType castFireballAnimationType;
    public AnimationDataSO.AnimationType readySpellAnimationType;

}