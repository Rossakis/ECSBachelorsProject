using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FPSManagerUI : MonoBehaviour
    {
        public static bool IsVsyncOn => QualitySettings.vSyncCount > 0;

        public float updateInterval = 0.1f;
        public TMP_Text fpsText;
        public TMP_Text vsyncText;
        public Toggle VsyncToggle;

        private float accumulated = 0f;
        private int frames = 0;
        private float timeLeft;
        private float fps;


        void Start()
        {
            timeLeft = updateInterval;

            if (fpsText == null)
            {
                Debug.LogError("FPSManager: TextMeshProUGUI reference not set!");
                enabled = false;
            }

            SynchronizeVsync();
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;
            accumulated += Time.timeScale / Time.deltaTime;
            ++frames;

            if (timeLeft <= 0.0f)
            {
                fps = accumulated / frames;
                fpsText.text = $"{fps:F1} f/s";
                timeLeft = updateInterval;
                accumulated = 0f;
                frames = 0;
            }
        }

        private void SynchronizeVsync()
        {
            VsyncToggle.isOn = QualitySettings.vSyncCount > 0;
        }

        public void ToggleVsync()
        {
            QualitySettings.vSyncCount = VsyncToggle.isOn ? 1 : 0;
        
            if(vsyncText != null)
                vsyncText.text = QualitySettings.vSyncCount == 1 ? "VSync: On" : "VSync: Off";
        }
    }
}
