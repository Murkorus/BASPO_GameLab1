using System.Collections.Generic;
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
        [Range(0,1)] public float spawnChance;

        public override string ToString() => $"{spawnableObject.name}, {spawnChance * 100}% chance.";
    }

    public List<Spawnable> platformSpawnOptions = new List<Spawnable>();
    public List<Spawnable> enemySpawnOptions = new List<Spawnable>();

    [HideInInspector] public bool isActive; // Switched on when the player gets close enough, and off when spawning is finished.

    private void Awake() // Awake for SEO purposes. Must add itself to the SpawnerManager list before the SpawnerManager list does anything.
    {
        GetComponent<SpriteRenderer>().enabled = false;
        SpawnerManager.spawnZones.Add(this);
    }
}
