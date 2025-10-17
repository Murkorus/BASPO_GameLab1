using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool debugMode;
    public static bool playerIsDead;
    public static float score;

    public static Rigidbody2D playerRB;

    [SerializeField] bool debug;

    private void Awake()
    {
        playerRB = FindFirstObjectByType<Movement>().GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        debugMode = debug;
    }
}
