using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [Header("Game Controls")]
    bool isGamePaused = false;

    [Header("Locomotion Variables")]
    [SerializeField] float playerWalkSpeed = 5.0f;
    [SerializeField] float playerSprintSpeed = 12.0f;
    [SerializeField] float DashHeight = 1.5f;
    [SerializeField] float gravityValue = -9.81f;
    bool isSprinting = false;

    [Header("Camera Look")]
    [SerializeField] float mouseSensitivity = 100.0f;

    [Header("Camera Shake")]
    [SerializeField] float duration = 0.3f;
    [SerializeField] float magnitude = 0.2f;

    [Header("Camera Bobbing")]
    [SerializeField] float bobAmplitude = 0.05f;
    [SerializeField] float bobFrequencyWalking = 8f;
    [SerializeField] float bobFrequencyRunning = 20f;
    [SerializeField] float bobSmoothing = 20f;
    [SerializeField] private Transform _camera = null;
    private Vector3 cameraStartPosition;

    [Header("Death Camera")]
    [SerializeField] float deathTiltAngleX = 60f;
    [SerializeField] float deathTiltAngleZ = 20f;
    [SerializeField] float deathDuration = 1.2f;
    [SerializeField] float deathCameraDrop = 0.3f;



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
    [SerializeField] internal int TotalMasks = 10;
    [SerializeField] internal int MaxMasksCarriable = 3;

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

        inputManager.PlayerControls.Locomotion.Pause.performed += ctx =>
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

        Vector3 move = MoveAndRotatePlayer();
        if (move.magnitude > 0.1f) Sprint();
        controller.Move(move * (isSprinting ? playerSprintSpeed : playerWalkSpeed) * Time.deltaTime);

        if (move.magnitude > 0)
        {
            HandleFootsteps();
        }

        CameraBobbing(move);
        Dash();

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
            if (!inputManager.IsSprintPressed() && currentStamina < maxStamina)
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

    void Dash()
    {
        if (inputManager.IsDashPressed() && groundedPlayer)
        {
            // Dash logic
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
        if (sin > 0.97f && canPlayFootstep && controller.isGrounded)
        {
            canPlayFootstep = false;
            PlayWalkingFootstepSound();
        }
        else if (sin < 0f && !canPlayFootstep)
        {
            canPlayFootstep = true;
        }
    }

    internal void AddMask()
    {
        Debug.Log("Mask collected. Total: " + MasksCollected);
        if (MasksCollected < MaxMasksCarriable)
        {
            MasksCollected++;
        }
    }

    public void PlayerDeath()
    {
        inputManager.DisableAllInputs();
        controller.enabled = false;

        StopAllCoroutines();
        StartCoroutine(DeathCameraCoroutine());

        Debug.Log("Player has died.");
    }

    public void PlayWalkingFootstepSound()
    {
        AudioClip footstepClip = walkFootstepClips[UnityEngine.Random.Range(0, walkFootstepClips.Length)];
        float volume = 1f;

        SoundFXManager.instance.PlayFootstep(footstepClip, transform.position, volume);
    }

    public void CameraShake(float magnitudeMultiplier = 1f)
    {
        magnitude *= magnitudeMultiplier;
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = _camera.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            _camera.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _camera.localPosition = originalPos;
    }

    private IEnumerator DeathCameraCoroutine()
    {
        Transform cam = _camera;

        Quaternion startRot = cam.localRotation;
        Vector3 startPos = cam.localPosition;

        Quaternion targetRot = Quaternion.Euler(
            deathTiltAngleX,
            0f,
            Random.Range(-deathTiltAngleZ, deathTiltAngleZ)
        );

        Vector3 targetPos = startPos + Vector3.down * deathCameraDrop;

        float elapsed = 0f;

        while (elapsed < deathDuration)
        {
            float t = elapsed / deathDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            cam.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            cam.localPosition = Vector3.Lerp(startPos, targetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localRotation = targetRot;
        cam.localPosition = targetPos;
    }


}
