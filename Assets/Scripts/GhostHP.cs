using UnityEngine;
using UnityEngine.UI;

public class GhostHP : MonoBehaviour
{
    public float maxHealth = 20f;
    private float currentHealth;

    public Slider healthBar;

    public Renderer meshRenderer;
    public float dissolveSpeed = 1.5f;
    private float dissolveAmount = 0f;
    private bool isDissolving = false;
    public LevelClearController levelClearController;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void Update()
    {
        if (isDissolving && meshRenderer != null)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;

            meshRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);

            if (dissolveAmount >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Ghost ghost = GetComponent<Ghost>();
        if (ghost != null)
        {
            ghost.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        isDissolving = true;

        levelClearController.EnemyKilled();
    }



    public void TakeDamage()
    {
        healthBar.value -= 20;
        //gameObject.GetComponentInChildren<ParticleSystem>().Play();
        Invoke("HealthBarFill", 0.5f);
    }
}