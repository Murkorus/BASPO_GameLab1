using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int platformsBroken;
    public static float platformBreakScore;
    public static int enemiesKilled;
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
    public float scoreFromBreakingPlatform; // How much score should the player be rewarded for breaking a platform?

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

        if (Time.timeSinceLevelLoad > corruptionScript.startTimeDelay && GameManager.playerMaxY >= corruptionScript.startHeightDelay)
        {
            timeTaken += Time.deltaTime;
        }
        CalculateAllScores();
        scoreText.text = ((int)TotalScore()).ToString();
    }

    public void CalculateAllScores()
    {
        timeScore = timeTakenMult * Mathf.Sqrt(Mathf.Clamp(Mathf.Pow((timeTaken - minimumTimeForScore) * timeSpeed, timeScorePower), 0, Mathf.Infinity));
        platformBreakScore = platformsBroken * scoreFromBreakingPlatform * platformBreakMult;
        heightScore = Mathf.Clamp(GameManager.playerMaxY * maxHeightMult, 0, Mathf.Infinity);
    }

    public static float TotalScore()
    {
        return heightScore + timeScore;
    }

    public static float HighScore()
    {
        float highscore = PlayerPrefs.GetFloat("Highscore");
        float totalScore = TotalScore();
        highscore = highscore > totalScore ? highscore : totalScore;
        PlayerPrefs.SetFloat("Highscore", highscore);
        return highscore;
    }

    public static string ScoreFactors()
    {
        int minutes = Mathf.FloorToInt(timeTaken / 60);
        int seconds = Mathf.FloorToInt(timeTaken - minutes * 60);
        string time = string.Format("{0:00}:{1:00}", minutes, seconds);

        string scoreFactors =   $"{(int)GameManager.playerMaxY}\n" +
                                $"{(int)heightScore}\n" +
                                $"{time}\n" +
                                $"{(int)timeScore}\n" +
                                $"{platformsBroken}\n" +
                                $"{(int)platformBreakScore}\n" +
                                $"{enemiesKilled}\n" +
                                $"{(int)enemyKillScore}";
        return scoreFactors;
    }

    public static string HighscoreFactors()
    {
        float highscore = PlayerPrefs.GetFloat("Highscore");
        string highscoreFactors = PlayerPrefs.GetString("HighscoreFactors");
        if (highscoreFactors == string.Empty)
        {
            highscoreFactors = $"0\n0\n00:00\n0\n0\n0\n0\n0";
        }
        highscoreFactors = highscore > TotalScore() ? highscoreFactors : ScoreFactors();
        PlayerPrefs.SetString("HighscoreFactors", highscoreFactors);
        return highscoreFactors;
    }
}
