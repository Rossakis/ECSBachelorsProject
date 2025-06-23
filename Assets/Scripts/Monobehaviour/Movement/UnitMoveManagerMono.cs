using System.Collections.Generic;
using Assets.Scripts.Monobehaviour.Combat;
using Assets.Scripts.Monobehaviour.Navigation;
using Assets.Scripts.Monobehaviour.Units;
using Assets.Scripts.ScriptableObjects.Animation;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Movement
{
    public class UnitMoveManagerMono : MonoBehaviour
    { 
        public static UnitMoveManagerMono Instance { get; private set; }

        private readonly HashSet<UnitMono> units = new();
        private readonly List<UnitMono> unitBuffer = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            //Because the unit hashset is changed in runtime, we need to copy it to a list for iteration
            unitBuffer.Clear();
            unitBuffer.AddRange(units);

            //We use a pre-allocated list as a "scratch buffer" to avoid allocations in Update
            foreach (var unit in unitBuffer)
            {
                if (unit == null) 
                    continue; 

                if (unit.IsFollowingFlowField)
                {
                    Vector2Int gridPos = FlowFieldManagerMono.Instance.grid.GetGridPosition(unit.transform.position);
                    Vector2 dir = FlowFieldManagerMono.Instance.grid.flowField[gridPos.x, gridPos.y];
                    if (dir == Vector2.zero)
                    {
                        unit.StopFollowingFlowField();
                        continue;
                    }

                    int nextX = Mathf.Clamp(gridPos.x + Mathf.RoundToInt(dir.x), 0, FlowFieldManagerMono.Instance.grid.width - 1);
                    int nextY = Mathf.Clamp(gridPos.y + Mathf.RoundToInt(dir.y), 0, FlowFieldManagerMono.Instance.grid.height - 1);

                    Vector3 moveTarget = FlowFieldManagerMono.Instance.grid.GetWorldCenterPosition(nextX, nextY);
                    unit.SetTargetPosition(moveTarget);

                    if (Vector3.Distance(unit.transform.position, unit.FlowFieldTarget) < FlowFieldManagerMono.Instance.grid.nodeSize)
                    {
                        unit.StopFollowingFlowField();
                    }

                    if (unit.FactionType == FactionType.Wizard)
                        unit.Move(AnimationDataSO.AnimationType.WizardWalk, AnimationDataSO.AnimationType.WizardIdle);
                    else
                        unit.Move(AnimationDataSO.AnimationType.KnightWalk, AnimationDataSO.AnimationType.KnightIdle);

                }
                else
                {
                    // Try direct movement first
                    if (unit.FactionType == FactionType.Wizard)
                        unit.Move(AnimationDataSO.AnimationType.WizardWalk, AnimationDataSO.AnimationType.WizardIdle);
                    else
                        unit.Move(AnimationDataSO.AnimationType.KnightWalk, AnimationDataSO.AnimationType.KnightIdle);

                    // If direct path is blocked, start following flow field
                    if (unit.NeedsFlowField() && unit.currentTarget != null)
                    {
                        Vector2Int targetGrid = FlowFieldManagerMono.Instance.grid.GetGridPosition(unit.currentTarget.transform.position);
                        FlowFieldManagerMono.Instance.grid.CalculateFlowField(targetGrid);
                        unit.StartFollowingFlowField(unit.currentTarget.transform.position);
                    }
                }
            }
        }

        public void RegisterUnit(UnitMono unit)
        {
            units.Add(unit);
        }

        public void UnregisterUnit(UnitMono unit)
        {
            units.Remove(unit);
        }

        public void MoveUnitToPosition(UnitMono unit, Vector3 position)
        {
            RegisterUnit(unit);
            unit.SetTargetPosition(position);
        }
    }
}

