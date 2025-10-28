using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraHeight;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        float direction = ((GameManager.playerRB.transform.position + Vector3.up * cameraHeight + (Vector3)GameManager.playerRB.linearVelocity) - cam.transform.position).y; // Positive means up, negative means down
        // direction = Mathf.Clamp(direction, 0, Mathf.Infinity);
        cam.transform.position += Vector3.up * direction * cameraSpeed * Time.deltaTime;
    }
}
