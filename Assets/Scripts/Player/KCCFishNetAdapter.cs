using UnityEngine;
using FishNet.Object;
using KinematicCharacterController;
using Unity.Cinemachine;

[RequireComponent(typeof(PlayerInputController), typeof(SwarmCharacterController), typeof(KinematicCharacterMotor))]
public class KCCFishNetAdapter : NetworkBehaviour
{
    private PlayerInputController _playerInputController;
    private SwarmCharacterController _swarmCharacterController;
    private KinematicCharacterMotor _kinematicCharacterMotor;
    private CinemachineCamera _cinemachineCamera;
    private GameObject _playerMesh;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _swarmCharacterController = GetComponent<SwarmCharacterController>();
        _kinematicCharacterMotor = GetComponent<KinematicCharacterMotor>();
        _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();

        // Loop on every child to find the player mesh
        foreach (Transform child in transform.GetChild(0)) // Getchild(0) = PlayerRoot
        {
            if (child.name == "PlayerMesh")
            {
                _playerMesh = child.gameObject;
                break;
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            // This is the local player.
            // All KCC components should be enabled and active.
            _playerInputController.enabled = true;
            _swarmCharacterController.enabled = true;
            _kinematicCharacterMotor.enabled = true;
            _cinemachineCamera.enabled = true;

        }
        else
        {
            // This is a remote player.
            // Disable all KCC components to save performance.
            // The NetworkTransform will handle syncing the position and rotation.
            _playerInputController.enabled = false;
            _swarmCharacterController.enabled = false;
            _kinematicCharacterMotor.enabled = false;
            _cinemachineCamera.enabled = false;
            
            // Change the local player mesh layer to "Default"
            if (_playerMesh != null)
            {
                foreach (Transform child in _playerMesh.transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
            else
            {
                Debug.LogWarning("PlayerMesh not found in KCCFishNetAdapter.");
            }
        }
    }
}
