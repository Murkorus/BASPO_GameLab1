using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static float platformBreakScore;
    public static float enemyKillScore;
    public static float speedScore;
    public static float timeScore;
    public static float timeTaken;
    public static float heightScore;
    // Any more ideas for things we can give the player some extra score for doing?

    [Header("Variables")]
    [SerializeField] float minimumTimeForScore;
    [Range(1, 2)] [SerializeField] float timeScorePower; // 1 means the time score follows a square root curve, 2 means it follows a linear function.
    [SerializeField] float timeSpeed; // How fast time go for the purposes of calculating score? Determines how many seconds pass per real second.

    [Header("Multipliers")]
    [SerializeField] float platformBreakMult;
    [SerializeField] float enemyKillMult;
    [SerializeField] float speedMult;
    [SerializeField] float timeTakenMult;
    [SerializeField] float maxHeightMult;

    [Header("References")]
    [SerializeField] Lava corruptionScript;
    [SerializeField] TMP_Text scoreText;

    private void Update()
    {
        if (GameManager.playerIsDead)
        {
            return;
        }

        scoreText.text = ((int)TotalScore()).ToString();

        if (Time.timeSinceLevelLoad > corruptionScript.startTimeDelay && GameManager.playerMaxY >= corruptionScript.startHeightDelay)
        {
            timeTaken += Time.deltaTime;
        }
        
        timeScore = timeTakenMult * Mathf.Sqrt(Mathf.Clamp(Mathf.Pow((timeTaken - minimumTimeForScore) * timeSpeed, timeScorePower), 0, Mathf.Infinity));
        heightScore = Mathf.Clamp(GameManager.playerMaxY * maxHeightMult, 0, Mathf.Infinity);
    }

    public static float TotalScore()
    {
        return heightScore + timeScore;
    }

    public static float HighScore()
    {
        float highScore = PlayerPrefs.GetFloat("Highscore");
        float totalScore = TotalScore();
        highScore = highScore > totalScore ? highScore : totalScore;
        PlayerPrefs.SetFloat("Highscore", highScore);
        return highScore;
    }
}
