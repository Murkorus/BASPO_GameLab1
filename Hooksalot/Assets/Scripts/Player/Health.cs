using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float hp;
    public float regenCooldown;
    public float regenSpeed;
    private float maxHP;
    [HideInInspector] public float regenTimer;

    private float corruptionStartingScale;
    private float vignetteStartingScale;

    [Header("References")]
    [SerializeField] GameObject deathScreen;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highscoreText;
    [SerializeField] GameObject vignette;
    [SerializeField] GameObject corruptionVeins;

    private void Start()
    {
        maxHP = hp;
        corruptionStartingScale = corruptionVeins.transform.localScale.x;
        vignetteStartingScale = vignette.transform.localScale.x;
    }

    private void Update()
    {
        if (GameManager.playerHealth.hp <= 0)
        {
            KillPlayer();
        }

        regenTimer += Time.deltaTime;

        if(regenTimer > regenCooldown)
        {
            hp = Mathf.Clamp(hp + Time.deltaTime * regenSpeed, 0, maxHP);
        }

        float hpPercent = 1 - (hp / maxHP);
        vignette.transform.localScale = Vector3.one * Mathf.Lerp(vignetteStartingScale, 1, hpPercent);
        corruptionVeins.transform.localScale = Vector3.one * Mathf.Lerp(corruptionStartingScale, 1, hpPercent);
    }

    private void KillPlayer()
    {
        GameManager.playerIsDead = true;

        deathScreen.SetActive(true);
        scoreText.text = ((int)GameManager.playerMaxY).ToString();
        float highScore = PlayerPrefs.GetFloat("Highscore");
        highScore = highScore > GameManager.playerMaxY ? highScore : GameManager.playerMaxY;
        highscoreText.text = ((int)highScore).ToString();
        PlayerPrefs.SetFloat("Highscore", highScore);
    }
}
