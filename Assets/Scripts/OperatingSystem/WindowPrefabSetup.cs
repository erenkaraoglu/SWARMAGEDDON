using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This class is just for guidance on how to set up a window prefab
[ExecuteInEditMode]
public class WindowPrefabSetup : MonoBehaviour
{
    [SerializeField] private GameObject windowPrefab;
    [SerializeField] private Sprite nineSlicedWindowSprite;
    
    public void SetupWindowPrefab()
    {
        if (windowPrefab == null)
        {
            Debug.LogError("Window prefab not assigned!");
            return;
        }
        
        // Create window hierarchy
        GameObject windowObject = new GameObject("Window", typeof(RectTransform), typeof(UIWindow));
        RectTransform windowRect = windowObject.GetComponent<RectTransform>();
        UIWindow windowComponent = windowObject.GetComponent<UIWindow>();
        
        // Set up window background using 9-slice sprite
        GameObject backgroundObject = new GameObject("Background", typeof(RectTransform), typeof(Image));
        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        Image backgroundImage = backgroundObject.GetComponent<Image>();
        backgroundObject.transform.SetParent(windowObject.transform, false);
        
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        
        if (nineSlicedWindowSprite != null)
        {
            backgroundImage.sprite = nineSlicedWindowSprite;
            backgroundImage.type = Image.Type.Sliced;
        }
        
        // Create title bar
        GameObject titleBarObject = new GameObject("TitleBar", typeof(RectTransform));
        RectTransform titleBarRect = titleBarObject.GetComponent<RectTransform>();
        titleBarObject.transform.SetParent(windowObject.transform, false);
        
        titleBarRect.anchorMin = new Vector2(0, 1);
        titleBarRect.anchorMax = Vector2.one;
        titleBarRect.offsetMin = new Vector2(0, -24);
        titleBarRect.offsetMax = Vector2.zero;
        
        // Create title text
        GameObject titleTextObject = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
        TextMeshProUGUI titleText = titleTextObject.GetComponent<TextMeshProUGUI>();
        titleTextObject.transform.SetParent(titleBarObject.transform, false);
        
        RectTransform titleTextRect = titleTextObject.GetComponent<RectTransform>();
        titleTextRect.anchorMin = new Vector2(0, 0);
        titleTextRect.anchorMax = new Vector2(1, 1);
        titleTextRect.offsetMin = new Vector2(10, 0);
        titleTextRect.offsetMax = new Vector2(-80, 0);
        
        titleText.text = "Window Title";
        titleText.alignment = TextAlignmentOptions.MidlineLeft;
        titleText.fontSize = 14;
        
        // Create buttons
        GameObject buttonsContainer = new GameObject("Buttons", typeof(RectTransform));
        buttonsContainer.transform.SetParent(titleBarObject.transform, false);
        
        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(1, 0);
        buttonsRect.anchorMax = Vector2.one;
        buttonsRect.offsetMin = new Vector2(-80, 0);
        buttonsRect.offsetMax = Vector2.zero;
        
        // Add buttons (close, minimize, fullscreen)
        // For brevity, I'm not detailing the button creation here
        
        // Create content area
        GameObject contentObject = new GameObject("Content", typeof(RectTransform));
        contentObject.transform.SetParent(windowObject.transform, false);
        
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(5, 5);
        contentRect.offsetMax = new Vector2(-5, -29);
        
        Debug.Log("Window prefab setup complete! Customize further in the editor.");
    }
}
