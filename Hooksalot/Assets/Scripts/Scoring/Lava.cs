using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    // This script will be attached to a GameObject in the scene which has a trigger collider set to only interact with the player and platforms.
    // If the player touches this trigger, they will die.
    // Whenever a platform is engulfed by this GameObject, it is destroyed.
    // This GameObject will slowly rise, chasing the player.

    [SerializeField] bool doDistanceBasedSpeed; // If true, the lava will speed up and slow down based on distance to the player. Min and max speed capped by numbers below.
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float distanceBasedSpeedScale; // How fast should the lava accelerate? Lower value = faster. This number is the base in a logarithmic function.
    [SerializeField] float speed;
    [SerializeField] float startDelay;
    [SerializeField] float destructionBuffer; // How far below the surface of the lava should the top of a platform be before it gets destroyed?
    public List<Platform> platformsToDestroy = new List<Platform>();

    public float currentSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6) // Layer 6 is the player layer
        {
            GameManager.playerIsDead = true;
            return;
        }

        platformsToDestroy.Add(collision.GetComponentInParent<Platform>());
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad > startDelay)
        {
            // Move upwards
            float distanceBasedSpeedMod = Mathf.Clamp(doDistanceBasedSpeed ? Mathf.Log(Vector2.Distance(transform.position + Vector3.up * transform.localScale.y * 0.5f, GameManager.playerRB.transform.position), distanceBasedSpeedScale) : 1, minSpeed, maxSpeed);
            transform.position += speed * distanceBasedSpeedMod * Time.deltaTime * Vector3.up;
            currentSpeed = speed * distanceBasedSpeedMod;
        }

        for(int i = 0; i < platformsToDestroy.Count; i++)
        {
            float platformTop = platformsToDestroy[i].transform.position.y + platformsToDestroy[i].platformScale.y * 0.5f;
            float lavaTop = transform.position.y + transform.localScale.y * 0.5f;
            if(lavaTop + destructionBuffer > platformTop)
            {
                // Platform has been engulfed, destroy it.
                Destroy(platformsToDestroy[i].gameObject);
                platformsToDestroy.RemoveAt(i);
                i--;
            }
        }
    }
}
