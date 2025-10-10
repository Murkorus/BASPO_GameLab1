using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] float hitboxRadius; // Hitbox is a circle
    [SerializeField] float gravityStrength;
    [SerializeField] LayerMask gravityMask; // Will only check for downwards collisions with objects selected here

    private void ApplyGravity()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, hitboxRadius, Vector2.down, gravityStrength, gravityMask);
        if(hit.collider != null)
        {
            transform.position -= Vector3.down * gravityStrength;
        }
        else
        {
            
        }
    }
}
