using UnityEngine;
using Cysharp.Threading.Tasks;
public class GameInitiator : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject loadingScreen;
    
    [Header("Object References")]
    [SerializeField] private Canvas canvas;
    // Initialized references
    private LoadingScreen _loadingScreen;

    private async void Start()
    {
        _loadingScreen = Instantiate(loadingScreen, canvas.transform).GetComponent<LoadingScreen>();
        _loadingScreen.ShowImmediate();
        await _loadingScreen.SetProgressAsync(1f);
        await _loadingScreen.HideAsync();

    }
    

}

