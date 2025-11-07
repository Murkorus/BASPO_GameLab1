using UnityEngine;

public class SuicideEnemy : MonoBehaviour
{
    public float Explosion = 15f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = GameManager.playerRB;

        GrapplingHook hook = GameManager.hook;
        if (hook.hookLaunched) 
        {
            hook.SwitchHookState();
        }

        Vector2 currentVel = rb.linearVelocity;
        currentVel.y = 0f; 
        rb.linearVelocity = currentVel;

        Vector2 direction = collision.transform.position - transform.position;
        
        rb.AddForce(direction * Explosion, ForceMode2D.Impulse);

        Destroy(gameObject);
    }
}
