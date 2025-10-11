using TMPro;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;
    [SerializeField] TMP_Text deathScoreText;
    [SerializeField] TMP_Text highScoreText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.playerIsDead = true;
        deathScoreText.text = ((int)GameManager.score).ToString();

        float highscore = PlayerPrefs.GetFloat("Highscore");
        if(GameManager.score > highscore)
        {
            highscore = GameManager.score;
            PlayerPrefs.SetFloat("Highscore", highscore);
        }
        highScoreText.text = ((int)highscore).ToString();

        deathScreen.SetActive(true);
    }
}