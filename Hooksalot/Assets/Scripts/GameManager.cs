using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool debugMode;
    public static bool playerIsDead;
    public static float score;
    [SerializeField] bool debug;
    private void Update()
    {
        debugMode = debug;
    }
}
