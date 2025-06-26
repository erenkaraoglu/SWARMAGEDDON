using UnityEngine;
using Cysharp.Threading.Tasks;
public class GameInitiator : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject soundManager;
    
    [Header("Object References")]
    [SerializeField] private Canvas canvas;
    // Initialized references
    private LoadingScreen _loadingScreen;

    private async void Start()
    {
        _loadingScreen = Instantiate(loadingScreen, canvas.transform).GetComponent<LoadingScreen>();
        _loadingScreen.ShowImmediate();

        // Instantiate the sound manager (singleton)
        Instantiate(soundManager, canvas.transform);
        
        await _loadingScreen.SetProgressAsync(1f);
        await _loadingScreen.HideAsync();

    }
    

}

