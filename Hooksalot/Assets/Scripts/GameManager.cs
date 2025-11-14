using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool debugMode;
    public static bool playerIsDead;
    public static float playerMaxY;
    public static Vector2 halfScreenSize;

    public static Movement playerMovement;
    public static Rigidbody2D playerRB;
    public static GrapplingHook hook;
    public static Health playerHealth;

    [SerializeField] bool debug;

    private void Awake()
    {
        ResetStaticVariables();
    }

    private void Update()
    {
        debugMode = debug;
        playerMaxY = playerRB.transform.position.y > playerMaxY ? playerRB.transform.position.y : playerMaxY;
    }

    private void ResetStaticVariables()
    {
        playerIsDead = false;
        playerMaxY = 0;

        ScoreManager.platformBreakScore = 0;
        ScoreManager.enemyKillScore = 0;
        ScoreManager.speedScore = 0;
        ScoreManager.timeScore = 0;
        ScoreManager.timeTaken = 0;
        ScoreManager.heightScore = 0;

        playerMovement = FindFirstObjectByType<Movement>();
        playerRB = playerMovement.GetComponent<Rigidbody2D>();
        hook = playerMovement.GetComponent<GrapplingHook>();
        playerHealth = playerMovement.GetComponent<Health>();

        halfScreenSize.y = Camera.main.orthographicSize;
        // Given an aspect ratio 9:16, we know that 9 / 16 = x / y
        // We also know y, as orthographicSize * 2
        // Thus, x = 9 / 16 * orthographicSize * 2
        halfScreenSize.x = Camera.main.aspect * halfScreenSize.y;
        Debug.Log($"Screen dimensions are: <color=green>{halfScreenSize.x * 2}x{halfScreenSize.y * 2}</color>.");
    }
}
