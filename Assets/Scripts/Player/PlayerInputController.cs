using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SwarmCharacterController))]
public class PlayerInputController : MonoBehaviour
{
    private SwarmCharacterController _characterController;

    public PlayerInput PlayerInput { get; private set; }
    public Camera MainCamera { get; private set; }

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;

    private void Awake()
    {
        _characterController = GetComponent<SwarmCharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        MainCamera = Camera.main;

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _sprintAction = PlayerInput.actions["Sprint"];
    }

    private void Update()
    {
        HandleCharacterInput();
    }

    private void HandleCharacterInput()
    {
        // Create a new inputs struct
        var inputs = new SwarmCharacterController.PlayerInputs();

        // Get input values
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        bool jumpInput = _jumpAction.WasPressedThisFrame();
        bool sprintInput = _sprintAction.IsPressed();

        // Assign inputs
        inputs.MoveAxisForward = moveInput.y;
        inputs.MoveAxisRight = moveInput.x;
        inputs.JumpDown = jumpInput;
        inputs.SprintHeld = sprintInput;

        // It is very important to provide the camera rotation to the character controller
        if (MainCamera != null)
        {
            inputs.CameraRotation = MainCamera.transform.rotation;
        }
        else
        {
            // Fallback to world space if no camera is found
            inputs.CameraRotation = Quaternion.identity;
        }

        // Set inputs on the character controller
        _characterController.SetInputs(ref inputs);
    }
}
