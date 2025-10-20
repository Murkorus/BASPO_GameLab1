using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static List<SpawnZone> spawnZones = new List<SpawnZone>();

    // Platforms will spawn as the player ascends.
    // Enemies will spawn periodically off-screen, even if the player fell down. (Not *while* falling, though)

    // Platforms will despawn when engulfed by corruption.
    // Enemies will despawn when sufficiently far away from the player, or when engulfed by corruption.

    [Header("Platform Variables")]
    [SerializeField] float spawnInterval; // How far between each platform?
    [SerializeField] float minimumEdgeDistance; // What is the minimum distance between the edges of any two platforms?
    [SerializeField] float activationDistance; // How far below a spawnZone should the player be for it to be activated?
    [SerializeField] float playerZoneDistance; // How far above the player should spawning be disallowed? Note: If set to values lower than around half the size of the camera on the y-axis, spawning will happen on-screen.
    [SerializeField] int batchSize; // How many platforms should be spawned per 'batch'?

    [Header("Enemy Variables")]
    [SerializeField] float timeBetweenWaves;
    [SerializeField] Vector2 waveSizeMinMax;

    private void SpawnPlatform()
    {
        // First, loop over every SpawnZone to see which one is currently active.
        // Each SpawnZone will keep track of the platforms spawned within them, in order to update a list of coordinate zones wherein platforms are not allowed to be spawned.
        // This coordinate zone also includes the player's "zone of no spawning".
        // Then, for each platform that should be spawned according to the batchSize variable, a function SpawnPlatform() will be called on the active SpawnZone.
        // This function will both spawn a platform, and then update the no-spawn zone.
    }
}
