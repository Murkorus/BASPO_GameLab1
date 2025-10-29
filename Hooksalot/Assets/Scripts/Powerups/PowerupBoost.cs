using UnityEngine;

public class VerticalLaunchPowerup : MonoBehaviour
{
    
    public float launchForce = 15f;   // Strength of upward boost

    void OnTriggerEnter2D(Collider2D other)
    {
        // Use layers instead. Using layers avoids collision in the first place, so there's no need to check if you have collided with the player.
        //if (!other.CompareTag("Player")) return; // Must tag sir hooksalot as player

        /* We have a reference to the player rigidbody through the GameManager, and since the powerup can only collide with the player, there's no need to do a null-check.
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;
        */
        Rigidbody2D rb = GameManager.playerRB;

        // Grappling hook interaction
        // GrapplingHook hook = other.GetComponent<GrapplingHook>(); // We also have a reference to the grappling hook through GameManager, no need to run this expensive function.
        GrapplingHook hook = GameManager.hook;
        if (/*hook != null && */hook.hookLaunched) // Again, no need to do a null-check.
        {
            // Retract the hook to prevent janky physics
            hook.SwitchHookState();
        }

        // Vertical launch
        Vector2 currentVel = rb.linearVelocity;
        currentVel.y = 0f;  // Reset Y velocity for a more consistent launch 
        rb.linearVelocity = currentVel;

        rb.AddForce(Vector2.up * launchForce, ForceMode2D.Impulse);

        Destroy(gameObject);
    }
}