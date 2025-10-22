using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    // This script will be placed on semi-transparent squares with no colliders that can be placed around the map.
    // This script will be on prefabs in the same folder as the script. The prefabs are as described above, and can simply be dragged into a scene and rescaled with the rect transform tool.
    // Multiple of this script will be in the same scene.
    // The squares will disappear on game start so they're only there for the visual reference of the level designer.
    // This is mostly just a data container.
    [System.Serializable]
    public struct Spawnable
    {
        public GameObject spawnableObject;
        public int spawnWeight;

        public override string ToString() => $"{spawnableObject.name}, {spawnWeight} weight.";
    }

    public Spawnable[] platformSpawnOptions = new Spawnable[0];
    public Spawnable[] enemySpawnOptions = new Spawnable[0];

    private SpawnerManager sm;

    [Header("Variables")]
    [SerializeField] bool useDefaultValues; // If true, uses the default values defined in the manager, rather than its own individual values.
    [SerializeField] float myEdgeDistance; // See SpawnerManager script for description of these values.
    [SerializeField] float myScaleVariance;
    [SerializeField] float myPositionVariance;
    [SerializeField] float myXScaleMultiplier;
    [SerializeField] float myYScaleMultiplier;
    [Range(1, 5)] public int maxPlatformsPerY; // How many platforms can at most spawn on the same y-level?

    [HideInInspector] public bool isActive; // Switched off at the start of the game, and on when the player gets close enough, then off again when spawning is finished.
    [HideInInspector] public bool isExhausted; // Switched off at the start of the game, and on when spawning is finished.
    [HideInInspector] public Vector2 bottomLeftCorner;
    [HideInInspector] public Vector2 topRightCorner;
    private Transform lastSpawnedPlatform;

    private void Awake() // Done in Awake for SEO purposes. Must add itself to the SpawnerManager list before the SpawnerManager list does anything.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        sm = FindFirstObjectByType<SpawnerManager>();
        sm.spawnZones.Add(this);

        bottomLeftCorner = transform.position - transform.localScale * 0.5f;
        topRightCorner = transform.position + transform.localScale * 0.5f;
    }

    private void UpdateNoSpawnZone()
    {
        // This is called at the beginning of every platform spawn function call.

        // Just in case the scale of the spawn zone itself has changed, first update both corners.
        // This makes it possible to move the zone and change its scale while the game is running.
        bottomLeftCorner = transform.position - transform.localScale * 0.5f;
        topRightCorner = transform.position + transform.localScale * 0.5f;

        // Update bottom left corner to take into account the player's position.
        bottomLeftCorner = new Vector2(bottomLeftCorner.x, Mathf.Clamp(bottomLeftCorner.y, GameManager.playerRB.transform.position.y + sm.playerZoneDistance, topRightCorner.y));

        if(lastSpawnedPlatform == null)
        {
            return;
        }

        // If platforms should be spawned in with random rotations, this function needs to change to accomodate that.
        // Currently, it assumes all platforms spawn in with Euler(0, 0, 0) rotations.

        bottomLeftCorner = new Vector2(bottomLeftCorner.x, Mathf.Clamp(bottomLeftCorner.y, lastSpawnedPlatform.transform.position.y + lastSpawnedPlatform.transform.localScale.y * 0.5f + (useDefaultValues ? sm.defaultEdgeDistance : myEdgeDistance), topRightCorner.y));

        // If the new zone is too small for the biggest possible platform to spawn in, then disable this SpawnZone.
        if(topRightCorner.y - bottomLeftCorner.y < 1 + (useDefaultValues ? sm.defaultScaleVariance : myScaleVariance))
        {
            isActive = false;
            isExhausted = true;
        }
    }

    public void SpawnPlatform()
    {
        UpdateNoSpawnZone();

        if (!isActive)
        {
            return;
        }

        // If more than 1 platform can spawn per y-level, choose how many platforms will spawn between 1 and the max amount.
        // Then divide the spawn area into that many equal sized chunks, and spawn a platform in each chunk.

        int platformsToSpawn = Random.Range(1, maxPlatformsPerY);
        float nthXSize = transform.localScale.x / platformsToSpawn;
        Debug.Log($"{transform.localScale.x} / {platformsToSpawn} = {nthXSize}.");

        for(int i = 0; i < platformsToSpawn; i++)
        {
            // First, find out which kind of platform to spawn.
            // Choose a number between 0 and the sum of the spawn weights.
            float chosenNumber = Random.Range(0, platformSpawnOptions.Sum(x => x.spawnWeight));

            // Find out which index the chosen number corresponds to.
            int weightSum = 0;
            int chosenIndex = 0;
            for (int j = 0; j < platformSpawnOptions.Length; j++)
            {
                weightSum += platformSpawnOptions[j].spawnWeight;
                if (chosenNumber < weightSum)
                {
                    // Platform is chosen.
                    chosenIndex = j;
                    break;
                }
            }

            // Then, decide the scale of the platform.
            Vector2 scaleMult = useDefaultValues ? new Vector2(sm.defaultXScaleMultiplier, sm.defaultYScaleMultiplier) : new Vector2(myXScaleMultiplier, myYScaleMultiplier);
            Vector2 scale = new Vector2(Mathf.Clamp((1 + Random.Range(0, useDefaultValues ? sm.defaultScaleVariance : myScaleVariance)) * scaleMult.x, 0, nthXSize),
                                        Mathf.Clamp((1 + Random.Range(0, useDefaultValues ? sm.defaultScaleVariance : myScaleVariance)) * scaleMult.y, 0, topRightCorner.y - bottomLeftCorner.y));
            // Then, decide the position of the platform.
            Vector2 position = new Vector2(Random.Range(bottomLeftCorner.x + nthXSize * i + scale.x * 0.5f, bottomLeftCorner.x + nthXSize * (i + 1) - scale.x * 0.5f),
                                           Random.Range(bottomLeftCorner.y + scale.y * 0.5f, bottomLeftCorner.y + scale.y * 0.5f + (useDefaultValues ? sm.defaultPositionVariance : myPositionVariance)));
            // Then, spawn the platform.
            Transform newPlatform = Instantiate(platformSpawnOptions[chosenIndex].spawnableObject, position, Quaternion.identity).transform;
            newPlatform.GetComponent<Platform>().UpdateScale(scale);
            lastSpawnedPlatform = newPlatform;
        }
    }

    public void SpawnEnemy()
    {

    }
}
