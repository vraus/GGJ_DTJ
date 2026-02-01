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

    [Header("Dash")]
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float dashStaminaCost = 25f;

    float dashCooldownTimer = 0f;
    bool isDashing = false;

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
    [SerializeField] float groundOffset = 0.08f;
    [SerializeField] LayerMask groundLayer;

    [Header("UI Elements")]
    [SerializeField] GameObject menuPauseUI;

    [Header("Stamina")]
    [SerializeField] Slider staminaSlider;
    [SerializeField] float maxStamina = 100;
    [SerializeField] float staminaDrainRate = 30f;
    [SerializeField] float staminaRegenRate = 15f;
    [SerializeField] float staminaRegenThreshold = 2f;
    Color staminaFullColor;
    Color staminaEmptyColor = Color.red;
    float staminaRegenTimer = 0f;
    float currentStamina;

    [Header("Collectibles")]
    [SerializeField] internal int MasksCollected = 0;
    [SerializeField] internal int TotalMasks = 10;
    [SerializeField] internal int MaxMasksCarriable = 3;

    [Header("Audio")]
    [SerializeField] private AudioClip[] walkFootstepClips;
    [SerializeField] private AudioClip maskCollectClip;
    [SerializeField] float walkRate = 10f;
    [SerializeField] float runRate = 20f;

    [Header("Ladder")]
    [SerializeField] float ladderClimbSpeed = 3f;
    bool isOnLadder = false;
    Vector3 UpLadder;

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
        staminaFullColor = staminaSlider.fillRect.GetComponent<Image>().color;
        controller = gameObject.GetComponent<CharacterController>();

        inputManager = InputManager.Instance;
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera != null)
            cameraStartPosition = _camera.localPosition;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentStamina = maxStamina;
        staminaSlider.value = currentStamina;


        // inputManager.PlayerControls.Locomotion.Pause.performed += ctx =>
        // {
        //     if (isGamePaused)
        //         isGamePaused = false;
        //     else
        //         isGamePaused = true;
        // 
        //     menuPauseUI.GetComponent<MenuPause>().TogglePauseMenu(isGamePaused);
        // };

        inputManager.PlayerControls.Locomotion.Pause.performed += ctx => TogglePause();
    }

    void Update()
    {
        if (controller == null || inputManager == null || isGamePaused || controller.enabled == false)
            return;

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        Vector3 move = MoveAndRotatePlayer();

        // --- LADDERS ---
        if (isOnLadder && UpLadder != null)
        {
            // On annule la gravité
            playerVelocity.y = 0f;

            // Si le joueur se deplace
            controller.Move(inputManager.GetPlayerMovement().y * UpLadder * ladderClimbSpeed * Time.deltaTime);
        }
        else
        {
            if (!isDashing)
            {
                Sprint();

                controller.Move(move * (isSprinting ? playerSprintSpeed : playerWalkSpeed) * Time.deltaTime);

                // Gravité normale
                playerVelocity.y += gravityValue * Time.deltaTime;
                controller.Move(playerVelocity * Time.deltaTime);
            }
        }
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

        if (inputManager.IsSprintPressed() && currentStamina > 0 && inputManager.GetPlayerMovement().magnitude > 0.1f)
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
            if (currentStamina < maxStamina)
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
        staminaSlider.fillRect.GetComponent<Image>().color = Color.Lerp(staminaEmptyColor, staminaFullColor, currentStamina / maxStamina);
        if (inputManager.IsDashPressed() && groundedPlayer && !isDashing && dashCooldownTimer <= 0f && currentStamina >= dashStaminaCost)
        {
            StartCoroutine(PerformDash());
        }
    }

    IEnumerator PerformDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        currentStamina -= dashStaminaCost;
        if (currentStamina < 0f) currentStamina = 0f;
        if (staminaSlider != null) staminaSlider.value = currentStamina;

        Vector3 dashDir = transform.forward;
        dashDir.y = 0f;
        dashDir.Normalize();

        float elapsed = 0f;
        float speed = dashDistance / Mathf.Max(dashDuration, 0.0001f);

        // small upward push if desired
        playerVelocity.y = DashHeight;

        while (elapsed < dashDuration)
        {
            controller.Move(dashDir * speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
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

    internal void AddMask(Transform maskTransform)
    {
        if (MasksCollected < MaxMasksCarriable)
        {
            MasksCollected++;
            Debug.Log("Mask collected. Total: " + MasksCollected);
            SoundFXManager.instance.PlayAudioClip(maskCollectClip, maskTransform, 1f);
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
        float volume = 0.7f;

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
        Vector3 startPos = cam.position;

        Quaternion targetRot = Quaternion.Euler(
            deathTiltAngleX,
            0f,
            Random.Range(-deathTiltAngleZ, deathTiltAngleZ)
        );

        RaycastHit hit;
        Vector3 targetWorldPos = startPos;

        if (Physics.Raycast(startPos, Vector3.down, out hit, 5f, groundLayer))
        {
            targetWorldPos = hit.point + Vector3.up * groundOffset;
        }
        else
        {
            targetWorldPos = startPos + Vector3.down * deathCameraDrop;
        }

        float elapsed = 0f;

        while (elapsed < deathDuration)
        {
            float t = elapsed / deathDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            cam.rotation = Quaternion.Slerp(cam.rotation, targetRot, t);
            cam.position = Vector3.Lerp(startPos, targetWorldPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            50f,
            Time.deltaTime * 3f
        );

        cam.rotation = targetRot;
        cam.position = targetWorldPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            playerVelocity.y = 0f; // reset propre
            UpLadder = other.transform.up;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        menuPauseUI.GetComponent<MenuPause>().TogglePauseMenu(isGamePaused);
    }

    private void TogglePause()
    {
        isGamePaused = !isGamePaused;
        menuPauseUI.GetComponent<MenuPause>().TogglePauseMenu(isGamePaused);
    }

}
