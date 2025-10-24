using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraHeight;

    private void Update()
    {
        float direction = ((GameManager.playerRB.transform.position + Vector3.up * cameraHeight + (Vector3)GameManager.playerRB.linearVelocity) - Camera.main.transform.position).y; // Positive means up, negative means down
        // direction = Mathf.Clamp(direction, 0, Mathf.Infinity);
        Camera.main.transform.position += Vector3.up * direction * cameraSpeed * Time.deltaTime;
    }
}
