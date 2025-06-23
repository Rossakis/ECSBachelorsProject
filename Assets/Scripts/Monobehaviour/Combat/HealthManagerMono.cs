using System.Collections.Generic;
using Assets.Scripts.Monobehaviour.Units;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Combat
{
    public class HealthManagerMono : MonoBehaviour
    {
        public static HealthManagerMono Instance { get; private set; }

        private readonly HashSet<UnitMono> units = new();
        private readonly List<UnitMono> unitBuffer = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            unitBuffer.Clear();
            unitBuffer.AddRange(units);

            UnityEngine.Camera mainCam = UnityEngine.Camera.main;
            if (mainCam == null) return;

            foreach (var unit in unitBuffer)
            {
                if (unit == null || unit.healthState == UnitMono.HealthState.Dead)
                    continue;

                // Assume each unit has a healthBar GameObject and a fill Transform
                if (unit.healthBar == null || unit.healthBarFill == null)
                    continue;

                Vector3 camForward = mainCam.transform.forward;
                unit.healthBar.transform.forward = camForward;

                // Health normalization
                float healthNormalized = (float)unit.currentHealth / Mathf.Max(1, unit.maxHealth);

                // Show/hide health bar based on health
                unit.healthBar.SetActive(healthNormalized < 1f && unit.currentHealth > 0);

                // Update fill (scale X)
                Vector3 fillScale = unit.healthBarFill.localScale;
                fillScale.x = Mathf.Clamp01(healthNormalized);
                unit.healthBarFill.localScale = fillScale;
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
    }
}