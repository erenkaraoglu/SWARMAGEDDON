using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(BaseInteractable))]
    public class InteractionHighlighter : MonoBehaviour
    {
        [Header("Outline Settings")]
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float outlineWidth = 5f;
        [SerializeField] private Outline.Mode outlineMode = Outline.Mode.OutlineAll;
        
        private Outline[] outlineComponents;
        private bool isHighlighted = false;
        
        private void Awake()
        {
            SetupOutlineComponents();
        }
        
        private void SetupOutlineComponents()
        {
            // Get all renderers in this object and its children
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            outlineComponents = new Outline[renderers.Length];
            
            for (int i = 0; i < renderers.Length; i++)
            {
                // Check if the renderer already has an Outline component
                Outline outline = renderers[i].gameObject.GetComponent<Outline>();
                
                // If not, add one
                if (outline == null)
                {
                    outline = renderers[i].gameObject.AddComponent<Outline>();
                    outline.OutlineMode = outlineMode;
                    outline.OutlineColor = highlightColor;
                    outline.OutlineWidth = 0; // Start with invisible outline
                }
                
                outlineComponents[i] = outline;
            }
            
            // Initially disable all outlines
            SetHighlight(false);
        }
        
        public void SetHighlight(bool highlighted)
        {
            isHighlighted = highlighted;
            
            if (outlineComponents == null) return;
            
            foreach (var outline in outlineComponents)
            {
                if (outline != null)
                {
                    outline.OutlineColor = highlightColor;
                    outline.OutlineWidth = highlighted ? outlineWidth : 0;
                }
            }
        }
        
        public void SetHighlightColor(Color color)
        {
            highlightColor = color;
            
            if (outlineComponents == null || !isHighlighted) return;
            
            foreach (var outline in outlineComponents)
            {
                if (outline != null)
                {
                    outline.OutlineColor = color;
                }
            }
        }
        
        public void SetOutlineWidth(float width)
        {
            outlineWidth = width;
            
            if (outlineComponents == null || !isHighlighted) return;
            
            foreach (var outline in outlineComponents)
            {
                if (outline != null)
                {
                    outline.OutlineWidth = width;
                }
            }
        }
    }
}
