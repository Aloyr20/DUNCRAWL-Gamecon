using UnityEngine;
using System.Collections;
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

    public Transform canvasTransform;
    public GameObject screenBleedPanel;
    public float screenBleedDuration;
    public int maxBleedLayers;
    int currentBleedLayers;

    private void Start()
    {
        screenBleedPanel.SetActive(false);
    }

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
            StopAllCoroutines();
            DestroyBleedPanels();
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

        StartCoroutine(StartScreenBleed());
    }

    public void GiveHP(int hp)
    {
        health += hp;
        health = Mathf.Clamp(health, 0, health);

        startValue = healthBar.value;
        targetValue = health;
        t = 0f;
    }

    public IEnumerator StartScreenBleed()
    {
        if (currentBleedLayers < maxBleedLayers)
        {
            currentBleedLayers++;

            GameObject spawnedBleedLayer = Instantiate(screenBleedPanel, canvasTransform);

            spawnedBleedLayer.SetActive(true);

            yield return new WaitForSeconds(screenBleedDuration);

            Destroy(spawnedBleedLayer);

            currentBleedLayers--;
        }
    }

    public void DestroyBleedPanels()
    {
        foreach (Transform bleedPanel in canvasTransform)
        {
            if (bleedPanel.name.Contains(screenBleedPanel.name))
            {
                Destroy(bleedPanel.gameObject);
            }
        }
        currentBleedLayers = 0;
    }

}
