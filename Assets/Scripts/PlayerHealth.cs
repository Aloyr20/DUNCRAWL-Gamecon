using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public LevelClearController levelClearController;
    public Slider healthBar;

    public float maxTime = 0.1f;
    float t = 0f;
    float startValue;
    float targetValue;

    void Awake()
    {
        healthBar.maxValue = health;
        healthBar.minValue = 0;
        healthBar.value = health;
        startValue = health;
        targetValue = health;
    }

    void Update()
    {
        if (health <= 0)
        {
            health = 0;
            healthBar.value = 0;
            levelClearController.Lose();
            return;
        }

        if (t < maxTime)
        {
            t += Time.deltaTime;
            float n = t / maxTime;
            n = 1f - Mathf.Pow(1f - n, 3f);
            healthBar.value = Mathf.Lerp(startValue, targetValue, n);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        health = Mathf.Clamp(health, 0, health);

        startValue = healthBar.value;
        targetValue = health;
        t = 0f;
    }

    public void GiveHP(int hp)
    {
        health += hp;
        health = Mathf.Clamp(health, 0, health);

        startValue = healthBar.value;
        targetValue = health;
        t = 0f;
    }
}
