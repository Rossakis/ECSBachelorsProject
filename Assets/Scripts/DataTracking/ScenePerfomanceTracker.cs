using System;
using System.IO;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.DataTracking
{
    /// <summary>
    /// A tracker for scene performance metrics, including FPS statistics. which then output the data to a JSON file
    /// </summary>
    public class ScenePerformanceTracker : MonoBehaviour
    {
        public bool isECSScene;
        private float lowestFPS = float.MaxValue;
        private float highestFPS = float.MinValue;
        private float totalFPS = 0f;
        private int frameCount = 0;

        private string currentSceneName;

        void Awake()
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        void Update()
        {
            float currentFPS = FPSManager.CurrentFPS; 
            if (currentFPS <= 0f)
                return; 

            lowestFPS = Mathf.Min(lowestFPS, currentFPS);
            highestFPS = Mathf.Max(highestFPS, currentFPS);
            totalFPS += currentFPS;
            frameCount++;
        }

        void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            ExportData();
        }

        void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            ExportData();
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            ExportData();
            ResetStats();
            currentSceneName = newScene.name;
        }

        private void ResetStats()
        {
            lowestFPS = float.MaxValue;
            highestFPS = float.MinValue;
            totalFPS = 0f;
            frameCount = 0;
        }

        private void ExportData()
        {
            //If Vsync is enabled, ignore export
            if (frameCount == 0 || QualitySettings.vSyncCount > 0) 
                return;

            float averageFPS = totalFPS / frameCount;

            var data = new ScenePerformanceData
            {
                SceneName = currentSceneName,
                LowestFPS = lowestFPS,
                HighestFPS = highestFPS,
                AverageFPS = averageFPS,
                Timestamp = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss")
            };

            string json = JsonUtility.ToJson(data, true);

            string folderPath = "";

            if (isECSScene)
                folderPath= Path.Combine(Application.dataPath, "../OutputData/");
            else
                folderPath = Path.Combine(Application.dataPath, "../OutputData/");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{currentSceneName}_PerformanceData.json");
            File.WriteAllText(filePath, json);

            Debug.Log($"[ScenePerformanceTracker] Exported performance data to {filePath}");
        }

        [Serializable]
        public class ScenePerformanceData
        {
            public string SceneName;
            public float LowestFPS;
            public float HighestFPS;
            public float AverageFPS;
            public string Timestamp;
        }
    }
}
