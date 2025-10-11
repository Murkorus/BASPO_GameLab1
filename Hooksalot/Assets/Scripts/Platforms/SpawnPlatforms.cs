using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatforms : MonoBehaviour
{
    // Spawn platforms ahead of the camera, delete them when they get far enough away
    // Make it possible to configure if platforms should spawn only between certain heights (So we can do manual level design)

    [SerializeField] GameObject player;

    [Header("Spawning Settings")]
    [SerializeField] float spawnPlatformEveryXUnits;
    [SerializeField] int platformsSpawned;
    [SerializeField] float platformYOffsetLimit; // If this is set too close to the same number as spawnPlatformEveryXUnits, you might get overlapping platforms.
    [SerializeField] Vector2[] disallowedZones; // We can add zones in here to prevent platforms from spawning, so we can design it manually. E.g. inputting (5, 10), would disallow platforms to spawn between y = 5 and y = 10.
    [SerializeField] float spawnHeight; // How far ahead of the player should the platforms be spawned? (20 is a decent value)

    private float unitsSinceLastSpawn;
    private float lastSpawnHeight;
    [SerializeField] GameObject platformPrefab;
    List<GameObject> spawnedPlatforms = new List<GameObject>();

    private void Start()
    {
        lastSpawnHeight = -20;
    }

    private void Update()
    {
        unitsSinceLastSpawn = Mathf.Clamp(player.transform.position.y - lastSpawnHeight, 0, Mathf.Infinity);
        if(unitsSinceLastSpawn >= spawnPlatformEveryXUnits)
        {
            for(int i  = 0; i < platformsSpawned; i++)
            {
                SpawnPlatform();
            }
        }

        if(Time.time % 10 <= Time.deltaTime) // Every 10 seconds, despawn old platforms
        {
            for(int i = 0; i < spawnedPlatforms.Count; i++)
            {
                float despawnThreshold = 15;
                float distance = Mathf.Clamp(Camera.main.transform.position.y - spawnedPlatforms[i].transform.position.y, 0, Mathf.Infinity);
                if(distance >= despawnThreshold)
                {
                    Destroy(spawnedPlatforms[i]);
                    spawnedPlatforms.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private void SpawnPlatform()
    {
        // Calculate the possible spawn area for the y-axis
        Vector2 spawnZone = new Vector2(player.transform.position.y + spawnHeight - platformYOffsetLimit, player.transform.position.y + spawnHeight + platformYOffsetLimit);
        Debug.Log(spawnZone);
        for(int i = 0; i < disallowedZones.Length; i++)
        {
            // If either the bottom or the top value of the spawnZone is between the values of the disallowed zone, then the platform should not spawn.
            if(spawnZone.x < disallowedZones[i].y && spawnZone.x > disallowedZones[i].x)
            {
                return;
            }
            if(spawnZone.y > disallowedZones[i].x && spawnZone.y < disallowedZones[i].y)
            {
                return;
            }
        }

        // Spawn them ahead of player, and despawn them behind camera.
        Vector2 platformPosition = new Vector2(Random.Range(-11, 11), Random.Range(spawnZone.x, spawnZone.y));
        GameObject spawnedPlatform = Instantiate(platformPrefab, (Vector3)platformPosition, Quaternion.identity, null);
        spawnedPlatform.transform.localScale = new Vector3(Random.Range(0.3f, 5f), Random.Range(0.5f, 1.5f), 1);
        spawnedPlatforms.Add(spawnedPlatform);
        lastSpawnHeight = player.transform.position.y;
    }
}
