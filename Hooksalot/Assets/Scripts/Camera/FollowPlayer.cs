using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float cameraSpeed;

    private void Update()
    {
        float direction = (player.transform.position - Camera.main.transform.position).y; // Positive means up, negative means down
        direction = Mathf.Clamp(direction, 0, Mathf.Infinity);
        Camera.main.transform.position += (Vector3)(Vector2.up * direction * cameraSpeed * Time.deltaTime);
    }
}
