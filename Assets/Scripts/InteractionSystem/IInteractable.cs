using UnityEngine;

namespace Interaction
{
    public interface IInteractable
    {
        /// <summary>
        /// Display name shown to the player
        /// </summary>
        string InteractionText { get; }
        
        /// <summary>
        /// Whether this object can currently be interacted with
        /// </summary>
        bool CanInteract { get; }
        
        /// <summary>
        /// Transform of the interactable object
        /// </summary>
        Transform Transform { get; }
        
        /// <summary>
        /// Called when player interacts with this object
        /// </summary>
        /// <param name="interactor">The object performing the interaction</param>
        void Interact(GameObject interactor);
        
        /// <summary>
        /// Called when player enters interaction range
        /// </summary>
        /// <param name="interactor">The object that can interact</param>
        void OnInteractionEnter(GameObject interactor);
        
        /// <summary>
        /// Called when player exits interaction range
        /// </summary>
        /// <param name="interactor">The object that was interacting</param>
        void OnInteractionExit(GameObject interactor);
    }
}
