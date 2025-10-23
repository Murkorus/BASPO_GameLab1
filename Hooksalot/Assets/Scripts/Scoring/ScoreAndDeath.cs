using TMPro;
using UnityEngine;

public class CalculateScore : MonoBehaviour
{
    // This script is outdated
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
