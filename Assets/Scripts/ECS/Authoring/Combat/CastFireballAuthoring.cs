using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Combat
{
    public class CastFireballAuthoring : MonoBehaviour {
    
        public float timerMax;
        public int damageAmount;
        public float attackDistance;
        public Transform fireballSpawnPositionTransform;

        public class Baker : Baker<CastFireballAuthoring> {
            public override void Bake(CastFireballAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CastFireball {
                    timerMax = authoring.timerMax,
                    damageAmount = authoring.damageAmount,
                    attackDistance = authoring.attackDistance,
                    fireballSpawnLocalPosition = authoring.fireballSpawnPositionTransform.localPosition,
                });
            }
        }
    }


    public struct CastFireball : IComponentData {

        public float timer;
        public float timerMax;
        public int damageAmount;
        public float attackDistance;
        public float3 fireballSpawnLocalPosition;
        public OnShootEvent onShoot;

        public struct OnShootEvent {
            public bool isTriggered;
            public float3 shootFromPosition;
        }

    }
}