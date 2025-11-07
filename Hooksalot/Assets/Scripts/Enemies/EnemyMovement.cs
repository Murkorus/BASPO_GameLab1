using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float EnemySpeed = 10f;
    public Transform playerTransform;
    private bool isChasing = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        isChasing = true;

    }

    void Update()
    {
        if (isChasing == true)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)(direction * EnemySpeed * Time.deltaTime);
        }
    }
}
