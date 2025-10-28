using UnityEngine;

public class Platform : MonoBehaviour
{
    // This script is attached to every platform.
    // Make it possible for the player to break platforms when they hit them if they have enough speed.
    // Make different kinds of platforms that can withstand more or less force before they break.
    // Also make a "checkpoint" platform that never breaks.

    [SerializeField] float durability; // The speed threshold at which the platform breaks, Average player speed is around 10-14.
    public Vector2 platformScale;

    public bool onlyCollideFromTop;
    [SerializeField] float respawnTime;
    [SerializeField] float slowDownMultiplier;
    [SerializeField] bool doDurabilityBasedSlowdown;
    [Range(0,1)] [SerializeField] float slowDownLimit; // Maximum amount of slow-down applied to the player when they break a platform? In % of current velocity.

    [Header("References")]
    [SerializeField] BoxCollider2D triggerCollider;
    [SerializeField] BoxCollider2D platformCollider;
    [SerializeField] SpriteRenderer texture;
    [SerializeField] LineRenderer outline;
    [SerializeField] SpriteRenderer minimapMarker;

    private LayerMask excludedCollisionLayers;
    private float platformTopY;
    [HideInInspector] public bool hasBeenDestroyed;
    private float respawnTimer;

    private void Start()
    {
        // Instead of having to change the durabilities in the script, it's better to just make the durability public, letting it be changed from the inspector.
        // Think about how much time it takes if we were to add a new type of platform. We'd have to first make a new variable for it, then extend the switch statement, then make a new tag for it and assign it.
        // Instead, if the durability variable is just public, we can simply change a number, and voila, new platform.
        // As a bonus, we also free up the tags for other potential uses in the future.
        /*switch (gameObject.tag) // Use tags to differentiate platform types, more can be added as needed
        {
            case "MarblePlatform":
                durability = marblePlatform; // Marble platforms are unbreakable but diffrent form checkpoint platforms
                break;
            case "StoneBrickPlatform":
                durability = stoneBrickPlatform;
                break;
            case "WoodPlatform":
                durability = woodPlatform;
                break;
            case "CheckpointPlatform":
                durability = Mathf.Infinity; // Can add special behavior for checkpoint platforms here if needed
                break;
            default:
                //durability = stoneBrickPlatform; // Default durability for all other platforms can be changed here
                break;
        }*/
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // We want the platform to break if the player would collide with it with enough.
        // It should then still slow down the player slightly.

        if(durability == Mathf.Infinity)
        {
            // Platform is unbreakable, skip this call.
            return;
        }

        if (onlyCollideFromTop && GameManager.playerRB.transform.position.y - GameManager.playerRB.transform.localScale.y * 0.5f < platformTopY)
        {
            // If the platform can be passed from below and the bottom of the player is below the top of the platform, skip this call.
            return;
        }

        Vector2 playerVelo = GameManager.playerRB.linearVelocity;
        if (!Physics2D.Raycast(GameManager.playerRB.transform.position, playerVelo, playerVelo.magnitude)) // Check if the player will hit the platform within the next second.
        {
            // If the player doesn't hit the platform within the next second, then just skip this call.
            return;
        }

        if (Mathf.Abs(playerVelo.y) > durability)
        {
            // Play breaking animation.
            SwitchDestroyedState();
            
            // Apply slowdown force to player.
            GameManager.playerRB.AddForce(playerVelo.normalized * Mathf.Sign(playerVelo.y * -1) * Mathf.Clamp(slowDownMultiplier * (doDurabilityBasedSlowdown ? Mathf.Sqrt(durability) : 1), -slowDownLimit * playerVelo.magnitude, slowDownLimit * playerVelo.magnitude), ForceMode2D.Impulse);
            
            // Retract the hook if the player is hooked to this platform.
            if (GameManager.hook.grappledObject == platformCollider.gameObject && GameManager.hook.hookLaunched)
            {
                GameManager.hook.SwitchHookState();
            }
        }
    }

    public void UpdateScale(Vector2 scale)
    {
        triggerCollider.size *= scale;
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
        triggerCollider.enabled = !triggerCollider.enabled;
        platformCollider.enabled = !platformCollider.enabled;
    }
}