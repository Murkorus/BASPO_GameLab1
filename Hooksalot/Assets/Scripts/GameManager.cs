using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool debugMode;
    [SerializeField] bool debug;
    private void Update()
    {
        debugMode = debug;
    }
}
