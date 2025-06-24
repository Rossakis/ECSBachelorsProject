using Assets.Scripts.Monobehaviour.Combat;
using Assets.Scripts.Monobehaviour.Input;
using Assets.Scripts.Monobehaviour.Movement;
using Assets.Scripts.Monobehaviour.Units;
using Assets.Scripts.ScriptableObjects.Scene;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Monobehaviour.UI
{
    public class UnitSelectionManagerMono : MonoBehaviour
    {
        public EcsSceneDataSO ecsSceneData;
        public MonoSceneDataSO monoSceneData;
        public bool isECSScene = false;

        public float ringSize = 1f;
        public LayerMask unitLayerMask;
        public static UnitSelectionManagerMono Instance { get; private set; }

        public List<UnitMono> selectedUnits = new List<UnitMono>();

        private Vector2 selectionStartMousePosition;
        private const float MultipleSelectionSizeMin = 40f;

        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;
        public event EventHandler OnSelectedEntitiesChanged; 
        
        private bool canSelect = true; // Can't select if the scene is in benchmark mode

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if ((isECSScene && ecsSceneData.IsBenchMarkMode) || (!isECSScene && monoSceneData.IsBenchMarkMode))
            {
                canSelect = false;
            }
        }

        private void Update()
        {
            if (!canSelect)
                return;

            // Only block selection start if over UI
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                selectionStartMousePosition = UnityEngine.Input.mousePosition;
                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
            }

            // Always allow selection end, even if over UI
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                Rect selectionRect = GetScreenRect(selectionStartMousePosition, UnityEngine.Input.mousePosition);

                DeselectAllUnits();

                bool isMultipleSelection = selectionRect.width + selectionRect.height > MultipleSelectionSizeMin;

                if (isMultipleSelection)
                {
                    foreach (var unit in Object.FindObjectsByType<UnitMono>(FindObjectsSortMode.None))
                    {
                        Vector2 screenPos = UnityEngine.Camera.main.WorldToScreenPoint(unit.transform.position);
                        if (selectionRect.Contains(screenPos))
                        {
                            selectedUnits.Add(unit);
                            unit.SetSelected(true);
                        }
                    }
                }
                else
                {
                    Ray ray = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000f, unitLayerMask))
                    {
                        var unit = hit.collider.GetComponentInParent<UnitMono>();
                        if (unit != null)
                        {
                            selectedUnits.Add(unit);
                            unit.SetSelected(true);
                        }
                    }
                }

                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
                OnSelectedEntitiesChanged?.Invoke(this, EventArgs.Empty);
            }

            // Move or Attack command
            if (UnityEngine.Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
            {
                Ray ray = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f, unitLayerMask))
                {
                    var targetUnit = hit.collider.GetComponentInParent<UnitMono>();
                    if (targetUnit != null && IsEnemy(targetUnit))
                    {
                        foreach (var unit in selectedUnits)
                        {
                            unit.Attack(targetUnit);
                        }
                        return;
                    }
                }

                // Move command
                Vector3 targetPos = MouseWorldPosition.Instance.GetPosition();
                var positions = GenerateMovePositions(targetPos, selectedUnits.Count, ringSize);

                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    if (selectedUnits[i].FactionType == FactionType.Wizard)
                        selectedUnits[i].SetMoveOverride(positions[i]);
                    else
                        UnitMoveManagerMono.Instance.MoveUnitToPosition(selectedUnits[i], positions[i]);
                }
            }
        }

        private bool IsEnemy(UnitMono targetUnit)
        {
            return targetUnit.FactionType != selectedUnits[0].FactionType;
        }

        private void DeselectAllUnits()
        {
            foreach (var unit in selectedUnits)
            {
                unit.SetSelected(false);
            }
            selectedUnits.Clear();
        }

        private Rect GetScreenRect(Vector2 start, Vector2 end)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            return new Rect(min, max - min);
        }

        private List<Vector3> GenerateMovePositions(Vector3 center, int count, float ringSpacing)
        {
            var positions = new List<Vector3>(count);
            if (count == 1)
            {
                positions.Add(center);
                return positions;
            }

            int ring = 0;
            int index = 0;
            positions.Add(center);
            index++;

            while (index < count)
            {
                int ringCount = 3 + ring * 2;
                for (int i = 0; i < ringCount && index < count; i++)
                {
                    float angle = i * Mathf.PI * 2f / ringCount;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * ringSpacing * (ring + 1);
                    positions.Add(center + offset);
                    index++;
                }
                ring++;
            }
            return positions;
        }

        public Rect GetSelectionAreaRect()
        {
            Vector2 selectionEndMousePosition = UnityEngine.Input.mousePosition;

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
    }
}