using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] GameObject prefabEnemy;
    private GameObject currentEnemy;
    [SerializeField] float respawnTime;

    void Update()
    {
        if (currentEnemy != null)
        {
            Invoke("SpawnEnemy", respawnTime);
        }
    }
    private void Awake()
    {
        SpawnEnemy();
    }
    public void SpawnEnemy()
    {
        GameObject temp = Instantiate(prefabEnemy, transform.position, Quaternion.identity);
    }
}
