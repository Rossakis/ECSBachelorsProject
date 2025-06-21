using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Camera
{
    public class CameraZoomAndTilt : MonoBehaviour
    {
        public CinemachineCamera virtualCamera;

        [Header("Zoom Settings")]
        public float zoomSpeed = 1f;
        public float maxZoomIn = 0f;
        public float maxZoomOut = 100f;

        [Header("Tilt Settings")]
        public float tiltSpeed = 3f;

        private CinemachineFollow transposer;
        private CinemachinePanTilt tilt;

        void Start()
        {
            if (virtualCamera != null)
            {
                transposer = virtualCamera.GetComponent<CinemachineFollow>();
                tilt = virtualCamera.GetComponent<CinemachinePanTilt>();
            }
        }

        void Update()
        {
            if (transposer == null || tilt == null) 
                return;

            float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Vector3 offset = transposer.FollowOffset;
                offset.y -= scroll * zoomSpeed;
                offset.y = Mathf.Clamp(offset.y, maxZoomIn, maxZoomOut);
                transposer.FollowOffset = offset;
            }

            // Tilt horizontally with middle mouse drag
            if (UnityEngine.Input.GetMouseButton(2))
            {
                float mouseDeltaX = UnityEngine.Input.GetAxis("Mouse X");
                if (Mathf.Abs(mouseDeltaX) > 0.01f)
                {
                    tilt.PanAxis.Value += mouseDeltaX * tiltSpeed;
                }
            }
        }
    }
}