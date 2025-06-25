using UnityEngine;
using DG.Tweening;

namespace Interaction
{
    public enum DoorType
    {
        Rotational,
        Sliding
    }
    
    public class DoorInteractable : BaseInteractable
    {
        [Header("Door Settings")]
        [SerializeField] private bool isOpen = false;
        [SerializeField] private string openText = "Open Door";
        [SerializeField] private string closeText = "Close Door";
        [SerializeField] private bool canToggle = true;
        [SerializeField] private bool isLocked = false;
        [SerializeField] private string lockedText = "Locked";
        
        [SerializeField] private DoorType doorType = DoorType.Rotational;
        
        [SerializeField] private Transform doorTransform;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Ease animationEase = Ease.OutSine;
        [SerializeField] private Collider doorCollider; // Added collider reference
        
        [Header("Rotational Settings")]
        [SerializeField] private float openAngle = 80f;
        
        [Header("Sliding Settings")]
        [SerializeField] private Vector3 openOffset = new Vector3(0, 0, 2f);
        
        [Header("Door Audio Indices")]
        [SerializeField] private int openSoundIndex = 0;
        [SerializeField] private int closeSoundIndex = 1;
        
        private bool isAnimating = false;
        private Tween doorTween;
        private Vector3 closedRotation;
        private Vector3 closedPosition;
        private GameObject currentInteractor;
        
        public override string InteractionText 
        {
            get
            {
                if (isLocked) return lockedText;
                return isOpen ? closeText : openText;
            }
        }
        
        public override bool CanInteract => canInteract && !isAnimating && !isLocked;
        public bool IsOpen => isOpen;
        public bool IsLocked => isLocked;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (doorTransform == null)
                doorTransform = transform;
                
            // Auto-find collider if not assigned
            if (doorCollider == null)
                doorCollider = doorTransform.GetComponent<Collider>();
                
            // Store the initial closed state
            closedRotation = doorTransform.eulerAngles;
            closedPosition = doorTransform.position;
            
            // Set initial door state
            if (isOpen)
            {
                SetInitialOpenState();
            }
        }
        
        private void SetInitialOpenState()
        {
            if (doorType == DoorType.Rotational)
            {
                Vector3 openRotation = closedRotation + new Vector3(0, openAngle, 0);
                doorTransform.rotation = Quaternion.Euler(openRotation);
            }
            else // Sliding
            {
                Vector3 openPosition = closedPosition + openOffset;
                doorTransform.position = openPosition;
            }
        }
        
        public override void Interact(GameObject interactor)
        {
            if (isAnimating || isLocked) return;
            
            currentInteractor = interactor;
            
            // Toggle door state
            if (canToggle || !isOpen)
            {
                ToggleDoor();
            }
        }
        
        private void ToggleDoor()
        {
            isOpen = !isOpen;
            
            // Play appropriate sound
            int soundIndex = isOpen ? openSoundIndex : closeSoundIndex;
            PlayInteractionSound(soundIndex);
            
            // Animate door based on type
            if (doorType == DoorType.Rotational)
            {
                AnimateRotationalDoor();
            }
            else
            {
                AnimateSlidingDoor();
            }
            
            Debug.Log($"Door {(isOpen ? "opened" : "closed")}");
        }
        
        private void AnimateRotationalDoor()
        {
            Vector3 targetRotation;
            if (isOpen)
            {
                targetRotation = CalculateOpenDirection();
            }
            else
            {
                targetRotation = closedRotation;
            }
            
            AnimateDoorRotation(targetRotation);
        }
        
        private void AnimateSlidingDoor()
        {
            Vector3 targetPosition;
            if (isOpen)
            {
                targetPosition = closedPosition + openOffset;
            }
            else
            {
                targetPosition = closedPosition;
            }
            
            AnimateDoorPosition(targetPosition);
        }
        
