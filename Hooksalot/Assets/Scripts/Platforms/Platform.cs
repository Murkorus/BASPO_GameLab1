using UnityEngine;

public class Platform : MonoBehaviour
{
    // This script is attached to every platform.
    // Make it possible for the player to break platforms when they hit them if they have enough speed.
    // Make different kinds of platforms that can withstand more or less force before they break.
    // Also make a "checkpoint" platform that never breaks.

    public float durability; // The speed threshold at which the platform breaks, Average player speed is around 10-14.
    public Vector2 platformScale;

    public bool onlyCollideFromTop;
    [SerializeField] float respawnTime;
    [SerializeField] float slowDownMultiplier;

    [Header("References")]
    [SerializeField] BoxCollider2D platformCollider;
    [SerializeField] SpriteRenderer texture;
    [SerializeField] LineRenderer outline;
    [SerializeField] SpriteRenderer minimapMarker;

    private LayerMask excludedCollisionLayers;
    [HideInInspector] public float platformTopY;
    [HideInInspector] public bool hasBeenDestroyed;
    private float respawnTimer;

    private void Start()
    {
        UpdateScale(platformScale);
        platformTopY = transform.position.y + platformCollider.size.y * 0.5f - 0.05f; // The 0.05 is a buffer.
        excludedCollisionLayers = platformCollider.excludeLayers;
    }

    private void Update()
    {
        if (hasBeenDestroyed)
        {
            respawnTimer += Time.deltaTime;

            if(respawnTimer > respawnTime)
            {
                respawnTimer = 0;
                SwitchDestroyedState();
            }
        }

        if (onlyCollideFromTop && !hasBeenDestroyed)
        {
            if(GameManager.playerRB.transform.position.y - GameManager.playerRB.transform.localScale.y * 0.5f < platformTopY) // If the bottom of the player is above the top of the platform, disable collision with this platform.
            {
                platformCollider.excludeLayers = LayerMask.GetMask("Player");
            }
            else
            {
                platformCollider.excludeLayers = excludedCollisionLayers;
            }
        }
    }

    public void UpdateScale(Vector2 scale)
    {
        platformCollider.size *= scale;
        texture.size *= scale;
        platformScale = scale;
        outline.SetPositions(new Vector3[]
        {
            new Vector3(transform.position.x + scale.x * 0.5f, transform.position.y + scale.y * 0.5f),
            new Vector3(transform.position.x + scale.x * 0.5f, transform.position.y - scale.y * 0.5f),
            new Vector3(transform.position.x - scale.x * 0.5f, transform.position.y - scale.y * 0.5f),
            new Vector3(transform.position.x - scale.x * 0.5f, transform.position.y + scale.y * 0.5f)
        });

        minimapMarker.size *= scale;
        minimapMarker.enabled = true;
        minimapMarker.transform.position += Vector3.forward * -25;
        // The marker is a spriterenderer on the prefab. It is disabled and has a position of 0,0,0 by default to preserve the prefab preview in the project view.
        // The only purpose of enabling it here and setting its position to (0,0,-25) here is to make each platform prefab more easily distinguishable when browsing the files.
    }

    private void SwitchDestroyedState()
    {
        hasBeenDestroyed = !hasBeenDestroyed;
        texture.enabled = !texture.enabled;
        outline.enabled = !outline.enabled;
        platformCollider.enabled = !platformCollider.enabled;
        minimapMarker.enabled = !minimapMarker.enabled;
    }

    public void SimulatePlayerImpact()
    {
        SwitchDestroyedState();

        Vector2 playerVelo = GameManager.playerRB.linearVelocity;

        // Apply slowdown force to player.
        //GameManager.playerRB.AddForce(playerVelo.normalized * Mathf.Sign(playerVelo.y * -1) * Mathf.Clamp(slowDownMultiplier * (doDurabilityBasedSlowdown ? Mathf.Sqrt(durability) : 1), -slowDownLimit * playerVelo.magnitude, slowDownLimit * playerVelo.magnitude), ForceMode2D.Impulse);
        GameManager.playerRB.AddForce(Vector2.ClampMagnitude(-playerVelo * slowDownMultiplier, durability), ForceMode2D.Impulse);

        // Retract the hook if the player is hooked to this platform.
        if (GameManager.hook.grappledObject == platformCollider.gameObject && GameManager.hook.hookLaunched)
        {
            GameManager.hook.SwitchHookState();
        }
    }
}