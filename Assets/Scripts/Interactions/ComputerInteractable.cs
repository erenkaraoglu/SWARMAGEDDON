using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;

namespace Interaction
{
    public class ComputerInteractable : BaseInteractable
    {
        [Header("Computer Settings")]
        [SerializeField] private string sitText = "Use Computer";
        [SerializeField] private string standText = "Stand Up";
        [SerializeField] private bool isInUse = false;
        
        [Header("Chair Settings")]
        [SerializeField] private Transform chairPosition;
        [SerializeField] private float sitAnimationDuration = 1f;
        [SerializeField] private Ease sitAnimationEase = Ease.OutSine;
        
        [Header("Camera Settings")]
        [SerializeField] private CinemachineCamera computerCamera;
        [SerializeField] private int computerCameraPriority = 20;
        [SerializeField] private int defaultCameraPriority = 10;
        
        [Header("Computer Audio")]
        [SerializeField] private int sitSoundIndex = 0;
        [SerializeField] private int standSoundIndex = 1;
        
        private bool isAnimating = false;
        private GameObject currentUser;
        private Vector3 originalPlayerPosition;
        private Quaternion originalPlayerRotation;
        private Tween sitTween;
        private SwarmCharacterController playerController;
        
        public override string InteractionText 
        {
            get
            {
                return isInUse ? standText : sitText;
            }
        }
        
        public override bool CanInteract => canInteract && !isAnimating;
        public bool IsInUse => isInUse;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (chairPosition == null)
            {
                Debug.LogWarning($"Chair position not assigned for {gameObject.name}");
            }
            
            if (computerCamera == null)
            {
                Debug.LogWarning($"Computer camera not assigned for {gameObject.name}");
            }
        }
        
        public override void Interact(GameObject interactor)
        {
            if (isAnimating) return;
            
            if (isInUse)
            {
                StandUp();
            }
            else
            {
                SitDown(interactor);
            }
        }
        
        private void SitDown(GameObject interactor)
        {
            if (chairPosition == null || computerCamera == null) return;
            
            currentUser = interactor;
            playerController = interactor.GetComponent<SwarmCharacterController>();
            
            if (playerController == null)
            {
                Debug.LogWarning("SwarmCharacterController not found on interactor. Current:" + interactor.name);
                return;
            }
            
            // Store original position and rotation
            originalPlayerPosition = interactor.transform.position;
            originalPlayerRotation = interactor.transform.rotation;
            
            isInUse = true;
            isAnimating = true;
            
            // Enable computer mode on player
            playerController.SetComputerMode(true, this);
            
            // Switch camera priority
            if (computerCamera != null)
            {
                computerCamera.Priority = computerCameraPriority;
            }
            
            // Animate player to chair position
            AnimatePlayerToChair(interactor);
            
            // Play sit sound
            PlayInteractionSound(sitSoundIndex);
            
            Debug.Log($"Player sat down at {gameObject.name}");
        }
        
        private void StandUp()
        {
            if (currentUser == null) return;
            
            isInUse = false;
            isAnimating = true;
            
            // Disable computer mode on player
            if (playerController != null)
            {
                playerController.SetComputerMode(false, null);
            }
            
            // Reset camera priority
            if (computerCamera != null)
            {
                computerCamera.Priority = defaultCameraPriority;
            }
            
            // Animate player back to original position
            AnimatePlayerFromChair();
            
            // Play stand sound
            PlayInteractionSound(standSoundIndex);
            
            Debug.Log($"Player stood up from {gameObject.name}");
        }
        
        private void AnimatePlayerToChair(GameObject player)
        {
            sitTween?.Kill();
            
            Vector3 targetPosition = chairPosition.position;
            Quaternion targetRotation = chairPosition.rotation;
            
            // Move player to chair position
            sitTween = DOTween.Sequence()
                .Append(player.transform.DOMove(targetPosition, sitAnimationDuration).SetEase(sitAnimationEase))
                .Join(player.transform.DORotateQuaternion(targetRotation, sitAnimationDuration).SetEase(sitAnimationEase))
                .OnComplete(() => {
                    isAnimating = false;
                    sitTween = null;
                });
        }
        
        private void AnimatePlayerFromChair()
        {
            if (currentUser == null) return;
            
            sitTween?.Kill();
            
            // Move player back to original position
            sitTween = DOTween.Sequence()
                .Append(currentUser.transform.DOMove(originalPlayerPosition, sitAnimationDuration).SetEase(sitAnimationEase))
                .Join(currentUser.transform.DORotateQuaternion(originalPlayerRotation, sitAnimationDuration).SetEase(sitAnimationEase))
                .OnComplete(() => {
                    isAnimating = false;
                    sitTween = null;
                    currentUser = null;
                    playerController = null;
                });
        }
        
        public void ForceStandUp()
        {
            if (isInUse)
            {
                StandUp();
            }
        }
        
        public override void OnInteractionEnter(GameObject interactor)
        {
            base.OnInteractionEnter(interactor);
        }
        
        public override void OnInteractionExit(GameObject interactor)
        {
            // Don't hide highlight if computer is in use and this is the current user
            if (isInUse && interactor == currentUser)
            {
                return;
            }
            
            base.OnInteractionExit(interactor);
        }
        
        private void OnValidate()
        {
            sitSoundIndex = Mathf.Max(0, sitSoundIndex);
            standSoundIndex = Mathf.Max(0, standSoundIndex);
            sitAnimationDuration = Mathf.Max(0.1f, sitAnimationDuration);
            computerCameraPriority = Mathf.Max(0, computerCameraPriority);
            defaultCameraPriority = Mathf.Max(0, defaultCameraPriority);
        }
        
        private void OnDestroy()
        {
            sitTween?.Kill();
        }
    }
}
