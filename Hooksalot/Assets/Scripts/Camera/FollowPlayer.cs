using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] float cameraSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float cameraHeight;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        //float playerDistance = Vector2.Distance(GameManager.playerRB.transform.position, cam.transform.position);
        // Positive means up, negative means down

        float direction = ((GameManager.playerRB.transform.position + Vector3.up * cameraHeight + (Vector3)GameManager.playerRB.linearVelocity) - cam.transform.position).y;
        cam.transform.position += Vector3.up * Mathf.Sign(direction) * Mathf.Clamp(Mathf.Abs(direction) * cameraSpeed, minSpeed, Mathf.Infinity) * Time.deltaTime;

        //cam.transform.position = Vector3.up * (Mathf.Round(cam.transform.position.y * 1000) / 1000) + Vector3.forward * cam.transform.position.z;
    }
}
