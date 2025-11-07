using UnityEngine;

public class BreakPlatform : MonoBehaviour
{
    // This script is attached to the player, and is in charge of breaking platforms when appropriate.
    // Each physics framne, this script checks if the player would collide with a platform on the next physics frame.
    // If yes, the platform is evaluated and maybe destroyed.

    private Rigidbody2D rb;
    [SerializeField] float predicitionFrames; // How many frames ahead should the player's movement be predicted to determine if they will collide with a platform?
    [SerializeField] LayerMask platformMask; // What layers should be checked for collisions with platforms?

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Check if the player will hit a platform on the next physics frame.
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, transform.localScale.x * 0.50f, rb.linearVelocity, rb.linearVelocity.magnitude * Time.fixedDeltaTime * predicitionFrames, platformMask);
        if(hit == false)
        {
            return;
        }

        Platform hitPlatform;
        if(!hit.collider.TryGetComponent(out hitPlatform))
        {
            //Debug.Log($"Hit {hit.collider.name}, but could not find a Platform component attached to it.");
            return;
        }

        if(hitPlatform.durability == Mathf.Infinity)
        {
            //Debug.Log($"Hit {hit.collider.name}, but the platform is unbreakable.");
            return;
        }

        if (hitPlatform.durability > rb.linearVelocity.magnitude)
        {
            //Debug.Log($"Hit {hit.collider.name}, but the platform's durability of {hitPlatform.durability} exceeds the player's current velocity of {rb.linearVelocity.magnitude}.");
            return;
        }

        if (hitPlatform.onlyCollideFromTop && transform.position.y - transform.localScale.y * 0.5f < hitPlatform.platformTopY)
        {
            // If the platform can be passed from below and the bottom of the player is below the top of the platform, skip this call.
            //Debug.Log($"Hit {hit.collider.name}, but the platform can be passed freely from below, and the player is currently below it.");
            return;
        }

        hitPlatform.SimulatePlayerImpact();
    }
}
