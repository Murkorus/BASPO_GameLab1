using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Grappling Hook SFX")]
    [SerializeField] private EventReference hookLaunchSFX;
    [SerializeField] private EventReference hookAttachSFX;
    [SerializeField] private EventReference reelLoopSFX;

    private EventInstance reelInstance;
    private bool reelIsActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }

    public void PlayHookLaunch(Vector3 position)
    {
        RuntimeManager.PlayOneShot(hookLaunchSFX, position);
    }

    public void PlayHookAttach(Vector3 position)
    {
        RuntimeManager.PlayOneShot(hookAttachSFX, position);
    }

    public void StartReelLoop()
    {
        if (reelIsActive) return;

        reelInstance = RuntimeManager.CreateInstance(reelLoopSFX);
        reelInstance.start();
        reelIsActive = true;
    }

    public void StopReelLoop()
    {
        if (!reelIsActive) return;

        reelInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        reelInstance.release();
        reelIsActive = false;
    }

    public void SetReelTension(float tension)
    {
        if (reelIsActive)
        {
            reelInstance.setParameterByName("Tension", tension);
        }
    }
}
