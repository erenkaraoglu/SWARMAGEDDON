using UnityEngine;
using UnityEngine.InputSystem;
using Interaction;

[RequireComponent(typeof(SwarmCharacterController))]
[RequireComponent(typeof(InteractionDetector))]
public class PlayerInputController : MonoBehaviour
{
    private SwarmCharacterController _characterController;
    private InteractionDetector _interactionDetector;

    public PlayerInput PlayerInput { get; private set; }
    public Camera MainCamera { get; private set; }

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;
    private InputAction _interactAction;
    private InputAction _escapeAction;

    private void Awake()
    {
        _characterController = GetComponent<SwarmCharacterController>();
        _interactionDetector = GetComponent<InteractionDetector>();
        PlayerInput = GetComponent<PlayerInput>();
        MainCamera = Camera.main;

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _crouchAction = PlayerInput.actions["Crouch"];
        _sprintAction = PlayerInput.actions["Sprint"];
        _interactAction = PlayerInput.actions["Interact"];
        _escapeAction = PlayerInput.actions["Escape"];
    }

    private void Update()
    {
        HandleCharacterInput();
        HandleInteractionInput();
        HandleEscapeInput();
    }

    private void HandleEscapeInput()
    {
        if (_escapeAction.WasPressedThisFrame())
        {
            _characterController.ExitComputerMode();
        }
    }

    private void HandleInteractionInput()
    {
        if (_interactAction.WasPressedThisFrame())
        {
            _interactionDetector.TryInteract();
        }
    }

    private void HandleCharacterInput()
    {
        // Create a new inputs struct
        var inputs = new PlayerCharacterInputs();

        // Get input values
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        bool jumpInput = _jumpAction.WasPressedThisFrame();
        bool crouchInputDown = _crouchAction.WasPressedThisFrame();
        bool crouchInputUp = _crouchAction.WasReleasedThisFrame();
        bool sprintInput = _sprintAction.IsPressed();

        // Assign inputs
        inputs.MoveAxisForward = moveInput.y;
        inputs.MoveAxisRight = moveInput.x;
        inputs.JumpDown = jumpInput;
        inputs.CrouchDown = crouchInputDown;
        inputs.CrouchUp = crouchInputUp;
        inputs.Sprint = sprintInput;

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
