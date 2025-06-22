using System.Collections.Generic;
using Assets.Scripts.Monobehaviour.Combat;
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
                if (unit.FactionType == FactionType.Wizard)
                    unit.Move(AnimationDataSO.AnimationType.WizardWalk, AnimationDataSO.AnimationType.WizardIdle);
                else
                    unit.Move(AnimationDataSO.AnimationType.KnightWalk, AnimationDataSO.AnimationType.KnightIdle);
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

