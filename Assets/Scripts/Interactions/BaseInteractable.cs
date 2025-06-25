using UnityEngine;

namespace Interaction
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] protected string interactionText = "Interact";
        [SerializeField] protected bool canInteract = true;
        [SerializeField] protected bool highlightOnHover = true;
        [SerializeField] protected Color highlightColor = Color.yellow;
        
        [Header("Audio Settings")]
        [SerializeField] protected string interactionSoundId = "";
        [SerializeField] protected bool playSound3D = true;
        
        protected InteractionHighlighter highlighter;
        
        public virtual string InteractionText => interactionText;
        public virtual bool CanInteract => canInteract;
        public Transform Transform => transform;

        protected virtual void Awake()
        {
            SetupHighlighter();
            if(gameObject.layer != LayerMask.NameToLayer("Interactable"))
            {
                gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
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
        
        protected virtual void PlayInteractionSound(int? clipIndex = null)
        {
            if (string.IsNullOrEmpty(interactionSoundId)) return;
            
            if (SoundManager.Instance != null)
            {
                Vector3? position = playSound3D ? transform.position : null;
                SoundManager.Instance.PlaySound(interactionSoundId, position, clipIndex);
            }
            else
            {
                Debug.LogWarning("SoundManager instance not found. Make sure SoundManager is present in the scene.");
            }
        }
    }
}