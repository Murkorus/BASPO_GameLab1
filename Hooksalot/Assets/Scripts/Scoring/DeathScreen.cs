using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [Header("References")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private void Awake()
    {
        scoreText.text = ((int)ScoreManager.TotalScore()).ToString();
        highScoreText.text = ((int)ScoreManager.HighScore()).ToString();
    }
}
