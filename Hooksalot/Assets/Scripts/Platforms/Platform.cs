using UnityEngine;

public class Platform : MonoBehaviour
{
    // This script is attached to every platform.
    // Make it possible for the player to break platforms when they hit them if they have enough speed.
    // Make different kinds of platforms that can withstand more or less force before they break.
    // Also make a "checkpoint" platform that never breaks.
    public float durability;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (GameManager.playerRB.linearVelocity.magnitude > durability)
        {
            transform.parent.gameObject.SetActive(false);
            Debug.Log("Broke " + gameObject.name);
            
        }
        Debug.Log("Collision with " + gameObject.name);
    }

    

}
