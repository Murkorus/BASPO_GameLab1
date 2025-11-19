using UnityEngine;
using FMODUnity;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene.");
        }
        instance = this;
    }
    
    public void PlayOneShot(sound, worldPos) 
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }