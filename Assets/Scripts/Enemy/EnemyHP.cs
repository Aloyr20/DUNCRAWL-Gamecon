using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;

public class EnemyHP : MonoBehaviour
{
    public enum SpellType { None, Ice, Fire, Poison }
    public bool spellDamage;
    public Slider healthBar;
    public float maxHealth = 200f;
    public float currentHealth;

    public float originalSpeed;
    public EnemyAi ai;

    [Header("Status Effects")]
    private bool isPoisoned = false;
    private bool isBurning = false;
    private bool isSlowed = false;

    [Header("Health Bar Animation")]
    public float barSmoothTime = 0.1f;
    private float barTimer = 0f;
    private float startValue;
    private float targetValue;
    bool isDead;

    public float DissolveTime = 3f;

    GameObject mesh;
    public Material dissolveMaterial;
    private Material _originalMeshMaterial;

    public LevelClearController levelClearController;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
            _animator.ResetTrigger("Attack");
            _animator.ResetTrigger("Die");
            _animator.SetBool("Moving", false);
        }

        ai = GetComponent<EnemyAi>();
        originalSpeed = ai.nav.speed;

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.minValue = 0;
            healthBar.value = currentHealth;
        }

        startValue = currentHealth;
        targetValue = currentHealth;

        mesh = transform.GetChild(1).GetChild(1).gameObject;
        _originalMeshMaterial = mesh.GetComponent<SkinnedMeshRenderer>().material;
    }

    void Update()
    {
        if (barTimer < barSmoothTime)
        {
            barTimer += Time.deltaTime;
            float n = barTimer / barSmoothTime;
            n = 1f - Mathf.Pow(1f - n, 3f);
            healthBar.value = Mathf.Lerp(startValue, targetValue, n);
        }
    }

    public void ApplySpellEffect(SpellType effect)
    {
        if (effect == SpellType.Ice)
        {
            if (!isSlowed)
            {
                StartCoroutine(SlowRoutine(0.5f, 5f, 50f));
            }
        }
        else if (effect == SpellType.Fire)
        {
            if (!isBurning)
            {
                StartCoroutine(DamageOverTimeRoutine(20f, 3, 2f, () => isBurning = false, () => isBurning = true));
            }
        }
        else if (effect == SpellType.Poison)
        {
            if (!isPoisoned)
            {
                StartCoroutine(DamageOverTimeRoutine(5f, 15, 1f, () => isPoisoned = false, () => isPoisoned = true));
            }
        }
    }

    IEnumerator DamageOverTimeRoutine(float dmgPerTick, int ticks, float tickRate, System.Action endStatus, System.Action startStatus)
    {
        if (!isDead)
        {
            startStatus.Invoke();

            for (int i = 0; i < ticks; i++)
            {
                if (!isDead)
                {
                    TakeDamage(dmgPerTick);
                    yield return new WaitForSeconds(tickRate);
                }
            }

            endStatus.Invoke();
        }
    }

    IEnumerator SlowRoutine(float slowPercent, float duration, float slowDamage)
    {
        if (!isDead)
        {
            isSlowed = true;
            ai.nav.speed = originalSpeed * (1f - slowPercent);
            TakeDamage(slowDamage);

            yield return new WaitForSeconds(duration);

            ai.nav.speed = originalSpeed;
            isSlowed = false;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        startValue = healthBar.value;
        targetValue = currentHealth;
        barTimer = 0f;

        if (GetComponentInChildren<ParticleSystem>() != null)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }

        CheckAlive();
    }

    void CheckAlive()
    {
        if (currentHealth <= 0)
        {
            isDead = true;

            mesh.GetComponent<SkinnedMeshRenderer>().material = dissolveMaterial;

            StartCoroutine(StartDissolving());

            if (_animator != null)
            {
                _animator.SetTrigger("Die");
            }

            if (ai != null)
            {
                ai.enabled = false;
            }

            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Collider>().enabled = false;

            if (levelClearController != null)
            {
                levelClearController.EnemyKilled();
            }

            Destroy(gameObject, DissolveTime - 0.6f);
        }
    }

    public IEnumerator StartDissolving()
    {
        SetDissolveRate(0);

        float time = 0;
        while (time < DissolveTime)
        {
            time += Time.deltaTime;
            float rate = Mathf.Clamp01(time / DissolveTime);
            SetDissolveRate(rate);
            yield return null;
        }

        SetDissolveRate(1);
    }

    private void SetDissolveRate(float value)
    {
        int shaderId = Shader.PropertyToID("_ClipRate");
        mesh.GetComponent<SkinnedMeshRenderer>().material.SetFloat(shaderId, value);
    }
}