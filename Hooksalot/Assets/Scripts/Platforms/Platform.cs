using UnityEngine;

public class Platform : MonoBehaviour
{
    // This script is attached to every platform.
    // Make it possible for the player to break platforms when they hit them if they have enough speed.
    // Make different kinds of platforms that can withstand more or less force before they break.
    // Also make a "checkpoint" platform that never breaks.

    private float durability; // The speed threshold at which the platform breaks, Average player speed is around 10-20
    private float marblePlatform = Mathf.Infinity;
    private float stoneBrickPlatform = 13;
    private float woodPlatform = 10;

    private void Start()
    {
        switch (gameObject.tag) // Use tags to differentiate platform types, more can be added as needed
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
                durability = marblePlatform; // Default durability for all other platforms can be changed here
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.playerRB.linearVelocity.magnitude > durability)
        {
            transform.parent.gameObject.SetActive(false);
            Debug.Log("Broke " + gameObject.name);
            GrapplingHook hook = collision.GetComponent<GrapplingHook>();
            if (hook != null && hook.hookLaunched)
            {
                hook.SwitchHookState();
            }
        }
        Debug.Log("Collision with " + gameObject.name);
    }
}