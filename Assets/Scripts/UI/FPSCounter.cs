using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.5f;
    public TMP_Text fpsText;

    private float accumulated = 0f;
    private int frames = 0;
    private float timeLeft;
    private float fps;

    void Start()
    {
        timeLeft = updateInterval;

        if (fpsText == null)
        {
            Debug.LogError("FPSCounterTMP: TextMeshProUGUI reference not set!");
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
    }
}
