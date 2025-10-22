using UnityEngine;

public class VerticalLaunchPowerup : MonoBehaviour
{
    
    public float launchForce = 15f;   // Strength of upward boost

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return; // Must tag sir hooksalot as player

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        // Grappling hook interaction
        GrapplingHook hook = other.GetComponent<GrapplingHook>();
        if (hook != null && hook.hookLaunched)
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