using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS.Authoring.Movement
{
    public class UnitMoverAuthoring : MonoBehaviour {
    
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;

        public class Baker : Baker<UnitMoverAuthoring> {

            public override void Bake(UnitMoverAuthoring authoring) {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover {
                    moveSpeed = authoring.moveSpeed,
                    rotationSpeed = authoring.rotationSpeed,
                });
            }

        }

    }

    public struct UnitMover : IComponentData {
        public float moveSpeed;
        public float rotationSpeed;
        public float3 targetPosition;
        public bool isMoving;
    
    }
}