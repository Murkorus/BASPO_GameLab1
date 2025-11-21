using UnityEngine;
using FMODUnity;

public class MenuMusicManager : MonoBehaviour
{
    public EventReference menuMusicEvent;

    private FMOD.Studio.EventInstance menuMusicInstance;

    void Start()
    {
        menuMusicInstance = RuntimeManager.CreateInstance(menuMusicEvent);
        menuMusicInstance.start();
        menuMusicInstance.release(); // Safe cleanup when event stops
    }

    void OnDestroy()
    {
        menuMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
