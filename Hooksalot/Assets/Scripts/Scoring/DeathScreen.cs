using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [Header("References")]
    public TMP_Text score;
    public TMP_Text highscore;
    public TMP_Text scoreFactors;
    public TMP_Text highscoreFactors;

    private void Awake()
    {
        score.text = ((int)ScoreManager.TotalScore()).ToString();
        highscore.text = ((int)ScoreManager.HighScore()).ToString();
        scoreFactors.text = ScoreManager.ScoreFactors();
        highscoreFactors.text = ScoreManager.HighscoreFactors();
    }
}
