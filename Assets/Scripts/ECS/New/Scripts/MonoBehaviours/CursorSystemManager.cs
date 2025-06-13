using System;
using CursorSystemPRO.Scripts;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorSystemManager : MonoBehaviour {


    private bool isMouseOverButton;
    private CursorTypeSO selectedCursorTypeSO;


    private void Start() {
        CursorSystem.SetActiveCursorTypeSO(CursorTypeListSO.Instance.arrowBig);
    }

    private void BuildingPlacementManagerUI_ButtonSingle_OnAnyButtonSingleMouseOver(object sender, EventArgs e) {
        isMouseOverButton = true;
    }

    private void BuildingPlacementManagerUI_ButtonSingle_OnAnyButtonSingleMouseOut(object sender, EventArgs e) {
        isMouseOverButton = false;
    }

    private void Update() {
        CursorTypeSO lastSelectedCursorTypeSO = selectedCursorTypeSO;

        selectedCursorTypeSO = CursorTypeListSO.Instance.arrowBig;

        TestIfUnitMoveOrder();

        TestIfUnitIsUnderMouse();

        TestIfAttackingKnight();

        if (isMouseOverButton) {
            selectedCursorTypeSO = CursorTypeListSO.Instance.cursorHandUnClick;
        }

        if (selectedCursorTypeSO != lastSelectedCursorTypeSO) {
            CursorSystem.SetActiveCursorTypeSO(selectedCursorTypeSO);
        }
    }

    private void TestIfUnitMoveOrder() {
        if (HasAnyUnitSelected()) {
            selectedCursorTypeSO = CursorTypeListSO.Instance.move;
        }
    }

    private void TestIfAttackingKnight() {
        if (HasAnyUnitSelected()) {
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
                    CollidesWith = 1u << GameAssets.UNITS_LAYER,
                    GroupIndex = 0,
                }
            };
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) {
                if (entityManager.HasComponent<Faction>(raycastHit.Entity)) {
                    // Hit something with a Faction
                    Faction faction = entityManager.GetComponentData<Faction>(raycastHit.Entity);
                    if (faction.factionType == FactionType.Knight) {
                        // Right clicking on a Zombie
                        selectedCursorTypeSO = CursorTypeListSO.Instance.attack;
                    }
                }
            }
        }
    }

    private bool HasAnyUnitSelected() {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        // Is a Unit selected?
        if (entityArray.Length >= 1 && entityManager.HasComponent<Unit>(entityArray[0])) {
            // Yup unit is selected
            return true;
        }

        return false;
    }

    private void TestIfUnitIsUnderMouse() {
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
                CollidesWith = 1u << GameAssets.UNITS_LAYER,
                GroupIndex = 0,
            }
        };

        if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) {
            if (entityManager.HasComponent<Selected>(raycastHit.Entity)) {
                // Hit a Selectable entity
                selectedCursorTypeSO = CursorTypeListSO.Instance.unit;
            }
        }
    }


}