using System;
using Assets.Scripts.ECS.Authoring.Combat;
using Assets.Scripts.ECS.Authoring.Movement;
using Assets.Scripts.ECS.Authoring.Navigation;
using Assets.Scripts.Monobehaviour.Assets;
using Assets.Scripts.Monobehaviour.Input;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class UnitSelectionManager : MonoBehaviour {

        public static UnitSelectionManager Instance { get; private set; }
        public float ringSize = 1f;

        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;
        public event EventHandler OnSelectedEntitiesChanged;


        private Vector2 selectionStartMousePosition;


        private void Awake() {
            Instance = this;
        }

        private void Update() {

            if (Input.GetMouseButtonUp(0)) {
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
            
                // Deselect all
                for (int i = 0; i < entityArray.Length; i++) {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                    Selected selected = selectedArray[i];
                    selected.onDeselected = true;
                    entityManager.SetComponentData(entityArray[i], selected);
                }


                Rect selectionAreaRect = GetSelectionAreaRect();
                float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
                float multipleSelectionSizeMin = 40f;
                bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;

                if (isMultipleSelection) {
                    entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);

                    entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                    NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                    for (int i = 0; i < localTransformArray.Length; i++) {
                        LocalTransform unitLocalTransform = localTransformArray[i];
                        Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                        if (selectionAreaRect.Contains(unitScreenPosition)) {
                            // Unit is inside the selection area
                            entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                            Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                            selected.onSelected = true;
                            entityManager.SetComponentData(entityArray[i], selected);
                        }
                    }
                } else {
                    // Single select
                    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                    UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastInput raycastInput = new RaycastInput {
                        Start = cameraRay.GetPoint(0f),
                        End = cameraRay.GetPoint(9999f),
                        Filter = new CollisionFilter {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u,
                            GroupIndex = 0,
                        }
                    };
                    if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) {
                        if (entityManager.HasComponent<Selected>(raycastHit.Entity)) {
                            // Hit a Selectable entity
                            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                            Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                            selected.onSelected = true;
                            entityManager.SetComponentData(raycastHit.Entity, selected);
                        }
                    }
                }
            
                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
                OnSelectedEntitiesChanged?.Invoke(this, EventArgs.Empty);
            }

            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                selectionStartMousePosition = Input.mousePosition;

                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetMouseButtonDown(1)) {
                Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastInput raycastInput = new RaycastInput {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u,
                        GroupIndex = 0,
                    }
                };
                bool isAttackingSingleTarget = false;
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) {
                    if (entityManager.HasComponent<Faction>(raycastHit.Entity)) {
                        // Hit something with a Faction
                        Faction faction = entityManager.GetComponentData<Faction>(raycastHit.Entity);
                        if (faction.factionType == FactionType.Knight) {
                            // Right clicking on a Knight
                            isAttackingSingleTarget = true;

                            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<TargetOverride>().Build(entityManager);

                            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                            NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
                            for (int i = 0; i < targetOverrideArray.Length; i++) {
                                TargetOverride targetOverride = targetOverrideArray[i];
                                targetOverride.targetEntity = raycastHit.Entity;
                                targetOverrideArray[i] = targetOverride;
                                entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], false);
                            }
                            entityQuery.CopyFromComponentDataArray(targetOverrideArray);
                        }
                    }
                }

                if (!isAttackingSingleTarget) {
                    entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>()
                        .WithPresent<MoveOverride, TargetOverride, TargetPositionPathQueued, FlowFieldPathRequest, FlowFieldFollower>().Build(entityManager);

                    NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                    NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);
                    NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
                    NativeArray<TargetPositionPathQueued> targetPositionPathQueuedArray = entityQuery.ToComponentDataArray<TargetPositionPathQueued>(Allocator.Temp);
                    NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);
                
                    for (int i = 0; i < moveOverrideArray.Length; i++) {
                        MoveOverride moveOverride = moveOverrideArray[i];
                        moveOverride.targetPosition = movePositionArray[i];
                        moveOverrideArray[i] = moveOverride;
                        entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);

                        TargetOverride targetOverride = targetOverrideArray[i];
                        targetOverride.targetEntity = Entity.Null;
                        targetOverrideArray[i] = targetOverride;

                        TargetPositionPathQueued targetPositionPathQueued = targetPositionPathQueuedArray[i];
                        targetPositionPathQueued.targetPosition = movePositionArray[i];
                        targetPositionPathQueuedArray[i] = targetPositionPathQueued;
                        entityManager.SetComponentEnabled<TargetPositionPathQueued>(entityArray[i], true);

                        entityManager.SetComponentEnabled<FlowFieldPathRequest>(entityArray[i], false);
                        entityManager.SetComponentEnabled<FlowFieldFollower>(entityArray[i], false);
                    }
                    entityQuery.CopyFromComponentDataArray(moveOverrideArray);
                    entityQuery.CopyFromComponentDataArray(targetOverrideArray);
                    entityQuery.CopyFromComponentDataArray(targetPositionPathQueuedArray);
                }
            }
        }

        public Rect GetSelectionAreaRect() {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            Vector2 lowerLeftCorner = new Vector2(
                Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
                Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));

            Vector2 upperRightCorner = new Vector2(
                Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
                Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));

            return new Rect(
                lowerLeftCorner.x,
                lowerLeftCorner.y,
                upperRightCorner.x - lowerLeftCorner.x,
                upperRightCorner.y - lowerLeftCorner.y
            );
        }

        private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount) {
            NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
            if (positionCount == 0) {
                return positionArray;
            }
            positionArray[0] = targetPosition;
            if (positionCount == 1) {
                return positionArray;
            }

            int ring = 0;
            int positionIndex = 1;

            while (positionIndex < positionCount) {
                int ringPositionCount = 3 + ring * 2;

                for (int i = 0; i < ringPositionCount; i++) {
                    float angle = i * (math.PI2 / ringPositionCount);
                    float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                    float3 ringPosition = targetPosition + ringVector;

                    positionArray[positionIndex] = ringPosition;
                    positionIndex++;

                    if (positionIndex >= positionCount) {
                        break;
                    }
                }
                ring++;
            }

            return positionArray;
        }

        public static void DeselectAllUnits() {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++) {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                Selected selected = selectedArray[i];
                selected.onDeselected = true;
                entityManager.SetComponentData(entityArray[i], selected);
            }
        }

    }
}