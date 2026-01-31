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

    public bool IsJumpPressed()
    {
        return _playerControls.Locomotion.Jump.triggered;
    }

    public bool IsCollectPressed()
    {
        return _playerControls.Locomotion.Collect.triggered;
    }

}
