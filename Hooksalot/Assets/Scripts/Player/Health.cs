using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        FindFirstObjectByType<ScoreManager>().CalculateAllScores();
        SceneManager.LoadScene("DeathScreen");
    }
}
