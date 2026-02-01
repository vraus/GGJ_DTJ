using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private PlayerInputActions _playerControls;

    public PlayerInputActions PlayerControls => _playerControls;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        _playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return _playerControls.Locomotion.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return _playerControls.Locomotion.Look.ReadValue<Vector2>();
    }

    public bool IsDashPressed()
    {
        return _playerControls.Locomotion.Dash.triggered;
    }

    public bool IsCollectPressed()
    {
        return _playerControls.Locomotion.Collect.triggered;
    }

    public bool IsSprintPressed()
    {
        return _playerControls.Locomotion.Sprint.ReadValue<float>() > 0.5f;
    }

    public bool IsDroppedPressed()
    {
        return _playerControls.Locomotion.Drop.triggered;
    }

    public void DisableAllInputs()
    {
        _playerControls.Disable();
    }

    public void EnableAllInputs()
    {
        _playerControls.Enable();
    }
}
