using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Interaction;

namespace UI
{
    public class InteractionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject interactionPanel;
        [SerializeField] private TextMeshProUGUI interactionText;
        [SerializeField] private TextMeshProUGUI promptText;
        
        [Header("Settings")]
        [SerializeField] private string interactionKey = "E";
        [SerializeField] private float fadeSpeed = 5f;
        
        private CanvasGroup canvasGroup;
        private bool isVisible = false;
        
        private void Awake()
        {
            canvasGroup = interactionPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = interactionPanel.AddComponent<CanvasGroup>();
            }
            
            SetVisibility(false, true);
        }
        
        private void Update()
        {
            UpdateFade();
        }
        
        public void ShowInteraction(IInteractable interactable)
        {
            if (interactable == null) return;
            
            interactionText.text = interactable.InteractionText;
            promptText.text = $"Press [{interactionKey}]";
            SetVisibility(true);
        }
        
        public void HideInteraction()
        {
            SetVisibility(false);
        }
        
        private void SetVisibility(bool visible, bool immediate = false)
        {
            isVisible = visible;
            interactionPanel.SetActive(true);
            
            if (immediate)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                if (!visible)
                    interactionPanel.SetActive(false);
            }
        }
        
        private void UpdateFade()
        {
            float targetAlpha = isVisible ? 1f : 0f;
            float currentAlpha = canvasGroup.alpha;
            
            if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
            {
                canvasGroup.alpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
                
                if (!isVisible && canvasGroup.alpha <= 0f)
                {
                    interactionPanel.SetActive(false);
                }
            }
        }
    }
}
