using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    [SerializeField]
    private float normalMoveSpeed = 3.0f;
    
    [SerializeField]
    private float shiftMoveSpeed = 6.0f;
    
    void Update()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? shiftMoveSpeed : normalMoveSpeed;

        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Get camera directions
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

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
