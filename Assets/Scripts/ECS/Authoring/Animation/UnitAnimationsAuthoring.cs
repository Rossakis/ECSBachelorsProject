using Unity.Entities;
using UnityEngine;

public class UnitAnimationsAuthoring : MonoBehaviour {
    
    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;
    public AnimationDataSO.AnimationType castFireballAnimationType;
    public AnimationDataSO.AnimationType readySpellAnimationType;
    public AnimationDataSO.AnimationType knightAttackAnimationType;


    public class Baker : Baker<UnitAnimationsAuthoring> {


        public override void Bake(UnitAnimationsAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitAnimations {
                idleAnimationType = authoring.idleAnimationType,
                walkAnimationType = authoring.walkAnimationType,
                castFireballAnimationType = authoring.castFireballAnimationType,
                readySpellAnimationType = authoring.readySpellAnimationType,
                knightAttackAnimationType = authoring.knightAttackAnimationType,
            });
        }

    }
}


public struct UnitAnimations : IComponentData {

    public AnimationDataSO.AnimationType idleAnimationType;
    public AnimationDataSO.AnimationType walkAnimationType;
    public AnimationDataSO.AnimationType castFireballAnimationType;
    public AnimationDataSO.AnimationType readySpellAnimationType;
    public AnimationDataSO.AnimationType knightAttackAnimationType;

}