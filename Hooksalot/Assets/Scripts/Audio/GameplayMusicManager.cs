using UnityEngine;
using FMODUnity;

public class GameplayMusicManager : MonoBehaviour
{
    public EventReference gameplayMusicEvent;

    private FMOD.Studio.EventInstance gameplayMusicInstance;

    public Transform player;
    public float maxHeight = 1000f; // adjust based on your level size

    void Start()
    {
        gameplayMusicInstance = RuntimeManager.CreateInstance(gameplayMusicEvent);
        gameplayMusicInstance.start();
        gameplayMusicInstance.release();
    }

    void Update()
    {
        float wrappedHeight = player.position.y % maxHeight;

        if (wrappedHeight < 0f)
            wrappedHeight += maxHeight; // ensures value is positive if player falls below 0

        float normalizedY = wrappedHeight / maxHeight;

        gameplayMusicInstance.setParameterByName("VerticalProgress", normalizedY);
    }

    void OnDestroy()
    {
        gameplayMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
