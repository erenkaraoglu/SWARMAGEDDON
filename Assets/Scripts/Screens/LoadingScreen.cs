using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image fillImage;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float progressDuration = 0.3f;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        // Initialize with hidden state
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }

    public async UniTask ShowAsync()
    {
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        await canvasGroup.DOFade(1f, fadeDuration).ToUniTask();
    }

    public void ShowImmediate()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public async UniTask HideAsync()
    {
        await canvasGroup.DOFade(0f, fadeDuration).ToUniTask();
        
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    public async UniTask SetProgressAsync(float progress)
    {
        progress = Mathf.Clamp01(progress);
        
        if (fillImage != null)
        {
            await fillImage.DOFillAmount(progress, progressDuration).ToUniTask();
        }
    }
}
