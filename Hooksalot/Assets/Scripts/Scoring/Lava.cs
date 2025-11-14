using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lava : MonoBehaviour
{
    // This script will be attached to a GameObject in the scene which has a trigger collider set to only interact with the player and platforms.
    // If the player touches this trigger, they will die.
    // Whenever a platform is engulfed by this GameObject, it is destroyed.
    // This GameObject will slowly rise, chasing the player.

    [Header("Variables")]
    [SerializeField] bool doDistanceBasedSpeed; // If true, the lava will speed up and slow down based on distance to the player. Min and max speed capped by numbers below.
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float distanceBasedSpeedScale; // How fast should the lava accelerate? Lower value = faster. This number is the base in a logarithmic function.
    [SerializeField] float speed;
    public float startTimeDelay;
    public float startHeightDelay;
    [SerializeField] float destructionBuffer; // How far below the surface of the lava should the top of a platform be before it gets destroyed?
    [SerializeField] float buoyancyForce; // How much force is applied to the player to keep them buoyant?
    [SerializeField] float floatingLevel; // How far below the surface of the corruption should the player be floating? Negative means below the surface.
    [SerializeField] float maxBuoyancyForce; // At what velocity should the player stop being pulled upwards?
    [SerializeField] float instaKillDepth; // How far below the surface should the player be before they are just killed instantly as a fallback in case they are going too fast?

    public List<Platform> platformsToDestroy = new List<Platform>();

    public float currentSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            return;
        }
        platformsToDestroy.Add(collision.GetComponentInParent<Platform>());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6) // 6 is player layer
        {
            GameManager.playerHealth.hp -= Time.deltaTime;
            GameManager.playerHealth.regenTimer = 0;
            float topOfCorruption = transform.position.y + transform.localScale.y * 0.5f + floatingLevel;
            if (GameManager.playerRB.transform.position.y < topOfCorruption)
            {
                // If the player is below the top of the corruption.
                if(GameManager.playerRB.linearVelocityY < maxBuoyancyForce)
                {
                    GameManager.playerRB.AddForce((topOfCorruption - GameManager.playerRB.transform.position.y) * buoyancyForce * Time.deltaTime * Vector2.up);
                }
            }

            if(GameManager.playerRB.transform.position.y < topOfCorruption + instaKillDepth)
            {
                GameManager.playerHealth.hp = 0;
            }
        }
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad > startTimeDelay && GameManager.playerMaxY >= startHeightDelay)
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
