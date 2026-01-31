using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Locomotion Variables")]
    [SerializeField] float playerSpeed = 5.0f;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravityValue = -9.81f;

    [Header("Camera Look")]
    [SerializeField] float mouseSensitivity = 100.0f;

    [Header("Camera Bobbing")]
    [SerializeField] float bobAmount = 0.05f;
    [SerializeField] float bobSpeed = 8f;
    [SerializeField] float bobSmoothing = 8f;

    [Header("Collectibles")]
    [SerializeField] internal int MasksCollected = 0;

    Vector3 playerVelocity;
    bool groundedPlayer;
    Vector3 initialCameraLocalPos;
    float bobTimer = 0f;
    float xRotation = 0.0f;
    InputManager inputManager;
    Camera playerCamera;
    CharacterController controller;

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera != null)
            initialCameraLocalPos = playerCamera.transform.localPosition;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector3 move = MoveAndRotatePlayer();
        controller.Move(move * playerSpeed * Time.deltaTime);
        CameraBobbing(move);
        Jump();
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    Vector3 MoveAndRotatePlayer()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0f)
            playerVelocity.y = -2f; // Small negative value to keep the player grounded

        Vector2 look = inputManager.GetMouseDelta();
        float mouseX = look.x * mouseSensitivity * Time.deltaTime;
        float mouseY = look.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = camForward * movement.y + camRight * movement.x;
        return move;
    }

    void Jump()
    {
        if (inputManager.IsJumpPressed() && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }
    }

    void CameraBobbing(Vector3 move)
    {
        if (playerCamera != null)
        {
            float moveMagnitude = new Vector3(move.x, 0f, move.z).magnitude;
            if (moveMagnitude > 0.01f && groundedPlayer)
            {
                bobTimer += Time.deltaTime * bobSpeed * moveMagnitude;
                float bobOffset = Mathf.Sin(bobTimer) * bobAmount;
                Vector3 targetPos = initialCameraLocalPos + new Vector3(0f, bobOffset, 0f);
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPos, Time.deltaTime * bobSmoothing);
            }
            else
            {
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, initialCameraLocalPos, Time.deltaTime * bobSmoothing);
                if (Mathf.Abs(playerCamera.transform.localPosition.y - initialCameraLocalPos.y) < 0.001f)
                    bobTimer = 0f;
            }
        }
    }

    internal void AddMask()
    {
        MasksCollected++;
        Debug.Log("Mask collected. Total: " + MasksCollected);
    }

}
