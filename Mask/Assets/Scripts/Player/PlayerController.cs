using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [Header("Game Controls")]
    bool isGamePaused = false;

    [Header("Locomotion Variables")]
    [SerializeField] float playerWalkSpeed = 5.0f;
    [SerializeField] float playerSprintSpeed = 12.0f;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravityValue = -9.81f;
    bool isSprinting = false;

    [Header("Camera Look")]
    [SerializeField] float mouseSensitivity = 100.0f;

    [Header("Camera Bobbing")]
    [SerializeField] float bobAmplitude = 0.05f;
    [SerializeField] float bobFrequencyWalking = 8f;
    [SerializeField] float bobFrequencyRunning = 20f;
    [SerializeField] float bobSmoothing = 20f;
    [SerializeField] private Transform _camera = null;
    private Vector3 cameraStartPosition;


    [Header("Stamina")]
    [SerializeField] Slider staminaSlider;
    [SerializeField] float maxStamina = 100;
    [SerializeField] float staminaDrainRate = 30f;
    [SerializeField] float staminaRegenRate = 15f;
    [SerializeField] float staminaRegenThreshold = 2f;
    float staminaRegenTimer = 0f;
    float currentStamina;

    [Header("Collectibles")]
    [SerializeField] internal int MasksCollected = 0;

    [Header("Audio")]
    [SerializeField] private AudioClip[] walkFootstepClips;
    [SerializeField] float walkRate = 10f;
    [SerializeField] float runRate = 20f;
    float sin;
    bool canPlayFootstep = true;

    Vector3 playerVelocity;
    bool groundedPlayer;
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
            cameraStartPosition = _camera.localPosition;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentStamina = maxStamina;
        staminaSlider.value = currentStamina;

        inputManager.PlayerControls.Locomotion.InGameMenu.performed += ctx =>
        {
            if (isGamePaused)
            {
                isGamePaused = false;
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                isGamePaused = true;
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        };
    }

    void Update()
    {
        Sprint();

        Vector3 move = MoveAndRotatePlayer();
        controller.Move(move * (isSprinting ? playerSprintSpeed : playerWalkSpeed) * Time.deltaTime);

        if (move.magnitude > 0)
        {
            HandleFootsteps();
        }

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

    void Sprint()
    {

        if (inputManager.IsSprintPressed() && currentStamina > 0)
        {
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 0)
                currentStamina = 0;
            staminaRegenTimer = 0;
        }
        else
        {
            isSprinting = false;
            if (!inputManager.IsSprintPressed())
            {
                staminaRegenTimer += Time.deltaTime;
                if (staminaRegenTimer >= staminaRegenThreshold)
                    currentStamina += staminaRegenRate * Time.deltaTime;
            }
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
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
        if (_camera == null) return;

        float speed = new Vector3(move.x, 0f, move.z).magnitude;

        if (speed < 0.01f || !controller.isGrounded)
        {
            ResetCamera();
            return;
        }

        float frequency = isSprinting ? bobFrequencyRunning : bobFrequencyWalking;

        float bobY = Mathf.Sin(Time.time * frequency) * bobAmplitude;
        float bobX = Mathf.Cos(Time.time * frequency / 2f) * bobAmplitude * 2f;

        Vector3 targetPos = cameraStartPosition + new Vector3(bobX, bobY, 0f);
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, targetPos, Time.deltaTime * bobSmoothing);
    }

    void ResetCamera()
    {
        if (_camera.localPosition == cameraStartPosition) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, cameraStartPosition, Time.deltaTime * bobSmoothing);
    }

    void HandleFootsteps()
    {
        sin = Mathf.Sin(Time.time * (isSprinting ? runRate : walkRate));
        if (sin > 0.97f && canPlayFootstep)
        {
            canPlayFootstep = false;
            Debug.Log("Play Footstep");
            PlayWalkingFootstepSound();
        }
        else if (sin < 0f && !canPlayFootstep)
        {
            canPlayFootstep = true;
        }
    }

    internal void AddMask()
    {
        MasksCollected++;
        Debug.Log("Mask collected. Total: " + MasksCollected);
    }

    public void PlayWalkingFootstepSound()
    {
        AudioClip footstepClip = walkFootstepClips[UnityEngine.Random.Range(0, walkFootstepClips.Length)];
        float volume = 1f;

        SoundFXManager.instance.PlayFootstep(footstepClip, transform.position, volume);
    }

}
