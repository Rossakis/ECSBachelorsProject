using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineCamera virtualCamera;

    [Header("Zoom Settings")]
    public float zoomSpeed = 1f;
    public float maxZoomIn = 0f;
    public float maxZoomOut = 100f;

    private CinemachineFollow transposer;

    void Start()
    {
        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetComponent<CinemachineFollow>();
        }
    }

    void Update()
    {
        if (transposer == null) 
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 offset = transposer.FollowOffset;
            offset.y -= scroll * zoomSpeed; 
            offset.y = Mathf.Clamp(offset.y, maxZoomIn, maxZoomOut);
            transposer.FollowOffset = offset;
        }
    }
}