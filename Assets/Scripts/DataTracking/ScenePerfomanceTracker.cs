using Assets.Scripts.UI;
using System;
using System.IO;
using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Assets.Scripts.DataTracking
{
    /// <summary>
    /// A tracker for scene performance metrics, including FPS statistics and device specs, which then outputs the data to a JSON file
    /// </summary>
    public class ScenePerformanceTracker : MonoBehaviour
    {
        [Header("Loading params")]
        public EcsSceneDataSO ecsSceneDataSO; 
        public MonoSceneDataSO monoSceneDataSO;
        public bool isECSScene;
        public float sceneReadyDelay = 4.0f;
        private bool isSceneReady = false; // Flag to check if the scene is ready for performance tracking (e.g., after Start() has been called)

        private float lowestFPS = float.MaxValue;
        private float highestFPS = float.MinValue;
        private float totalFPS = 0f;
        private int frameCount = 1;

        private string currentSceneName;

        // Benchmark Mode
        private bool isBenchMarkMode;
        private static float minFPS = 25f; // Minimum FPS threshold for performance tracking
        private int wizardAmount;
        private int knightAmount;
        private int fpsAtTimeOfLeave;
        private UnitCountManager unitCountManager;

        private float lowFpsDuration = 0f; 

        private void Awake()
        {
            wizardAmount = 0;
            knightAmount = 0;
            currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void Start()
        {
            unitCountManager = UnitCountManager.Instance;
            isSceneReady = false;
            isBenchMarkMode = isECSScene ? ecsSceneDataSO.IsBenchMarkMode : monoSceneDataSO.IsBenchMarkMode;

            StartCoroutine(SetSceneReadyAfterDelay());
        }

        private System.Collections.IEnumerator SetSceneReadyAfterDelay()
        {
            yield return new WaitForSeconds(sceneReadyDelay);
            isSceneReady = true;
        }

        private void Update()
        {
            // Calculate FPS for this frame
            float currentFPS = 1.0f / Time.unscaledDeltaTime;
            if (currentFPS <= 0f || float.IsInfinity(currentFPS))
                return;

            lowestFPS = Mathf.Min(lowestFPS, currentFPS);
            highestFPS = Mathf.Max(highestFPS, currentFPS);
            totalFPS += currentFPS;
            frameCount++;

            // Only trigger scene change if FPS is below threshold for more than 1 second
            if (isSceneReady)
            {
                if (currentFPS < minFPS)
                {
                    lowFpsDuration += Time.unscaledDeltaTime;
                    if (lowFpsDuration > 1f)
                    {
                        fpsAtTimeOfLeave = Mathf.RoundToInt(currentFPS);
                        SceneManager.LoadScene(0);
                        Debug.LogWarning($"FPS dropped below minimum threshold for over 1 second: {currentFPS} FPS in scene {currentSceneName}");
                    }
                }
                else
                {
                    lowFpsDuration = 0f;
                }
            }

            if (unitCountManager.IsECSScene)
            {
                wizardAmount = unitCountManager.ECSWizardsCount;
                knightAmount = unitCountManager.ECSKnightsCount;
            }
            else
            {
                wizardAmount = unitCountManager.MonoWizardsCount;
                knightAmount = unitCountManager.MonoKnightsCount;
            }
            
        }

        private void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            ExportData();
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            ExportData();
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            ExportData();
            ResetStats();
            currentSceneName = newScene.name;
            isSceneReady = false; // Reset on scene change
        }

        private void ResetStats()
        {
            lowestFPS = float.MaxValue;
            highestFPS = float.MinValue;
            totalFPS = 0f;
            frameCount = 0;
            lowFpsDuration = 0f;
        }

        private void ExportData()
        {
            //If Vsync is enabled, ignore export
            if (QualitySettings.vSyncCount > 0)
            {
                Debug.LogWarning("[ScenePerformanceTracker] Vsync is enabled. Skipping export.");
                return;
            }

            float averageFPS = totalFPS / frameCount;


            var data = new ScenePerformanceData
            {
                SceneName = currentSceneName,
                LowestFPS = lowestFPS,
                HighestFPS = highestFPS,
                AverageFPS = averageFPS,

                KnightUnitsAmount = knightAmount,
                WizardUnitsAmount = wizardAmount,
                FPSAtTimeOfLeave = fpsAtTimeOfLeave,

                DeviceModel = SystemInfo.deviceModel,
                DeviceType = SystemInfo.deviceType.ToString(),
                OperatingSystem = SystemInfo.operatingSystem,
                ProcessorType = SystemInfo.processorType,
                ProcessorCount = SystemInfo.processorCount,
                SystemMemoryMB = SystemInfo.systemMemorySize,
                GraphicsDeviceName = SystemInfo.graphicsDeviceName,
                GraphicsDeviceType = SystemInfo.graphicsDeviceType.ToString(),
                GraphicsMemoryMB = SystemInfo.graphicsMemorySize,
                Timestamp = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss"),
            };

            string json = JsonUtility.ToJson(data, true);

            string folderPath;

            if(!isBenchMarkMode)
                folderPath = Path.Combine(Application.dataPath, "../OutputData/FreeCameraMode/");
            else
                folderPath = Path.Combine(Application.dataPath, "../OutputData/BenchmarkMode/");


            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{currentSceneName}_PerformanceData.json");
            File.WriteAllText(filePath, json);

            Debug.Log($"[ScenePerformanceTracker] Exported performance data to {filePath}");
        }

        [Serializable]
        public class ScenePerformanceData
        {
            // General Scene Data
            public string SceneName;
            public float LowestFPS;
            public float HighestFPS;
            public float AverageFPS;

            // Bench Mark Mode specific
            public int WizardUnitsAmount;
            public int KnightUnitsAmount;
            public int FPSAtTimeOfLeave; // what was the FPS when the player left the scene

            // Device specs
            public string DeviceModel;
            public string DeviceType;
            public string OperatingSystem;
            public string ProcessorType;
            public int ProcessorCount;
            public int SystemMemoryMB;
            public string GraphicsDeviceName;
            public string GraphicsDeviceType;
            public int GraphicsMemoryMB;
            public string Timestamp;
        }
    }
}
