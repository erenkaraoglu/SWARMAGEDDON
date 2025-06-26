using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using Unity.Cinemachine;
using Interaction;
using UI;

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
    
    [Header("Interaction")]
    [SerializeField] private InteractionDetector interactionDetector;
    [SerializeField] private InteractionUI interactionUI;
    
    [Header("Camera FOV Settings")]
    public float DefaultFOV = 60f;
    public float SprintFOV = 70f;
    public float FOVChangeSpeed = 5f;
    
    // Input Action references
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction attackAction;
    private InputAction scrollAction;
    private InputAction interactAction;
    private InputAction sprintAction; // New sprint action
    private InputAction escapeAction; // New escape action
    
    // Camera control
    private CinemachineOrbitalFollow orbitalFollow;
    private CinemachineThirdPersonFollow thirdPersonFollow;
    private CinemachineFollow cinemachineFollow;

    // Computer mode variables
    private bool isInComputerMode = false;
    private ComputerInteractable currentComputer;

    private void Awake()
    {
        // Get input actions from the Player action map
        var playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        lookAction = playerActionMap.FindAction("Look");
        jumpAction = playerActionMap.FindAction("Jump");
        crouchAction = playerActionMap.FindAction("Crouch");
        attackAction = playerActionMap.FindAction("Attack");
        interactAction = playerActionMap.FindAction("Interact");
        sprintAction = playerActionMap.FindAction("Sprint");
        escapeAction = playerActionMap.FindAction("Escape"); // Get escape action
        
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
        
        // Setup interaction detector if not assigned
        if (interactionDetector == null)
        {
            interactionDetector = GetComponent<InteractionDetector>();
            if (interactionDetector == null)
            {
                interactionDetector = gameObject.AddComponent<InteractionDetector>();
            }
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
        interactAction.performed += OnInteract;
        escapeAction.performed += OnEscape; // Subscribe to escape
        
        // Subscribe to interaction events
        if (interactionDetector != null)
        {
            interactionDetector.OnInteractableChanged += OnInteractableChanged;
        }
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
        if (interactAction != null)
        {
            interactAction.performed -= OnInteract;
        }
        if (escapeAction != null)
        {
            escapeAction.performed -= OnEscape;
        }
        
        // Unsubscribe from interaction events
        if (interactionDetector != null)
        {
            interactionDetector.OnInteractableChanged -= OnInteractableChanged;
        }
    }

    private void Update()
    {
        HandleCharacterInput();
        UpdateCameraFOV();
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
        // Don't handle movement input if in computer mode
        if (isInComputerMode)
        {
            // Create empty inputs to stop movement
            PlayerCharacterInputs emptyInputs = new PlayerCharacterInputs();
            Character.SetInputs(ref emptyInputs);
            return;
        }

        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        characterInputs.MoveAxisForward = moveInput.y;
        characterInputs.MoveAxisRight = moveInput.x;
        characterInputs.CameraRotation = VirtualCamera.transform.rotation;
        characterInputs.JumpDown = jumpAction.WasPressedThisFrame();
        characterInputs.CrouchDown = crouchAction.WasPressedThisFrame();
        characterInputs.CrouchUp = crouchAction.WasReleasedThisFrame();
        characterInputs.SprintDown = sprintAction.IsPressed(); // Pass sprint input

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
    
    // New method to handle camera FOV changes when sprinting
    private void UpdateCameraFOV()
    {
        if (VirtualCamera != null)
        {
            // Determine target FOV based on sprint state
            float targetFOV = Character.IsSprinting() ? SprintFOV : DefaultFOV;
            
            // Smoothly interpolate current FOV to target FOV
            VirtualCamera.Lens.FieldOfView = Mathf.Lerp(
                VirtualCamera.Lens.FieldOfView, 
                targetFOV, 
                FOVChangeSpeed * Time.deltaTime
            );
        }
    }

    private void OnCursorToggle(InputAction.CallbackContext context)
    {
        // Toggle if not in computer mode
        if (!isInComputerMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (interactionDetector != null)
        {
            interactionDetector.TryInteract();
        }
    }
    
    private void OnInteractableChanged(IInteractable interactable)
    {
        if (interactionUI != null)
        {
            if (interactable != null)
            {
                interactionUI.ShowInteraction(interactable);
            }
            else
            {
                interactionUI.HideInteraction();
            }
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

    private void OnEscape(InputAction.CallbackContext context)
    {
        if (isInComputerMode && currentComputer != null)
        {
            currentComputer.ForceStandUp();
        }
    }
    
    // New methods for computer mode
    public void SetComputerMode(bool enabled, ComputerInteractable computer)
    {
        isInComputerMode = enabled;
        currentComputer = enabled ? computer : null;
        
        // Control cursor visibility and lock state
        if (enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        // Disable/enable interaction detector
        if (interactionDetector != null)
        {
            interactionDetector.enabled = !enabled;
        }
        
        // Hide interaction UI when entering computer mode
        if (enabled && interactionUI != null)
        {
            interactionUI.HideInteraction();
        }
        
        Debug.Log($"Computer mode {(enabled ? "enabled" : "disabled")}");
    }
    
    public bool IsInComputerMode()
    {
        return isInComputerMode;
    }
}
