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

        Vector2 targetPosition = (Vector2)GameManager.playerRB.transform.position + Vector2.up * cameraHeight + GameManager.playerRB.linearVelocity;
        float direction = (targetPosition - (Vector2)cam.transform.position).y;
        Vector2 thisFrameTarget = Vector2.up * Mathf.Sign(direction) * Mathf.Clamp(Mathf.Abs(direction) * cameraSpeed, minSpeed, Mathf.Infinity) * Time.deltaTime;
        cam.transform.position = new Vector3(transform.position.x, Vector2.MoveTowards(cam.transform.position, targetPosition, thisFrameTarget.magnitude).y, transform.position.z);
        //cam.transform.position = Vector3.up * (Mathf.Round(cam.transform.position.y * 1000) / 1000) + Vector3.forward * cam.transform.position.z;
    }
}
