using UnityEngine;

namespace Assets.Scripts.Monobehaviour.Input
{
    public class MouseWorldPosition : MonoBehaviour {


        public static MouseWorldPosition Instance { get; private set; }


        private void Awake() {
            Instance = this;
        }

        public Vector3 GetPosition() {
            Ray mouseCameraRay = UnityEngine.Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(mouseCameraRay, out float distance)) {
                return mouseCameraRay.GetPoint(distance);
            } else {
                return Vector3.zero;
            }
        }


    }
}