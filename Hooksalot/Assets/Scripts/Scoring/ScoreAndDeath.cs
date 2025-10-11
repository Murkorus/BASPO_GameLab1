using TMPro;
using UnityEngine;

public class CalculateScore : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] TMP_Text scoreText;

    private void Update()
    {
        if(player.transform.position.y > GameManager.score)
        {
            GameManager.score = player.transform.position.y;
            scoreText.text = ((int)GameManager.score).ToString();
        }
    }
}
