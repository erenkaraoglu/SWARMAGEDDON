using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance { get; private set; }
    
    [SerializeField] private Transform windowsContainer;
    [SerializeField] private List<WindowDefinition> availableWindows = new List<WindowDefinition>();
    
    private Dictionary<string, List<UIWindow>> activeWindows = new Dictionary<string, List<UIWindow>>();
    private List<UIWindow> minimizedWindows = new List<UIWindow>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (windowsContainer == null)
        {
            windowsContainer = transform;
        }

        OpenWindow("w_welcome"); // Open a default window on startup
        OpenWindow("w_welcome"); // Open a default window on startup
        OpenWindow("w_welcome"); // Open a default window on startup
        
    }
    
    public UIWindow OpenWindow(string windowId)
    {
        // Find window definition
        WindowDefinition definition = availableWindows.Find(w => w.windowId == windowId);
        if (definition == null)
        {
            Debug.LogError($"Window definition not found: {windowId}");
            return null;
        }
        
        // Check if window with oneInstance flag is already open
        if (definition.oneInstance && activeWindows.ContainsKey(windowId) && activeWindows[windowId].Count > 0)
        {
            UIWindow existingWindow = activeWindows[windowId][0];
            // If minimized, restore it
            if (minimizedWindows.Contains(existingWindow))
            {
                RestoreWindow(existingWindow);
            }
            FocusWindow(existingWindow);
            return existingWindow;
        }
        
        // Create new window
        GameObject windowObject = Instantiate(definition.windowPrefab, windowsContainer);
        UIWindow windowComponent = windowObject.GetComponent<UIWindow>();
        
        if (windowComponent == null)
        {
            Debug.LogError($"Window prefab does not have UIWindow component: {windowId}");
            Destroy(windowObject);
            return null;
        }
        
        // Initialize window
        windowComponent.Initialize(definition);
        
        // Add to active windows
        if (!activeWindows.ContainsKey(windowId))
        {
            activeWindows[windowId] = new List<UIWindow>();
        }
        activeWindows[windowId].Add(windowComponent);
        
        FocusWindow(windowComponent);
        return windowComponent;
    }
    
    public void CloseWindow(UIWindow window)
    {
        if (window == null) return;
        
        string windowId = window.Definition.windowId;
        
        // Remove from active windows
        if (activeWindows.ContainsKey(windowId))
        {
            activeWindows[windowId].Remove(window);
        }
        
        // Remove from minimized windows if needed
        minimizedWindows.Remove(window);
        
        Destroy(window.gameObject);
    }
    
    public void MinimizeWindow(UIWindow window)
    {
        if (window == null || minimizedWindows.Contains(window)) return;
        
        window.gameObject.SetActive(false);
        minimizedWindows.Add(window);
    }
    
    public void RestoreWindow(UIWindow window)
    {
        if (window == null || !minimizedWindows.Contains(window)) return;
        
        window.gameObject.SetActive(true);
        minimizedWindows.Remove(window);
        FocusWindow(window);
    }
    
    public void FocusWindow(UIWindow window)
    {
        if (window == null) return;
        
        // Bring to front by setting as last sibling
        window.transform.SetAsLastSibling();
    }
    
    public bool IsWindowMinimized(UIWindow window)
    {
        return minimizedWindows.Contains(window);
    }
}
