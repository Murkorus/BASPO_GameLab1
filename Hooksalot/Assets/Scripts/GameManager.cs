using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool debugMode;
    public static bool playerIsDead;
    public static float score;
    public static float playerMaxY;

    public static Rigidbody2D playerRB;

    [SerializeField] bool debug;

    private void Awake()
    {
        playerRB = FindFirstObjectByType<Movement>().GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        debugMode = debug;

        playerMaxY = playerRB.transform.position.y > playerMaxY ? playerRB.transform.position.y : playerMaxY;
    }
}
