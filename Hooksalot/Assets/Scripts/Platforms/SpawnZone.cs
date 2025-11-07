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
    public Spawnable[] powerupSpawnOptions = new Spawnable[0];
    public Spawnable[] enemySpawnOptions = new Spawnable[0];

    private SpawnerManager sm;

    [Header("Variables")]
    [SerializeField] bool isInfinite;
    [SerializeField] bool useDefaultValues; // If true, uses the default values defined in the manager, rather than its own individual values.
    //[SerializeField] bool canSpawnPlatforms = true; // Setting this to true here because it's the easiest way of making true a default value.
    //[SerializeField] bool canSpawnPowerups;
    //[SerializeField] bool canSpawnEnemies;
    [SerializeField] float myEdgeDistance; // See SpawnerManager script for description of these values.
    [SerializeField] float myScaleVariance;
    [SerializeField] float myPositionVariance;
    [SerializeField] float myXScaleMultiplier;
    [SerializeField] float myYScaleMultiplier;
    [Range(0, 1)] [SerializeField] float myPowerupSpawnChance;
    [Range(0, 1)][SerializeField] float myEnemySpawnChance;
    [Range(1, 5)] public int maxPlatformsPerY; // How many platforms can at most spawn on the same y-level?

    [Header("Debugging")]
    public bool isActive; // Switched off at the start of the game, and on when the player gets close enough, then off again when spawning is finished.
    public bool isExhausted; // Switched off at the start of the game, and on when spawning is finished.
    public Vector2 bottomLeftCorner;
    public Vector2 topRightCorner;
    private Platform lastSpawnedPlatform;
    private float startingYPos;

    private void Awake() // Done in Awake for SEO purposes. Must add itself to the SpawnerManager list before the SpawnerManager list does anything.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        sm = FindFirstObjectByType<SpawnerManager>();
        if (!sm.spawnZones.Contains(this))
        {
            sm.spawnZones.Add(this);
        }
        
        bottomLeftCorner = transform.position - transform.localScale * 0.5f;
        topRightCorner = transform.position + transform.localScale * 0.5f;

        startingYPos = bottomLeftCorner.y;
    }

    private void UpdateNoSpawnZone()
    {
        if (isInfinite)
        {
            // Update size of the zone so it always stays well ahead of the player.
            float playerLoadingPosition = bottomLeftCorner.y + GameManager.playerRB.transform.position.y + (sm.spawningDistance + sm.playerZoneDistance + sm.spawnInterval);
            if (topRightCorner.y < playerLoadingPosition)
            {
                transform.position = Vector3.up * (startingYPos + playerLoadingPosition * 0.5f);
                transform.localScale = new Vector3(transform.localScale.x, playerLoadingPosition, 1);
            }
        }

        // This is called at the beginning of every platform spawn function call.

        // Just in case the scale of the spawn zone itself has changed, first update both corners.
        // This makes it possible to move the zone and change its scale while the game is running.
        // (Required for the infinite generation to work)
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
        if (GameManager.debugMode)
        {
            Debug.Log($"{gameObject.name}: Chose {platformsToSpawn} platforms. {transform.localScale.x} / {platformsToSpawn} = {nthXSize}.");
        }
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
            Platform newPlatform = Instantiate(platformSpawnOptions[chosenIndex].spawnableObject, position, Quaternion.identity, sm.platformParent).GetComponent<Platform>();
            newPlatform.platformScale = scale;
            lastSpawnedPlatform = newPlatform;

            // Spawn a powerup.
            if(Random.Range(0f, 1f) <= (useDefaultValues ? sm.defaultPowerupSpawnChance : myPowerupSpawnChance))
            {
                SpawnPowerup();
            }

            // Spawn a enemy
            if(Random.Range(0f, 1f) <= (useDefaultValues ? sm.defaultEnemySpawnChance : myEnemySpawnChance))
            {
                SpawnEnemy();
            }
        }
    }

    public void SpawnPowerup()
    {
        // Take advantage of the fact that platforms are already guaranteed to be somewhat spaced apart.
        // We can spawn powerups based on the position of the last spawned platform.

        // We could also, instead of spawning the powerup on top of a platform, send a circlecast downwards, and spawn the powerup somewhere in between the bottom of the platform and the place the circlecast hit.
        // This would make the powerups spawn mid-air, but might not always work if platforms are too densely packed.

        // First, decide between if the powerup should be spawned above or below the platform.
        if(Random.Range(0f, 1f) <= sm.powerupSpawningBehaviour)
        {
            // Spawn above
            Vector2 spawnpoint = (Vector2)lastSpawnedPlatform.transform.position + (lastSpawnedPlatform.platformScale.y * 0.5f + 1) * Vector2.up;
            Instantiate(powerupSpawnOptions[0].spawnableObject, spawnpoint, Quaternion.identity, sm.powerupParent);
        }
        else
        {
            // Spawn below
            // Only collide with default and grappleable layers.
            // We take the binary values of the masks we want to collide with, and add them together.
            LayerMask onlyPlatformMask = LayerMask.GetMask("Default") + LayerMask.GetMask("Grappleable");
            Vector2 circleCastOrigin = (Vector2)lastSpawnedPlatform.transform.position - (lastSpawnedPlatform.platformScale.y * 0.5f + powerupSpawnOptions[0].spawnableObject.transform.localScale.y * 0.5f + 1) * Vector2.up + Random.Range(-lastSpawnedPlatform.transform.localScale.x, lastSpawnedPlatform.transform.localScale.x) * Vector2.right;
            RaycastHit2D hit = Physics2D.CircleCast(circleCastOrigin, 1, -Vector2.up, Mathf.Infinity, onlyPlatformMask);
            if (GameManager.debugMode)
            {
                Debug.DrawLine(circleCastOrigin, hit.point, Color.yellow, 60);
            }
            // Since the circlecast can hit the default layer, and the floor is set to the default layer, hit should in theory always be true.
            if (hit)
            {
                // Find the vector between the hit point and the start point.
                Vector2 belowPlatformVector = new Vector2(circleCastOrigin.x, hit.point.y) - circleCastOrigin;

                // Pick a random spot on that vector.
                Vector2 spawnPoint = circleCastOrigin + belowPlatformVector * Random.Range(0.2f, 0.8f);

                // Spawn the powerup
                Instantiate(powerupSpawnOptions[0].spawnableObject, spawnPoint, Quaternion.identity, sm.powerupParent);
            }

        }
    }

    public void SpawnEnemy()
    {
        // Take advantage of the fact that platforms are already guaranteed to be somewhat spaced apart.
        // We can spawn enemies based on the position of the last spawned platform.

        // We could also, instead of spawning the enemy on top of a platform, send a circlecast downwards, and spawn the powerup somewhere in between the bottom of the platform and the place the circlecast hit.
        // This would make the enemies spawn mid-air, but might not always work if platforms are too densely packed.

        // First, decide between if the enemy should be spawned above or below the platform.
        if(Random.Range(0f, 1f) <= sm.enemySpawningBehaviour)
        {
            // Spawn above
            Vector2 spawnpoint = (Vector2)lastSpawnedPlatform.transform.position + (lastSpawnedPlatform.platformScale.y * 0.5f + 1) * Vector2.up;
            Instantiate(enemySpawnOptions[0].spawnableObject, spawnpoint, Quaternion.identity, sm.enemyParent);
        }
        else
        {
            // Spawn below
            // Only collide with default and grappleable layers.
            // We take the binary values of the masks we want to collide with, and add them together.
            LayerMask onlyPlatformMask = LayerMask.GetMask("Default") + LayerMask.GetMask("Grappleable");
            Vector2 circleCastOrigin = (Vector2)lastSpawnedPlatform.transform.position - (lastSpawnedPlatform.platformScale.y * 0.5f + enemySpawnOptions[0].spawnableObject.transform.localScale.y * 0.5f + 1) * Vector2.up + Random.Range(-lastSpawnedPlatform.transform.localScale.x, lastSpawnedPlatform.transform.localScale.x) * Vector2.right;
            RaycastHit2D hit = Physics2D.CircleCast(circleCastOrigin, 1, -Vector2.up, Mathf.Infinity, onlyPlatformMask);
            if (GameManager.debugMode)
            {
                Debug.DrawLine(circleCastOrigin, hit.point, Color.yellow, 60);
            }
            // Since the circlecast can hit the default layer, and the floor is set to the default layer, hit should in theory always be true.
            if (hit)
            {
                // Find the vector between the hit point and the start point.
                Vector2 belowPlatformVector = new Vector2(circleCastOrigin.x, hit.point.y) - circleCastOrigin;

                // Pick a random spot on that vector.
                Vector2 spawnPoint = circleCastOrigin + belowPlatformVector * Random.Range(0.2f, 0.8f);

                // Spawn the enemy
                Instantiate(enemySpawnOptions[0].spawnableObject, spawnPoint, Quaternion.identity, sm.enemyParent);
            }

        }
    }
}
