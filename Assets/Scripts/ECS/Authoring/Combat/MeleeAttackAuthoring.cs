using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MeleeAttackAuthoring : MonoBehaviour {
    
    public EcsSceneDataSO sceneData;
    public float timerMax;
    public float colliderSize;
    public float attackDistance = 3f;
    public AnimationDataSO attackAnimation; // take the last frame index of the animation

    public class Baker : Baker<MeleeAttackAuthoring> {

        public override void Bake(MeleeAttackAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MeleeAttack {
                timerMax = authoring.timerMax,
                damageAmount = authoring.sceneData.KnightDamage,
                colliderSize = authoring.colliderSize,
                attackDistance = authoring.attackDistance,
                lastFrameAttack = authoring.attackAnimation.meshArray.Length-1
            });
        }
    }
}



public struct MeleeAttack : IComponentData {

    public float timer;
    public float timerMax;
    public int damageAmount;
    public float colliderSize;
    public bool OnAttack;
    public float attackDistance;
    public int lastFrameAttack;
}