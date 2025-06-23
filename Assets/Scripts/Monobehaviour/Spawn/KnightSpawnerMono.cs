using System.Collections.Generic;
using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Spawn
{
    public class KnightSpawnerMono : MonoBehaviour
    {
        public MonoSceneDataSO sceneData;

        [Header("Spawn Settings")]
        public GameObject knightPrefab;
        public float minDistanceBetweenUnits = 1.5f;
        public float spawnRadiusMultiplier = 10f;
        public float minSpawnRadius = 1f;

        [Header("Infinite Spawn Settings")]
        public float timerMax = 0.05f;

        private float timer;
        private readonly HashSet<GameObject> spawnedKnights = new HashSet<GameObject>();

        private void Start()
        {
            timer = timerMax;

            if (!sceneData.IsKnightSpawnInfinite)
            {
                OneShotSpawn();
            }
        }

        private void Update()
        {
            if (!sceneData.IsKnightSpawnInfinite)
                return;

            timer -= Time.deltaTime;
            if (timer > 0f)
                return;

            timer = timerMax;
            Vector3 origin = transform.position;

            // Try to find a good spawn position
            float radius = Mathf.Max(spawnRadiusMultiplier * sceneData.KnightsAmountToSpawn / 100f, minSpawnRadius);
            Vector3 spawnPosition = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            while (attempts < 100)
            {
                Vector2 offset2D = Random.insideUnitCircle * radius;
                Vector3 candidate = origin + new Vector3(offset2D.x, 0f, offset2D.y);

                bool isFarEnough = true;
                foreach (var knight in spawnedKnights)
                {
                    if (knight == null) continue;
                    if (Vector3.Distance(candidate, knight.transform.position) < minDistanceBetweenUnits)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                {
                    spawnPosition = candidate;
                    positionFound = true;
                    break;
                }

                attempts++;
            }

            if (positionFound)
            {
                var knightObj = Instantiate(knightPrefab, spawnPosition, Quaternion.identity, transform);
                spawnedKnights.Add(knightObj);
            }
        }

        private void OneShotSpawn()
        {
            Vector3 origin = transform.position;
            List<Vector3> spawnPositions = new List<Vector3>();
            int attempts = 0;
            int spawnedCount = 0;

            while (spawnedCount < sceneData.KnightsAmountToSpawn && attempts < 1000)
            {
                float radius = Mathf.Max(spawnRadiusMultiplier * sceneData.KnightsAmountToSpawn / 100f, minSpawnRadius);
                Vector2 offset2D = Random.insideUnitCircle * radius;
                Vector3 candidate = origin + new Vector3(offset2D.x, 0f, offset2D.y);

                bool isFarEnough = true;
                foreach (Vector3 pos in spawnPositions)
                {
                    if (Vector3.Distance(candidate, pos) < minDistanceBetweenUnits)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough)
                {
                    var knightObj = Instantiate(knightPrefab, candidate, Quaternion.identity, transform);
                    spawnedKnights.Add(knightObj);
                    spawnPositions.Add(candidate);
                    spawnedCount++;
                }

                attempts++;
            }

            if (spawnedCount < sceneData.KnightsAmountToSpawn)
            {
                UnityEngine.Debug.LogWarning($"KnightSpawnerMono: Only spawned {spawnedCount} of {sceneData.KnightsAmountToSpawn} knights after {attempts} attempts.");
            }
        }
    }
}

