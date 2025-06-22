using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class TargetManagerMono : MonoBehaviour
    {
        public static TargetManagerMono Instance;
        private List<UnitMono> allUnits = new List<UnitMono>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Update()
        {
            foreach (var unit in allUnits)
            {
                unit.findTargetTimer -= Time.deltaTime;
                if (unit.findTargetTimer > 0f)
                    continue;

                unit.findTargetTimer = unit.findTargetInterval;

                if (unit.overrideTarget != null)
                {
                    unit.currentTarget = unit.overrideTarget;
                    continue;
                }

                Collider[] hitColliders = Physics.OverlapSphere(
                    unit.transform.position,
                    unit.findTargetRange,
                    1 << LayerMask.NameToLayer("Units")
                );

                UnitMono closestTarget = null;
                float closestDistance = float.MaxValue;

                if (unit.currentTarget != null)
                {
                    float dist = Vector3.Distance(unit.transform.position, unit.currentTarget.transform.position);
                    closestTarget = unit.currentTarget;
                    closestDistance = dist;
                }

                foreach (var hit in hitColliders)
                {
                    UnitMono targetUnit = hit.GetComponent<UnitMono>();
                    if (targetUnit == null || targetUnit.FactionType != unit.targetFaction)
                        continue;

                    float dist = Vector3.Distance(unit.transform.position, targetUnit.transform.position);

                    if (closestTarget == null || dist + 2f < closestDistance)
                    {
                        closestTarget = targetUnit;
                        closestDistance = dist;
                    }
                }

                if (closestTarget != null)
                {
                    unit.currentTarget = closestTarget;
                }
            }

            // Lose target if too far
            foreach (var unit in allUnits)
            {
                if (unit.currentTarget != null && unit.overrideTarget == null)
                {
                    float distance = Vector3.Distance(unit.transform.position, unit.currentTarget.transform.position);
                    if (distance > unit.loseTargetDistance)
                    {
                        unit.currentTarget = null;
                    }
                }

                if (unit.overrideTarget != null)
                {
                    float distance = Vector3.Distance(unit.transform.position, unit.overrideTarget.transform.position);
                    if (distance > unit.loseTargetDistance)
                    {
                        unit.overrideTarget = null;
                    }
                }
            }
        }

        public void RegisterUnit(UnitMono unit)
        {
            if (!allUnits.Contains(unit))
                allUnits.Add(unit);
        }

        public void UnregisterUnit(UnitMono unit)
        {
            allUnits.Remove(unit);
        }
    }
}
