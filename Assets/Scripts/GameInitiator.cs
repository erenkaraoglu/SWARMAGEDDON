using UnityEngine;
using Cysharp.Threading.Tasks;
public class GameInitiator : MonoBehaviour
{

    [SerializeField] private LoadingScreen loadingScreen;

    private async void Start()
    {
        loadingScreen.ShowImmediate();
        await loadingScreen.SetProgressAsync(0.1f);
        await UniTask.Delay(1000); // Simulate loading delay
        await loadingScreen.SetProgressAsync(0.5f);
        await UniTask.Delay(1000); // Simulate loading delay
        await loadingScreen.SetProgressAsync(1f);
        await UniTask.Delay(500); // Simulate final loading delay
        await loadingScreen.HideAsync();
        
    }
    

}

