using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class FPSManager : MonoBehaviour
    {
        public static float CurrentFPS { get; private set; }

        public float updateInterval = 0.1f;
        public TMP_Text fpsText;
        public TMP_Text vsyncText;
        public Toggle VsyncToggle;

        private float accumulated = 0f;
        private int frames = 0;
        private float timeLeft;
        private float fps;


        void Awake()
        {
            timeLeft = updateInterval;

            if (fpsText == null)
            {
                Debug.LogError("FPSManager: TextMeshProUGUI reference not set!");
                enabled = false;
            }
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

            CurrentFPS = fps;
        }

        public void ToggleVsync()
        {
            QualitySettings.vSyncCount = VsyncToggle.isOn ? 1 : 0;
        
            if(vsyncText != null) // Text update is not necessary
                vsyncText.text = QualitySettings.vSyncCount == 1 ? "VSync: On" : "VSync: Off";
        }
    }
}
