using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float enemySpeed = 10f;
    public float maxChaseDistance;
    public float maximumSpeed;
    public Transform playerTransform;
    private bool isChasing = false;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isChasing = true;

    }
    
    void Update()
    {
        Vector2 direction = GameManager.playerRB.transform.position - transform.position;
        if (direction.magnitude > maxChaseDistance)
        {
            isChasing = false;
            transform.up = Vector2.up;
        }
       
        if (isChasing == true)
        {
            transform.up = -direction;
            transform.position += (Vector3)Vector2.ClampMagnitude(direction * enemySpeed, maximumSpeed) * Time.deltaTime;
        }
    }
}
