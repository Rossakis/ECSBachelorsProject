using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Camera
{
    public class CameraController : MonoBehaviour {


        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private float fieldOfViewMin;
        [SerializeField] private float fieldOfViewMax;


        private float targetFieldOfView;



        private void Awake() {
            targetFieldOfView = cinemachineCamera.Lens.FieldOfView;
        }

        private void Update() {
            Vector3 moveDir = Vector3.zero;

            if (UnityEngine.Input.GetKey(KeyCode.W)) {
                moveDir.z = +1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.S)) {
                moveDir.z = -1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.A)) {
                moveDir.x = -1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.D)) {
                moveDir.x = +1f;
            }

            Transform cameraTransform = UnityEngine.Camera.main.transform;
            moveDir = cameraTransform.forward * moveDir.z + cameraTransform.right * moveDir.x;
            moveDir.y = 0f;
            moveDir.Normalize();

            float moveSpeed = 30f;
            transform.position += moveDir * moveSpeed * Time.deltaTime;


            float rotationAmount = 0f;
            if (UnityEngine.Input.GetKey(KeyCode.Q)) {
                rotationAmount = +1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.E)) {
                rotationAmount = -1f;
            }

            float rotationSpeed = 200f;
            transform.eulerAngles += new Vector3(0, rotationAmount * rotationSpeed * Time.deltaTime, 0);


            float zoomAmount = 4f;
            if (UnityEngine.Input.mouseScrollDelta.y > 0) {
                targetFieldOfView -= zoomAmount;
            }
            if (UnityEngine.Input.mouseScrollDelta.y < 0) {
                targetFieldOfView += zoomAmount;
            }

            targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

            float zoomSpeed = 10f;
            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetFieldOfView, zoomSpeed * Time.deltaTime);
        }

    }
}
