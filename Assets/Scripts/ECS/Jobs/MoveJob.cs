using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
    public partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;
        public float3 TargetPosition;
        public float CloseEnoughDistance;
        public float SeparationRange;
        public float SeparationWeight;

        [ReadOnly] public NativeArray<Entity> AllEntities;
        [ReadOnly] public NativeArray<LocalTransform> AllTransforms;

        public void Execute(Entity entity, ref LocalTransform transform, ref PhysicsVelocity velocity, in MoveSpeed speed, ref MoveStateData state)
        {
            float3 unitPos = transform.Position;
            float3 toTarget = TargetPosition - unitPos;
            float distance = math.length(toTarget);

            if (distance <= CloseEnoughDistance)
            {
                state.State = MoveStateData.MoveState.Arrived;
                velocity.Linear = float3.zero;
                return;
            }

            float3 moveDir = math.normalize(toTarget);

            float3 separation = float3.zero;
            int neighborCount = 0;

            for (int i = 0; i < AllEntities.Length; i++)
            {
                if (AllEntities[i] == entity) continue;

                float3 otherPos = AllTransforms[i].Position;
                float3 offset = unitPos - otherPos;
                float dist = math.length(offset);

                if (dist > 0 && dist < SeparationRange)
                {
                    separation += offset / dist;
                    neighborCount++;
                }
            }

            if (neighborCount > 0)
            {
                separation = separation / neighborCount;
                moveDir = math.normalize(moveDir + separation * SeparationWeight);
            }

            transform.Rotation = math.slerp(
                transform.Rotation,
                quaternion.LookRotation(moveDir, math.up()),
                DeltaTime * speed.RotateSpeed);

            velocity.Linear = moveDir * speed.WalkSpeed;
            velocity.Angular = float3.zero;
        }
    }