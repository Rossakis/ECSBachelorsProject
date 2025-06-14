using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public class CastAttackAuthoring : MonoBehaviour {
        
    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public Transform fireballSpawnPositionTransform;
        
    public class Baker : Baker<CastAttackAuthoring> {


        public override void Bake(CastAttackAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CastAttack {
                timerMax = authoring.timerMax,
                damageAmount = authoring.damageAmount,
                attackDistance = authoring.attackDistance,
                fireballSpawnPositionTransform = authoring.fireballSpawnPositionTransform.localPosition,
            });
        }


    }
}
    
public struct CastAttack : IComponentData {

    public float timer;
    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public float3 fireballSpawnPositionTransform;
    public OnShootEvent onShoot;

    public struct OnShootEvent {
        public bool isTriggered;
        public float3 castFromPosition;
    }

}