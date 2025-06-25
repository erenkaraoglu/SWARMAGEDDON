using UnityEngine;

namespace Interaction
{
    public class TestInteractable : BaseInteractable
    {
        [Header("Test Settings")]
        [SerializeField] private string debugMessage = "Test interactable activated!";
        [SerializeField] private bool logInteractor = true;
        [SerializeField] private int interactionCount = 0;
        
        [Header("Test Audio")]
        [SerializeField] private string alternativeSoundId = "";
        [SerializeField] private bool useAlternativeSound = false;
        
        public override void Interact(GameObject interactor)
        {
            interactionCount++;
            
            string message = $"{debugMessage} (Count: {interactionCount})";
            
            if (logInteractor && interactor != null)
            {
                message += $" - Interacted by: {interactor.name}";
            }
            
            Debug.Log(message);
            
            // Play sound based on settings
            if (useAlternativeSound && !string.IsNullOrEmpty(alternativeSoundId))
            {
                PlayAlternativeSound();
            }
            else
            {
                PlayInteractionSound();
            }
        }
        
        private void PlayAlternativeSound()
        {
            if (SoundManager.Instance != null)
            {
                Vector3? position = playSound3D ? transform.position : null;
                // Use index 0 for alternative sound or random if no index specified
                SoundManager.Instance.PlaySound(alternativeSoundId, position, 0);
            }
        }
        
        public override void OnInteractionEnter(GameObject interactor)
        {
            base.OnInteractionEnter(interactor);
            Debug.Log($"Player entered interaction range of {gameObject.name}");
        }
        
        public override void OnInteractionExit(GameObject interactor)
        {
            base.OnInteractionExit(interactor);
            Debug.Log($"Player exited interaction range of {gameObject.name}");
        }
    }
}
