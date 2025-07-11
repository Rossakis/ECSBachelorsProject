using Assets.Scripts.ECS.Authoring.Movement;
using Assets.Scripts.ECS.Authoring.Navigation;
using Assets.Scripts.ECS.Authoring.Reference;
using Assets.Scripts.Monobehaviour.Assets;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Assets.Scripts.ECS.Systems.Movement
{
    partial struct UnitMoverSystem : ISystem {
        // Assume this much difference of length from the final target Position
        public const float TARGET_POSITION_DIFF_DISTANCE_SQ = 2f;
        
        public ComponentLookup<TargetPositionPathQueued> targetPositionPathQueuedComponentLookup;
        public ComponentLookup<FlowFieldPathRequest> flowFieldPathRequestComponentLookup;
        public ComponentLookup<FlowFieldFollower> flowFieldFollowerComponentLookup;
        public ComponentLookup<MoveOverride> moveOverrideComponentLookup;
        public ComponentLookup<Navigation.GridSystem.GridNode> gridNodeComponentLookup;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Navigation.GridSystem.GridSystemData>();
            state.RequireForUpdate<SceneDataReference>();

            targetPositionPathQueuedComponentLookup = SystemAPI.GetComponentLookup<TargetPositionPathQueued>(false);
            flowFieldPathRequestComponentLookup = SystemAPI.GetComponentLookup<FlowFieldPathRequest>(false);
            flowFieldFollowerComponentLookup = SystemAPI.GetComponentLookup<FlowFieldFollower>(false);
            moveOverrideComponentLookup = SystemAPI.GetComponentLookup<MoveOverride>(false);
            gridNodeComponentLookup = SystemAPI.GetComponentLookup<Navigation.GridSystem.GridNode>(false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var gridSystemData = SystemAPI.GetSingleton<Navigation.GridSystem.GridSystemData>();
            var sceneData = SystemAPI.GetSingleton<SceneDataReference>();

            targetPositionPathQueuedComponentLookup.Update(ref state);
            flowFieldPathRequestComponentLookup.Update(ref state);
            flowFieldFollowerComponentLookup.Update(ref state);
            moveOverrideComponentLookup.Update(ref state);
            gridNodeComponentLookup.Update(ref state);

            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorldSingleton.CollisionWorld;

            if (sceneData.IsJobSystemOn)
            {
                new TargetPositionPathQueuedJob
                {
                    collisionWorld = collisionWorld,
                    gridNodeSize = gridSystemData.gridNodeSize,
                    width = gridSystemData.width,
                    height = gridSystemData.height,
                    costMap = gridSystemData.costMap,
                    flowFieldFollowerComponentLookup = flowFieldFollowerComponentLookup,
                    flowFieldPathRequestComponentLookup = flowFieldPathRequestComponentLookup,
                    moveOverrideComponentLookup = moveOverrideComponentLookup,
                    targetPositionPathQueuedComponentLookup = targetPositionPathQueuedComponentLookup
                }.ScheduleParallel();

                new TestCanMoveStraightJob
                {
                    collisionWorld = collisionWorld,
                    flowFieldFollowerComponentLookup = flowFieldFollowerComponentLookup,
                }.ScheduleParallel();

                new FlowFieldFollowerJob
                {
                    width = gridSystemData.width,
                    height = gridSystemData.height,
                    gridNodeSize = gridSystemData.gridNodeSize,
                    gridNodeSizeDouble = gridSystemData.gridNodeSize * 2f,
                    flowFieldFollowerComponentLookup = flowFieldFollowerComponentLookup,
                    totalGridMapEntityArray = gridSystemData.totalGridMapEntityArray,
                    gridNodeComponentLookup = gridNodeComponentLookup,
                }.ScheduleParallel();

                new UnitMoverJob
                {
                    deltaTime = SystemAPI.Time.DeltaTime,
                }.ScheduleParallel();
            }
            else
            {
                foreach (var (unitMover, localTransform, entity) in SystemAPI
                             .Query<RefRW<UnitMover>, RefRW<LocalTransform>>().WithEntityAccess())
                {
                    float3 moveDir = unitMover.ValueRW.targetPosition - localTransform.ValueRO.Position;

                    if (math.lengthsq(moveDir) <= TARGET_POSITION_DIFF_DISTANCE_SQ)
                    {
                        unitMover.ValueRW.isMoving = false;
                        if (SystemAPI.HasComponent<PhysicsVelocity>(entity))
                        {
                            SystemAPI.SetComponent(entity,
                                new PhysicsVelocity { Linear = float3.zero, Angular = float3.zero });
                        }

                        continue;
                    }

                    moveDir = math.normalize(moveDir);
                    unitMover.ValueRW.isMoving = true;

                    quaternion targetRot = quaternion.LookRotation(moveDir, math.up());
                    localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRot,
                        SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

                    // Force rotation to only affect Y axis
                    float3 euler = math.Euler(localTransform.ValueRW.Rotation);
                    euler.x = 0f;
                    euler.z = 0f;
                    localTransform.ValueRW.Rotation = quaternion.Euler(euler);

                    if (SystemAPI.HasComponent<PhysicsVelocity>(entity))
                    {
                        SystemAPI.SetComponent(entity, new PhysicsVelocity
                        {
                            Linear = moveDir * unitMover.ValueRO.moveSpeed,
                            Angular = float3.zero
                        });
                    }
                }

                foreach (var (unitMover, localTransform, entity) in SystemAPI
                             .Query<RefRW<UnitMover>, RefRO<LocalTransform>>()
                             .WithAll<TargetPositionPathQueued>()
                             .WithEntityAccess())
                {
                    float3 targetPos = targetPositionPathQueuedComponentLookup[entity].targetPosition;

                    RaycastInput raycastInput = new RaycastInput
                    {
                        Start = localTransform.ValueRO.Position,
                        End = targetPos,
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << GameAssets.PATHFINDING_WALLS,
                            GroupIndex = 0
                        }
                    };

                    if (!collisionWorld.CastRay(raycastInput))
                    {
                        unitMover.ValueRW.targetPosition = targetPos;
                        SystemAPI.SetComponentEnabled<FlowFieldPathRequest>(entity, false);
                        SystemAPI.SetComponentEnabled<FlowFieldFollower>(entity, false);
                    }
                    else
                    {
                        if (moveOverrideComponentLookup.HasComponent(entity))
                            moveOverrideComponentLookup.SetComponentEnabled(entity, false);

                        if (Navigation.GridSystem.IsValidWalkableGridPosition(targetPos, gridSystemData.width, gridSystemData.height,
                                gridSystemData.costMap, gridSystemData.gridNodeSize))
                        {
                            var pathRequest = flowFieldPathRequestComponentLookup[entity];
                            pathRequest.targetPosition = targetPos;
                            flowFieldPathRequestComponentLookup[entity] = pathRequest;
                            flowFieldPathRequestComponentLookup.SetComponentEnabled(entity, true);
                        }
                        else
                        {
                            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                            flowFieldPathRequestComponentLookup.SetComponentEnabled(entity, false);
                            flowFieldFollowerComponentLookup.SetComponentEnabled(entity, false);
                        }
                    }

                    targetPositionPathQueuedComponentLookup.SetComponentEnabled(entity, false);
                }

                foreach (var (unitMover, localTransform, entity) in SystemAPI
                             .Query<RefRW<UnitMover>, RefRO<LocalTransform>>()
                             .WithAll<FlowFieldFollower>()
                             .WithEntityAccess())
                {
                    FlowFieldFollower follower = flowFieldFollowerComponentLookup[entity];
                    int2 gridPos = Navigation.GridSystem.GetGridPosition(localTransform.ValueRO.Position, gridSystemData.gridNodeSize);
                    int index = Navigation.GridSystem.CalculateIndex(gridPos, gridSystemData.width);
                    int total = gridSystemData.width * gridSystemData.height;

                    Entity gridNodeEntity = gridSystemData.totalGridMapEntityArray[total * follower.gridIndex + index];
                    var gridNode = gridNodeComponentLookup[gridNodeEntity];
                    float3 moveVector = Navigation.GridSystem.GetWorldMovementVector(gridNode.vector);

                    if (Navigation.GridSystem.IsWall(gridNode))
                        moveVector = follower.lastMoveVector;
                    else
                        follower.lastMoveVector = moveVector;

                    float3 targetPos =
                        Navigation.GridSystem.GetWorldCenterPosition(gridPos.x, gridPos.y, gridSystemData.gridNodeSize) +
                        moveVector * (gridSystemData.gridNodeSize * 2f);
                    unitMover.ValueRW.targetPosition = targetPos;

                    if (math.distance(localTransform.ValueRO.Position, follower.targetPosition) <
                        gridSystemData.gridNodeSize)
                    {
                        unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                        flowFieldFollowerComponentLookup.SetComponentEnabled(entity, false);
                    }

                    flowFieldFollowerComponentLookup[entity] = follower;
                }

                foreach (var (unitMover, localTransform, entity) in SystemAPI
                             .Query<RefRW<UnitMover>, RefRO<LocalTransform>>()
                             .WithAll<FlowFieldFollower>()
                             .WithEntityAccess())
                {
                    FlowFieldFollower follower = flowFieldFollowerComponentLookup[entity];

                    RaycastInput raycastInput = new RaycastInput
                    {
                        Start = localTransform.ValueRO.Position,
                        End = follower.targetPosition,
                        Filter = new CollisionFilter
                        {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << GameAssets.PATHFINDING_WALLS,
                            GroupIndex = 0
                        }
                    };

                    if (!collisionWorld.CastRay(raycastInput))
                    {
                        unitMover.ValueRW.targetPosition = follower.targetPosition;
                        flowFieldFollowerComponentLookup.SetComponentEnabled(entity, false);
                    }
                }
            }
        }


        [BurstCompile]
        public partial struct UnitMoverJob : IJobEntity {


            public float deltaTime;


            public void Execute(ref LocalTransform localTransform, ref UnitMover unitMover, ref PhysicsVelocity physicsVelocity) {
                float3 moveDirection = unitMover.targetPosition - localTransform.Position;

                float reachedTargetDistanceSq = UnitMoverSystem.TARGET_POSITION_DIFF_DISTANCE_SQ;
                if (math.lengthsq(moveDirection) <= reachedTargetDistanceSq) {
                    // Reached the target position
                    physicsVelocity.Linear = float3.zero;
                    physicsVelocity.Angular = float3.zero;
                    unitMover.isMoving = false;
                    return;
                }

                unitMover.isMoving = true;

                moveDirection = math.normalize(moveDirection);

                localTransform.Rotation =
                    math.slerp(localTransform.Rotation,
                        quaternion.LookRotation(moveDirection, math.up()),
                        deltaTime * unitMover.rotationSpeed);

                // Force rotation to only affect Y axis
                float3 euler = math.Euler(localTransform.Rotation);
                euler.x = 0f;
                euler.z = 0f;
                localTransform.Rotation = quaternion.Euler(euler);

                physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
                physicsVelocity.Angular = float3.zero;
            }

        }




        [BurstCompile]
        [WithAll(typeof(TargetPositionPathQueued))]
        public partial struct TargetPositionPathQueuedJob : IJobEntity {


            [NativeDisableParallelForRestriction] public ComponentLookup<TargetPositionPathQueued> targetPositionPathQueuedComponentLookup;
            [NativeDisableParallelForRestriction] public ComponentLookup<FlowFieldPathRequest> flowFieldPathRequestComponentLookup;
            [NativeDisableParallelForRestriction] public ComponentLookup<FlowFieldFollower> flowFieldFollowerComponentLookup;
            [NativeDisableParallelForRestriction] public ComponentLookup<MoveOverride> moveOverrideComponentLookup;

            [ReadOnly] public CollisionWorld collisionWorld;
            [ReadOnly] public int width;
            [ReadOnly] public int height;
            [ReadOnly] public NativeArray<int> costMap;
            [ReadOnly] public float gridNodeSize;


            public void Execute(
                in LocalTransform localTransform,
                ref UnitMover unitMover,
                Entity entity) {

                RaycastInput raycastInput = new RaycastInput {
                    Start = localTransform.Position,
                    End = targetPositionPathQueuedComponentLookup[entity].targetPosition,
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.PATHFINDING_WALLS,
                        GroupIndex = 0
                    }
                };

                if (!collisionWorld.CastRay(raycastInput)) {
                    // Did not hit anything, no walls in between
                    unitMover.targetPosition = targetPositionPathQueuedComponentLookup[entity].targetPosition;
                    flowFieldPathRequestComponentLookup.SetComponentEnabled(entity, false);
                    flowFieldFollowerComponentLookup.SetComponentEnabled(entity, false);
                } else {
                    // There's a wall in between
                    if (moveOverrideComponentLookup.HasComponent(entity)) {
                        moveOverrideComponentLookup.SetComponentEnabled(entity, false);
                    }
                    if (Navigation.GridSystem.IsValidWalkableGridPosition(targetPositionPathQueuedComponentLookup[entity].targetPosition, width, height, costMap, gridNodeSize)) {
                        FlowFieldPathRequest flowFieldPathRequest = flowFieldPathRequestComponentLookup[entity];
                        flowFieldPathRequest.targetPosition = targetPositionPathQueuedComponentLookup[entity].targetPosition;
                        flowFieldPathRequestComponentLookup[entity] = flowFieldPathRequest;
                        flowFieldPathRequestComponentLookup.SetComponentEnabled(entity, true);
                    } else {
                        unitMover.targetPosition = localTransform.Position;
                        flowFieldPathRequestComponentLookup.SetComponentEnabled(entity, false);
                        flowFieldFollowerComponentLookup.SetComponentEnabled(entity, false);
                    }
                }

                targetPositionPathQueuedComponentLookup.SetComponentEnabled(entity, false);
            }

        }




        [BurstCompile]
        [WithAll(typeof(FlowFieldFollower))]
        public partial struct TestCanMoveStraightJob : IJobEntity {


            [NativeDisableParallelForRestriction] public ComponentLookup<FlowFieldFollower> flowFieldFollowerComponentLookup;


            [ReadOnly] public CollisionWorld collisionWorld;


            public void Execute(
                in LocalTransform localTransform,
                ref UnitMover unitMover,
                Entity entity) {

                FlowFieldFollower flowFieldFollower = flowFieldFollowerComponentLookup[entity];

                RaycastInput raycastInput = new RaycastInput {
                    Start = localTransform.Position,
                    End = flowFieldFollower.targetPosition,
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.PATHFINDING_WALLS,
                        GroupIndex = 0
                    }
                };

                if (!collisionWorld.CastRay(raycastInput)) {
                    // Did not hit anything, no walls in between
                    unitMover.targetPosition = flowFieldFollower.targetPosition;
                    flowFieldFollowerComponentLookup.SetComponentEnabled(entity, false);
                }
            }
        }




        [BurstCompile]
        [WithAll(typeof(FlowFieldFollower))]
        public partial struct FlowFieldFollowerJob : IJobEntity
        {


            [NativeDisableParallelForRestriction] public ComponentLookup<FlowFieldFollower> flowFieldFollowerComponentLookup;


            [ReadOnly] public ComponentLookup<Navigation.GridSystem.GridNode> gridNodeComponentLookup;
            [ReadOnly] public float gridNodeSize;
            [ReadOnly] public float gridNodeSizeDouble;
            [ReadOnly] public int width;
            [ReadOnly] public int height;
            [ReadOnly] public NativeArray<Entity> totalGridMapEntityArray;


            public void Execute(
                in LocalTransform localTransform,
                ref UnitMover unitMover,
                Entity entity)
            {

                FlowFieldFollower flowFieldFollower = flowFieldFollowerComponentLookup[entity];
                int2 gridPosition = Navigation.GridSystem.GetGridPosition(localTransform.Position, gridNodeSize);
                int index = Navigation.GridSystem.CalculateIndex(gridPosition, width);
                int totalCount = width * height;
                Entity gridNodeEntity = totalGridMapEntityArray[totalCount * flowFieldFollower.gridIndex + index];
                Navigation.GridSystem.GridNode gridNode = gridNodeComponentLookup[gridNodeEntity];
                float3 gridNodeMoveVector = Navigation.GridSystem.GetWorldMovementVector(gridNode.vector);

                if (Navigation.GridSystem.IsWall(gridNode))
                {
                    gridNodeMoveVector = flowFieldFollower.lastMoveVector;
                }
                else
                {
                    flowFieldFollower.lastMoveVector = gridNodeMoveVector;
                }

                unitMover.targetPosition =
                    Navigation.GridSystem.GetWorldCenterPosition(gridPosition.x, gridPosition.y, gridNodeSize) +
                    gridNodeMoveVector *
                    gridNodeSizeDouble;

                if (math.distance(localTransform.Position, flowFieldFollower.targetPosition) < gridNodeSize)
                {
                    // Target destination
                    unitMover.targetPosition = localTransform.Position;
                    flowFieldFollowerComponentLookup.SetComponentEnabled(entity, false);
                }

                flowFieldFollowerComponentLookup[entity] = flowFieldFollower;
            }
        }
    }
}