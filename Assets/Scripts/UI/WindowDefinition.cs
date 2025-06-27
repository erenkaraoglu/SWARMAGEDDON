using UnityEngine;

[CreateAssetMenu(fileName = "New Window", menuName = "UI/Window Definition")]
public class WindowDefinition : ScriptableObject
{
    [Header("Window Properties")]
    public string windowId;
    public string windowTitle;
    public GameObject windowPrefab;
    
    [Header("Window Options")]
    public bool oneInstance = true;
    public bool canClose = true;
    public bool canMinimize = true;
    public bool canFullscreen = true;
    public Vector2 defaultSize = new Vector2(400, 300);
}
