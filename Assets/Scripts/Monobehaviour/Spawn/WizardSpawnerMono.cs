using System.Collections.Generic;
using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Spawn
{
    public class WizardSpawnerMono : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public MonoSceneDataSO sceneData;
        public GameObject wizardPrefab;
        public float spawnRadius = 10f;
        public float minDistanceBetweenUnits = 2f;

        [Header("Spawn Control")]
        public bool spawnOnStart = true;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnWizards();
            }
        }

        public void SpawnWizards()
        {
            if (wizardPrefab == null)
            {
                UnityEngine.Debug.LogError("WizardSpawnerMono: Wizard prefab not assigned.");
                return;
            }

            int maxToSpawn = sceneData.WizardsAmountToSpawn;
            Vector3 origin = transform.position;
            List<Vector3> spawnedPositions = new List<Vector3>();
            int attempts = 0;
            int spawnedCount = 0;

            while (spawnedCount < maxToSpawn && attempts < 1000)
            {
                Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
                Vector3 candidatePos = origin + new Vector3(offset2D.x, 0f, offset2D.y);

                bool isFarEnough = true;
                foreach (Vector3 existing in spawnedPositions)
                {
                    if (Vector3.Distance(candidatePos, existing) < minDistanceBetweenUnits)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                {
                    Instantiate(wizardPrefab, candidatePos, Quaternion.identity, transform);
                    spawnedPositions.Add(candidatePos);
                    spawnedCount++;
                }

                attempts++;
            }

            if (spawnedCount < maxToSpawn)
            {
                UnityEngine.Debug.LogWarning($"WizardSpawnerMono: Only spawned {spawnedCount} of {maxToSpawn} wizards after {attempts} attempts.");
            }
        }
    }
}