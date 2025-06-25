using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEntry
    {
        [SerializeField] private string soundId;
        [SerializeField] private SoundGroup soundGroup;
        
        public string SoundId => soundId;
        public SoundGroup SoundGroup => soundGroup;
    }
    
    [Header("Sound Database")]
    [SerializeField] private SoundEntry[] soundEntries;
    
    [Header("Audio Source Pool")]
    [SerializeField] private int audioSourcePoolSize = 10;
    [SerializeField] private GameObject audioSourcePrefab;
    
    private Dictionary<string, SoundGroup> soundDatabase;
    private Queue<AudioSource> audioSourcePool;
    private List<AudioSource> activeAudioSources;
    
    public static SoundManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeSoundManager()
    {
        // Initialize sound database
        soundDatabase = new Dictionary<string, SoundGroup>();
        foreach (var entry in soundEntries)
        {
            if (!string.IsNullOrEmpty(entry.SoundId) && entry.SoundGroup != null)
            {
                soundDatabase[entry.SoundId] = entry.SoundGroup;
            }
        }
        
        // Initialize audio source pool
        audioSourcePool = new Queue<AudioSource>();
        activeAudioSources = new List<AudioSource>();
        
        CreateAudioSourcePool();
    }
    
    private void CreateAudioSourcePool()
    {
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            GameObject audioObj;
            if (audioSourcePrefab != null)
            {
                audioObj = Instantiate(audioSourcePrefab, transform);
            }
            else
            {
                audioObj = new GameObject($"AudioSource_{i}");
                audioObj.transform.SetParent(transform);
                audioObj.AddComponent<AudioSource>();
            }
            
            AudioSource audioSource = audioObj.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSourcePool.Enqueue(audioSource);
        }
    }
    
    public bool PlaySound(string soundId, Vector3? position = null, int? clipIndex = null)
    {
        if (!soundDatabase.TryGetValue(soundId, out SoundGroup soundGroup))
        {
            Debug.LogWarning($"Sound ID '{soundId}' not found in sound database.");
            return false;
        }
        
        AudioClip clip = soundGroup.GetClip(clipIndex);
        if (clip == null)
        {
            Debug.LogWarning($"No audio clips found for sound ID '{soundId}' at index {clipIndex}.");
            return false;
        }
        
        AudioSource audioSource = GetAudioSource();
        if (audioSource == null)
        {
            Debug.LogWarning("No available audio sources in pool.");
            return false;
        }
        
        // Configure audio source
        audioSource.clip = clip;
        audioSource.volume = soundGroup.Volume;
        audioSource.pitch = soundGroup.GetRandomPitch();
        
        // Set position if specified
        if (position.HasValue)
        {
            audioSource.transform.position = position.Value;
            audioSource.spatialBlend = 1f; // 3D sound
        }
        else
        {
            audioSource.spatialBlend = 0f; // 2D sound
        }
        
        audioSource.Play();
        
        // Return to pool when finished
        StartCoroutine(ReturnToPoolWhenFinished(audioSource));
        
        return true;
    }
    
    private AudioSource GetAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource audioSource = audioSourcePool.Dequeue();
            activeAudioSources.Add(audioSource);
            return audioSource;
        }
        
        // If pool is empty, try to find an inactive audio source
        foreach (var audioSource in activeAudioSources)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }
        
        return null;
    }
    
    private System.Collections.IEnumerator ReturnToPoolWhenFinished(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        
        activeAudioSources.Remove(audioSource);
        audioSourcePool.Enqueue(audioSource);
    }
    
    public void StopAllSounds()
    {
        foreach (var audioSource in activeAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
    
    public bool HasSound(string soundId)
    {
        return soundDatabase.ContainsKey(soundId);
    }
}
