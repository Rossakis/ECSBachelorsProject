using Assets.Scripts.ScriptableObjects.Scene;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Camera
{
    public class CameraTargetController : MonoBehaviour
    {
        public EcsSceneDataSO ecsSceneData;
        public MonoSceneDataSO monoSceneData;
        public bool isECSScene = false;

        [SerializeField]
        private float normalMoveSpeed = 3.0f;
    
        [SerializeField]
        private float shiftMoveSpeed = 6.0f;

        private bool isControllable = true;

        private CameraZoomAndTilt cameraZoomAndTilt;

        private void Start()
        {
            if ((isECSScene && ecsSceneData.IsBenchMarkMode) || (!isECSScene && monoSceneData.IsBenchMarkMode))
            {
                isControllable = false;
            }

            cameraZoomAndTilt = GetComponent<CameraZoomAndTilt>();
        }

        void Update()
        {
            if (!isControllable)
            {
                cameraZoomAndTilt.RotateCameraInfinite();
                return;
            }

            cameraZoomAndTilt.TiltCamera();
            float moveSpeed = UnityEngine.Input.GetKey(KeyCode.LeftShift) ? shiftMoveSpeed : normalMoveSpeed;

            // Get input
            float horizontal = UnityEngine.Input.GetAxis("Horizontal");
            float vertical = UnityEngine.Input.GetAxis("Vertical");

            // Get camera directions
            Vector3 camForward = UnityEngine.Camera.main.transform.forward;
            Vector3 camRight = UnityEngine.Camera.main.transform.right;

            // Flatten vectors to XZ plane
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Combine input with camera direction
            Vector3 moveDirection = camRight * horizontal + camForward * vertical;

            // Move the target
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}
