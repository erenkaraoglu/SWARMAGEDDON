using UnityEngine;

public enum SoundSelectionMode
{
    Random,
    Index
}

[CreateAssetMenu(fileName = "SoundGroup", menuName = "◆ Sound/Sound Group")]
public class SoundGroup : ScriptableObject
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip[] audioClips;
    
    [Header("Selection Mode")]
    [SerializeField] private SoundSelectionMode selectionMode = SoundSelectionMode.Random;
    
    [Header("Randomization")]
    [SerializeField] private bool randomizePitch = false;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;
    
    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    
    public AudioClip[] AudioClips => audioClips;
    public SoundSelectionMode SelectionMode => selectionMode;
    public bool RandomizePitch => randomizePitch;
    public float MinPitch => minPitch;
    public float MaxPitch => maxPitch;
    public float Volume => volume;
    
    public AudioClip GetRandomClip()
    {
        if (audioClips == null || audioClips.Length == 0) return null;
        
        if (audioClips.Length == 1) return audioClips[0];
        
        int randomIndex = Random.Range(0, audioClips.Length);
        return audioClips[randomIndex];
    }
    
    public AudioClip GetClipByIndex(int index)
    {
        if (audioClips == null || audioClips.Length == 0) return null;
        
        if (index < 0 || index >= audioClips.Length)
        {
            Debug.LogWarning($"Sound index {index} is out of range. Array length: {audioClips.Length}");
            return null;
        }
        
        return audioClips[index];
    }
    
    public AudioClip GetClip(int? index = null)
    {
        if (selectionMode == SoundSelectionMode.Index && index.HasValue)
        {
            return GetClipByIndex(index.Value);
        }
        
        return GetRandomClip();
    }
    
    public float GetRandomPitch()
    {
        return randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;
    }
    
    public int GetClipCount()
    {
        return audioClips?.Length ?? 0;
    }
}
