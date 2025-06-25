using UnityEngine;

namespace Interaction
{
    public class TestInteractable : BaseInteractable
    {
        [Header("Test Settings")]
        [SerializeField] private string debugMessage = "Test interactable activated!";
        [SerializeField] private bool logInteractor = true;
        [SerializeField] private int interactionCount = 0;
        
        public override void Interact(GameObject interactor)
        {
            interactionCount++;
            
            string message = $"{debugMessage} (Count: {interactionCount})";
            
            if (logInteractor && interactor != null)
            {
                message += $" - Interacted by: {interactor.name}";
            }
            
            Debug.Log(message);
            PlayInteractionSound();
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
