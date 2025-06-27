using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UIWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("Window References")]
    [SerializeField] private RectTransform windowRect;
    [SerializeField] private RectTransform titleBarRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button minimizeButton;
    [SerializeField] private Button fullscreenButton;
    
    // State
    private bool isFullscreen = false;
    private Vector2 originalSize;
    private Vector2 originalPosition;
    private Vector2 dragOffset;
    private bool isDraggingTitleBar = false;
    private Canvas parentCanvas;
    private RectTransform canvasRectTransform;
    private Vector2 dragStartPosition;
    private Vector2 rectStartPosition;
    
    // Properties
    public WindowDefinition Definition { get; private set; }
    
    private void Awake()
    {
        // Find parent canvas for proper screen point calculations
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            canvasRectTransform = parentCanvas.GetComponent<RectTransform>();
        }
    }
    
    private void Start()
    {
        // Store original size for restoration from fullscreen
        originalSize = windowRect.sizeDelta;
        originalPosition = windowRect.anchoredPosition;
    }
    
    public void Initialize(WindowDefinition definition)
    {
        Definition = definition;
        
        // Set title
        if (titleText != null)
        {
            titleText.text = definition.windowTitle;
        }
        
        // Set size
        if (windowRect != null)
        {
            windowRect.sizeDelta = definition.defaultSize;
            originalSize = definition.defaultSize;
        }
        
        // Configure buttons based on window options
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(definition.canClose);
            closeButton.onClick.AddListener(Close);
        }
        
        if (minimizeButton != null)
        {
            minimizeButton.gameObject.SetActive(definition.canMinimize);
            minimizeButton.onClick.AddListener(Minimize);
        }
        
        if (fullscreenButton != null)
        {
            fullscreenButton.gameObject.SetActive(definition.canFullscreen);
            fullscreenButton.onClick.AddListener(ToggleFullscreen);
        }
    }
    
    public void Close()
    {
        WindowManager.Instance.CloseWindow(this);
    }
    
    public void Minimize()
    {
        WindowManager.Instance.MinimizeWindow(this);
    }
    
    public void ToggleFullscreen()
    {
        if (isFullscreen)
        {
            // Restore original size and position
            windowRect.sizeDelta = originalSize;
            windowRect.anchoredPosition = originalPosition;
        }
        else
        {
            // Save current size and position
            originalSize = windowRect.sizeDelta;
            originalPosition = windowRect.anchoredPosition;
            
            // Set to fullscreen (covering entire parent container)
            RectTransform parentRect = transform.parent as RectTransform;
            if (parentRect != null)
            {
                windowRect.sizeDelta = parentRect.sizeDelta;
                windowRect.anchoredPosition = Vector2.zero;
            }
        }
        
        isFullscreen = !isFullscreen;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        WindowManager.Instance.FocusWindow(this);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Only allow dragging from title bar
        isDraggingTitleBar = IsTitleBarHit(eventData);
        if (!isDraggingTitleBar) return;
        
        // Don't allow dragging in fullscreen mode
        if (isFullscreen) return;
        
        // Store the starting positions
        dragStartPosition = eventData.position;
        rectStartPosition = windowRect.anchoredPosition;
        
        // Focus the window when dragging
        WindowManager.Instance.FocusWindow(this);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Only drag if we started on the title bar
        if (!isDraggingTitleBar) return;
        
        // Don't drag if in fullscreen mode
        if (isFullscreen) return;
        
        // Calculate the delta movement in screen space
        Vector2 dragDelta = eventData.position - dragStartPosition;
        
        // Convert the screen delta to canvas delta
        Vector2 canvasDelta = dragDelta;
        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            // For camera-based canvases, we need to scale the delta
            canvasDelta = dragDelta * parentCanvas.scaleFactor;
        }
        
        // Apply the delta to the original position
        windowRect.anchoredPosition = rectStartPosition + canvasDelta;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // Store position after dragging
        if (!isFullscreen && isDraggingTitleBar)
        {
            originalPosition = windowRect.anchoredPosition;
            isDraggingTitleBar = false;
        }
    }
    
    private bool IsTitleBarHit(PointerEventData eventData)
    {
        if (titleBarRect == null) return false;
        
        // Convert screen position to local position in the title bar's coordinate system
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            titleBarRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Check if the point is inside the title bar's rect
            Rect rect = titleBarRect.rect;
            return rect.Contains(localPoint);
        }
        
        return false;
    }
}
