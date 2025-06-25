using UnityEngine;

namespace Interaction
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [Header("Base Interaction Settings")]
        [SerializeField] protected string interactionText = "Interact";
        [SerializeField] protected bool canInteract = true;
        [SerializeField] protected bool highlightOnHover = true;
        [SerializeField] protected AudioClip interactionSound;
        
        [Header("Highlight Settings")]
        [SerializeField] protected Color highlightColor = Color.yellow;
        
        protected AudioSource audioSource;
        protected InteractionHighlighter highlighter;
        
        public virtual string InteractionText => interactionText;
        public virtual bool CanInteract => canInteract;
        public Transform Transform => transform;
        
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && interactionSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
            
            SetupHighlighter();
        }
        
        protected virtual void SetupHighlighter()
        {
            if (!highlightOnHover) return;
            
            highlighter = GetComponent<InteractionHighlighter>();
            if (highlighter == null && highlightOnHover)
            {
                highlighter = gameObject.AddComponent<InteractionHighlighter>();
            }
            
            if (highlighter != null)
            {
                highlighter.SetHighlightColor(highlightColor);
                highlighter.SetHighlight(false);
            }
        }
        
        public abstract void Interact(GameObject interactor);
        
        public virtual void OnInteractionEnter(GameObject interactor)
        {
            SetHighlight(true);
        }
        
        public virtual void OnInteractionExit(GameObject interactor)
        {
            SetHighlight(false);
        }
        
        protected virtual void SetHighlight(bool highlight)
        {
            if (!highlightOnHover || highlighter == null) return;
            highlighter.SetHighlight(highlight);
        }
        
        protected virtual void PlayInteractionSound()
        {
            if (audioSource != null && interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound);
            }
        }
    }
}