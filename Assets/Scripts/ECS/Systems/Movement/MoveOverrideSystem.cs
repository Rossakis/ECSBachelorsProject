using Assets.Scripts.ECS.Authoring.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.ECS.Systems.Movement
{
    partial struct MoveOverrideSystem : ISystem {


        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach ((
                         RefRO<LocalTransform> localTransform,
                         RefRO<MoveOverride> moveOverride,
                         EnabledRefRW<MoveOverride> moveOverrideEnabled,
                         RefRW<UnitMover> unitMover)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRO<MoveOverride>,
                         EnabledRefRW<MoveOverride>,
                         RefRW<UnitMover>>()) {

                if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) > UnitMoverSystem.TARGET_POSITION_DIFF_DISTANCE_SQ) {
                    // Move closer
                    unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
                } else {
                    // Reached the move override position
                    moveOverrideEnabled.ValueRW = false;
                }
            }
        }


    }
}