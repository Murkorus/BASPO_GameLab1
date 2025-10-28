using UnityEngine;

public class Minimap : MonoBehaviour
{
    // Only one of this script exists in the scene. It is attached to a GameObject named Minimap.
    // This script handles any necessary functions relating to the minimap.

    [SerializeField] SpriteRenderer background;
    [SerializeField] SpriteRenderer lavaMarker;
    [SerializeField] SpriteRenderer playerMarker;

    private void Start()
    {
        // The background covers up the scene view, making it difficult if not impossible to work.
        // Therefore it is disabled at all times, so this will simply enable it when the game starts.
        // Same story for the lava minimap marker and the player marker.
        background.enabled = true;
        lavaMarker.enabled = true;
        playerMarker.enabled = true;
    }
}
