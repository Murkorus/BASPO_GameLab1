using UnityEngine;

public class TowerBackground : MonoBehaviour
{
    [SerializeField] float width;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] BoxCollider2D collider;

    private void Update()
    {
        if(sprite != null)
        {
            sprite.size = Vector2.up * Mathf.Clamp(GameManager.playerMaxY + 50, 0, float.PositiveInfinity) + Vector2.right * width;
        }
        if(collider != null)
        {
            collider.size = Vector2.up * Mathf.Clamp(GameManager.playerMaxY + 50, 0, float.PositiveInfinity) + Vector2.right * width;
        }
        transform.position = Vector2.up * Mathf.Clamp(GameManager.playerMaxY * 0.5f, 0, float.PositiveInfinity) + Vector2.right * transform.position.x;
    }
}
