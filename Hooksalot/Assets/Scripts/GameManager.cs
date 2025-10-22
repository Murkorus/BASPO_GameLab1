using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool debugMode;
    public static bool playerIsDead;
    public static float score;
    public static float playerMaxY;
    public static Vector2 halfScreenSize;

    public static Rigidbody2D playerRB;

    [SerializeField] bool debug;

    private void Awake()
    {
        playerRB = FindFirstObjectByType<Movement>().GetComponent<Rigidbody2D>();

        halfScreenSize.y = Camera.main.orthographicSize;
        // Given an aspect ratio 9:16, we know that 9 / 16 = x / y
        // We also know y, as orthographicSize * 2
        // Thus, x = 9 / 16 * orthographicSize * 2
        halfScreenSize.x = Camera.main.aspect * halfScreenSize.y;
    }

    private void Update()
    {
        debugMode = debug;

        playerMaxY = playerRB.transform.position.y > playerMaxY ? playerRB.transform.position.y : playerMaxY;
    }
}
