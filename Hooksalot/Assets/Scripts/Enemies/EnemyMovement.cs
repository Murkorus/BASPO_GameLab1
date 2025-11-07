using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float EnemySpeed = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Rigidbody2D rb = GameManager.playerRB;
        Vector2 direction = transform.position - collision.transform.position;
        rb.AddForce(direction * EnemySpeed, ForceMode2D.Impulse);



    }

}
