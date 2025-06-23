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

        private UnityEngine.Camera mainCam;

        private void Awake()
        {
            Instance = this;
            mainCam = UnityEngine.Camera.main;
        }

        private void Update()
        {
            unitBuffer.Clear();
            unitBuffer.AddRange(units);

            foreach (var unit in unitBuffer)
            {
                if (unit == null || unit.healthState == UnitMono.HealthState.Dead)
                    continue;

                if (unit.healthBar == null || unit.healthBarFill == null)
                    continue;

                // Make health bar face the camera
                unit.healthBar.transform.forward = mainCam.transform.forward;

                float healthNormalized = (float)unit.currentHealth / Mathf.Max(1, unit.maxHealth);
                healthNormalized = Mathf.Clamp01(healthNormalized);

                Vector3 fillScale = unit.healthBarFill.localScale;
                fillScale.x = healthNormalized; 
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