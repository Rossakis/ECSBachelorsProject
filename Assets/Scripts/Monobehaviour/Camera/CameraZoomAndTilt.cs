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

        // Cirlce
        public float radius = 20f;     // Radius of the circular path
        public float angularSpeed = 30f; // Degrees per second

        private float angle; // Current angle in degrees
        public GameObject centerPoint; // Center point of the circular path

        void Start()
        {
            if (virtualCamera != null)
            {
                transposer = virtualCamera.GetComponent<CinemachineFollow>();
                tilt = virtualCamera.GetComponent<CinemachinePanTilt>();
            }
        }

        public void TiltCamera()
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

        public void RotateCameraInfinite()
        {
            angle += angularSpeed * Time.deltaTime;
            float radians = angle * Mathf.Deg2Rad;

            // Calculate new position around the center point
            Vector3 newPosition = centerPoint.transform.position + new Vector3(
                Mathf.Cos(radians) * radius,
                0f,
                Mathf.Sin(radians) * radius
            );

            transform.position = newPosition;

            if(angle >= 360f)
            {
                angle -= 360f; // Reset angle to prevent overflow
            }
        }
    }
}