using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnerManager : MonoBehaviour
{
    [HideInInspector] public List<SpawnZone> spawnZones = new List<SpawnZone>();
    private List<SpawnZone> activeZones = new List<SpawnZone>();

    // Platforms will spawn as the player ascends.
    // Enemies will spawn periodically off-screen, even if the player fell down. (Not *while* falling, though)

    // Platforms will despawn when engulfed by corruption.
    // Enemies will despawn when sufficiently far away from the player, or when engulfed by corruption.

    [SerializeField] bool generateSpawnZones; // Set this to true if you want to auto-generate spawnzones based on the list below. They will all be default spawn zones and have the width of the entire screen. Useful for making bulk spawnzones or very big ones.
    [SerializeField] GameObject spawnZonePrefab;
    [SerializeField] List<Vector2> spawnZoneBoundaries = new List<Vector2>(); // Each Vector2 defines a spawnzone with the bottom being the x-coordinate and the top being the y-coordinate.

    [Header("Platform Variables")]
    [SerializeField] float spawnInterval; // How far between each platform?
    
    [SerializeField] float activationDistance; // How far below a spawnZone should the player be for it to be activated?
    public float playerZoneDistance; // How far above the player should spawning be disallowed? Note: If set to values lower than around half the size of the camera on the y-axis, spawning will happen on-screen.
    //[SerializeField] int batchSize; // How many platforms should be spawned per 'batch'?
    public float defaultEdgeDistance; // What is the minimum distance between the edges of any two platforms?
    public float defaultScaleVariance; // What is the minimum and maxium variance in platform scale? Setting this to 0 means every platform is the same size.
    public float defaultPositionVariance; // What is the minimum and maximum variance in the distance between platforms? Setting this to 0 means every platform will be equally far apart.
    public float defaultXScaleMultiplier;
    public float defaultYScaleMultiplier;

    [Header("Enemy Variables")]
    [SerializeField] float timeBetweenWaves;
    [SerializeField] Vector2 waveSizeMinMax;

    private float spawnNextPlatformAt;

    private void Awake()
    {
        if (generateSpawnZones)
        {
            for (int i = 0; i < spawnZoneBoundaries.Count; i++)
            {
                Vector2 position = new Vector2(0, spawnZoneBoundaries[i].x + (spawnZoneBoundaries[i].y - spawnZoneBoundaries[i].x) * 0.5f);
                Instantiate(spawnZonePrefab, position, Quaternion.identity).transform.localScale = new Vector2(GameManager.halfScreenSize.x * 2, spawnZoneBoundaries[i].y - spawnZoneBoundaries[i].x);
            }
        }
    }

    private void Update()
    {
        //Debug.Log($"SpawnNextPlatformAt: {spawnNextPlatformAt}, playerMaxY: {GameManager.playerMaxY}.");
        // Keep track of the player's max achived y-coordinate.
        // When the player's max achieved y-coordinate equals or exceeds the "spawn next playform at y-coordinate" number, spawn a batch of platforms, and increase that number by X, where X is the spawnInterval.
        if(GameManager.playerMaxY + playerZoneDistance >= spawnNextPlatformAt)
        {
            Debug.Log("Spawn Platform");
            SpawnPlatforms();
            spawnNextPlatformAt += spawnInterval;
        }
    }
    private void SpawnPlatforms()
    {
        // First, loop over every SpawnZone to see which ones are currently active.
        // Each SpawnZone will keep track of the platforms spawned within them, in order to update a list of coordinate zones wherein platforms are not allowed to be spawned.
        // This coordinate zone also includes the player's "zone of no spawning".
        // Then, for each platform that should be spawned according to the batchSize variable, a function SpawnPlatform() will be called on the active SpawnZone.
        // This function will both spawn a platform, and then update the no-spawn zone.

        UpdateZoneStates();
        for(int i = 0; i < activeZones.Count; i++)
        {
            activeZones[i].SpawnPlatform();
        }
    }

    private void UpdateZoneStates()
    {
        for(int i = 0; i < spawnZones.Count; i++) // Remove exhausted zones
        {
            if (spawnZones[i].isExhausted)
            {
                // Potentially also GameObject.Destroy() the spawnZones?
                activeZones.Remove(spawnZones[i]);
                spawnZones.RemoveAt(i);
                i--;
            }
        }

        for(int i = 0; i < spawnZones.Count; i++) // Activate zones
        {
            if (spawnZones[i].isActive)
            {
                continue;
            }
            if (spawnZones[i].bottomLeftCorner.y <= GameManager.playerRB.transform.position.y + activationDistance)
            {
                activeZones.Add(spawnZones[i]);
                spawnZones[i].isActive = true;
            }
        }
    }
}
