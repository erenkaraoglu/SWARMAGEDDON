using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using Unity.Cinemachine;

public class Player : MonoBehaviour
{
    [Header("References")]
    public ExampleCharacterController Character;
    public CinemachineCamera VirtualCamera;
    public CinemachineInputAxisController InputAxisController;
    
    [Header("Input Actions")]
    public InputActionAsset inputActions;
    
    [Header("Camera Settings")]
    [Range(0.1f, 100.0f)]
    public float lookSensitivity = 1.0f;
    
    // Input Action references
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction attackAction;
    private InputAction scrollAction;

    // Camera control
    private CinemachineOrbitalFollow orbitalFollow;
    private CinemachineThirdPersonFollow thirdPersonFollow;
    private CinemachineFollow cinemachineFollow;

    private void Awake()
    {
        // Get input actions from the Player action map
        var playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        lookAction = playerActionMap.FindAction("Look");
        jumpAction = playerActionMap.FindAction("Jump");
        crouchAction = playerActionMap.FindAction("Crouch");
        attackAction = playerActionMap.FindAction("Attack");
        
        // Get scroll from UI action map
        var uiActionMap = inputActions.FindActionMap("UI");
        scrollAction = uiActionMap.FindAction("ScrollWheel");

        // Get camera components
        if (VirtualCamera != null)
        {
            orbitalFollow = VirtualCamera.GetComponent<CinemachineOrbitalFollow>();
            thirdPersonFollow = VirtualCamera.GetComponent<CinemachineThirdPersonFollow>();
            cinemachineFollow = VirtualCamera.GetComponent<CinemachineFollow>();
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Set up Cinemachine camera to follow character
        if (VirtualCamera != null)
        {
            VirtualCamera.Target.TrackingTarget = Character.CameraFollowPoint;
            VirtualCamera.Target.LookAtTarget = Character.CameraFollowPoint;
        }

        // Set up input axis controller for Cinemachine
        if (InputAxisController != null && InputAxisController.Controllers != null)
        {
            ApplySensitivitySettings();
        }

        // Subscribe to input events
        attackAction.performed += OnCursorToggle;
        scrollAction.performed += OnZoom;
    }

    private void OnDestroy()
    {
        // Unsubscribe from input events
        if (attackAction != null)
        {
            attackAction.performed -= OnCursorToggle;
        }
        if (scrollAction != null)
        {
            scrollAction.performed -= OnZoom;
        }
    }

    private void Update()
    {
        HandleCharacterInput();

    }

    private void LateUpdate()
    {
        // Handle rotating the camera along with physics movers
        if (Character.Motor.AttachedRigidbody != null)
        {
            var physicsMover = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>();
            if (physicsMover != null && VirtualCamera != null)
            {
                // Cinemachine handles this automatically, but we can adjust if needed
                Vector3 rotationDelta = physicsMover.RotationDeltaFromInterpolation * Vector3.forward;
                // Apply any additional rotation adjustments if necessary
            }
        }
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        characterInputs.MoveAxisForward = moveInput.y;
        characterInputs.MoveAxisRight = moveInput.x;
        characterInputs.CameraRotation = VirtualCamera.transform.rotation;
        characterInputs.JumpDown = jumpAction.WasPressedThisFrame();
        characterInputs.CrouchDown = crouchAction.WasPressedThisFrame();
        characterInputs.CrouchUp = crouchAction.WasReleasedThisFrame();

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }

    private void OnCursorToggle(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void OnZoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<Vector2>().y;
        
        if (orbitalFollow != null)
        {
            // Adjust orbital radius (keeping it close for first person feel)
            orbitalFollow.Radius = Mathf.Clamp(
                orbitalFollow.Radius - scrollValue * 0.1f,
                0.05f, 0.5f
            );
        }
        else if (thirdPersonFollow != null)
        {
            // Adjust third person camera distance (keeping it close)
            thirdPersonFollow.CameraDistance = Mathf.Clamp(
                thirdPersonFollow.CameraDistance - scrollValue * 0.1f,
                0.05f, 0.5f
            );
        }
        else if (cinemachineFollow != null)
        {
            // Adjust forward offset for first person
            Vector3 offset = cinemachineFollow.FollowOffset;
            offset.z = Mathf.Clamp(offset.z - scrollValue * 0.05f, 0.05f, 0.3f);
            cinemachineFollow.FollowOffset = offset;
        }
    }

    private void ApplySensitivitySettings()
    {
        if (InputAxisController != null && InputAxisController.Controllers != null)
        {
            for (int i = 0; i < InputAxisController.Controllers.Count; i++)
            {
                if (InputAxisController.Controllers[i].Input != null)
                {
                    // Apply look sensitivity with Y-axis inverted
                    if (i == 0) // X-axis (horizontal)
                    {
                        InputAxisController.Controllers[i].Input.Gain = lookSensitivity;
                    }
                    else if (i == 1) // Y-axis (vertical)
                    {
                        InputAxisController.Controllers[i].Input.Gain = lookSensitivity * -1f;
                    }
                }
            }
        }
    }
}