        private Vector3 CalculateOpenDirection()
        {
            if (currentInteractor == null)
            {
                return closedRotation + new Vector3(0, openAngle, 0);
            }
            
            Vector3 pivotPosition = doorTransform.position;
            Vector3 playerPosition = currentInteractor.transform.position;
            Vector3 directionToPlayer = (playerPosition - pivotPosition).normalized;
            Vector3 doorForward = doorTransform.forward;
            
            float dotProduct = Vector3.Dot(doorForward, directionToPlayer);
            float rotationDirection = dotProduct > 0 ? 1f : -1f;
            
            return closedRotation + new Vector3(0, openAngle * rotationDirection, 0);
        }
        
        private void AnimateDoorRotation(Vector3 targetEulerAngles)
        {
            doorTween?.Kill();
            isAnimating = true;
            SetHighlight(true);
            
            // Disable collider during animation
            if (doorCollider != null)
                doorCollider.enabled = false;
                
            doorTween = doorTransform.DORotate(targetEulerAngles, animationDuration)
                .SetEase(animationEase)
                .OnComplete(OnAnimationComplete);
        }
        
        private void AnimateDoorPosition(Vector3 targetPosition)
        {
            doorTween?.Kill();
            isAnimating = true;
            SetHighlight(true);
            
            // Disable collider during animation
            if (doorCollider != null)
                doorCollider.enabled = false;
                
            doorTween = doorTransform.DOMove(targetPosition, animationDuration)
                .SetEase(animationEase)
                .OnComplete(OnAnimationComplete);
        }
        
        private void OnAnimationComplete()
        {
            isAnimating = false;
            doorTween = null;
            
            // Re-enable collider after animation
            if (doorCollider != null)
                doorCollider.enabled = true;
                
            if (currentInteractor == null)
            {
                SetHighlight(false);
            }
        }
        
        public void SetDoorState(bool open, bool playSound = false, bool animate = true, GameObject interactor = null)
        {
            if (isOpen == open) return;
            
            currentInteractor = interactor;
            isOpen = open;
            
            if (animate && Application.isPlaying)
            {
                if (doorType == DoorType.Rotational)
                {
                    AnimateRotationalDoor();
                }
                else
                {
                    AnimateSlidingDoor();
                }
            }
            else
            {
                doorTween?.Kill();
                SetDoorStateImmediate();
                isAnimating = false;
            }
            
            if (playSound)
            {
                int soundIndex = isOpen ? openSoundIndex : closeSoundIndex;
                PlayInteractionSound(soundIndex);
            }
        }
        
        private void SetDoorStateImmediate()
        {
            if (doorType == DoorType.Rotational)
            {
                Vector3 targetRotation = isOpen ? CalculateOpenDirection() : closedRotation;
                doorTransform.rotation = Quaternion.Euler(targetRotation);
            }
            else
            {
                Vector3 targetPosition = isOpen ? closedPosition + openOffset : closedPosition;
                doorTransform.position = targetPosition;
            }
        }
        
        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }
        
        public override void OnInteractionEnter(GameObject interactor)
        {
            base.OnInteractionEnter(interactor);
            currentInteractor = interactor;
            
            // Update interaction text based on current state
            string currentText = isOpen ? closeText : openText;
            if (isLocked)
            {
                currentText = lockedText;
            }
        }
        
        public override void OnInteractionExit(GameObject interactor)
        {
            // Don't hide highlight if door is animating
            if (!isAnimating)
            {
                base.OnInteractionExit(interactor);
            }
            
            if (currentInteractor == interactor)
            {
                currentInteractor = null;
            }
        }
        
        protected override void SetHighlight(bool highlight)
        {
            // Always allow highlighting during animation
            if (isAnimating && highlight)
            {
                if (highlighter != null)
                {
                    highlighter.SetHighlight(true);
                }
                return;
            }
            
            // Normal highlight behavior when not animating
            base.SetHighlight(highlight);
        }
        
        private void OnValidate()
        {
            openSoundIndex = Mathf.Max(0, openSoundIndex);
            closeSoundIndex = Mathf.Max(0, closeSoundIndex);
            animationDuration = Mathf.Max(0.1f, animationDuration);
        }
        
        private void OnDestroy()
        {
            // Clean up tween on destroy
            doorTween?.Kill();
        }
    }
}