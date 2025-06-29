using UnityEngine;
using KinematicCharacterController;
using System.Collections.Generic;
using Unity.Cinemachine;


public class SwarmCharacterController : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor Motor;

    [Header("Movement")]
    public float MaxMoveSpeed = 10f;
    public float MaxSprintSpeed = 15f;
    public float StableMovementSharpness = 15f;
    public float OrientationSharpness = 10f;

    [Header("Jumping")]
    public float JumpSpeed = 10f;

    [Header("Misc")]
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;

    [Header("Camera")]
    public CinemachineCamera CinemachineCamera;
    public float NormalFov = 60f;
    public float SprintFov = 70f;
    public float FovChangeSpeed = 5f;

    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    private bool _jumpRequested = false;
    private bool _sprintHeld = false;

    private void Awake()
    {
        Motor.CharacterController = this;
        if (CinemachineCamera == null)
        {
            CinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        }
    }

    public struct PlayerInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool SprintHeld;
    }

    public void SetInputs(ref PlayerInputs inputs)
    {
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

        _moveInputVector = cameraPlanarRotation * moveInputVector;
        _lookInputVector = cameraPlanarDirection;

        if (inputs.JumpDown)
        {
            _jumpRequested = true;
        }

        _sprintHeld = inputs.SprintHeld;
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
        {
            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
        }

        Vector3 currentUp = (currentRotation * Vector3.up);
        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-OrientationSharpness * deltaTime));
        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (Motor.GroundingStatus.IsStableOnGround)
        {
            // Ground movement
            float currentVelocityMagnitude = currentVelocity.magnitude;
            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
            
            float currentMaxSpeed = _sprintHeld ? MaxSprintSpeed : MaxMoveSpeed;
            Vector3 targetMovementVelocity = reorientedInput * currentMaxSpeed;

            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));

            // Jumping
            if (_jumpRequested)
            {
                Motor.ForceUnground();
                currentVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                _jumpRequested = false;
            }
        }
        else
        {
            // Air movement
            if (_moveInputVector.sqrMagnitude > 0f)
            {
                float currentMaxSpeed = _sprintHeld ? MaxSprintSpeed : MaxMoveSpeed;
                Vector3 addedVelocity = _moveInputVector * currentMaxSpeed * deltaTime;
                currentVelocity += addedVelocity;
            }

            currentVelocity += Gravity * deltaTime;
        }
    }

    public void BeforeCharacterUpdate(float deltaTime) { }

    public void AfterCharacterUpdate(float deltaTime) 
    {
        if (CinemachineCamera != null)
        {
            float targetFov = _sprintHeld && _moveInputVector.sqrMagnitude > 0f ? SprintFov : NormalFov;
            CinemachineCamera.Lens.FieldOfView = Mathf.Lerp(CinemachineCamera.Lens.FieldOfView, targetFov, FovChangeSpeed * deltaTime);
        }
    }

    public void PostGroundingUpdate(float deltaTime) { }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void AddVelocity(Vector3 velocity) { }
}
