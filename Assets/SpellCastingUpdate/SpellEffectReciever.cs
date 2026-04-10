using UnityEngine;
using System.Collections;

public class SpellEffectReceiver : MonoBehaviour
{
    public enum SpellType { None, Ice, Fire, Poison }
    private SpellType currentEffect = SpellType.None;
    private Coroutine activeEffect;

    private float incomingDamageMultiplier = 1f;

    private Renderer[] _renderers;
    private Color _originalColor;

    [Header("Particle Prefabs")]
    public GameObject iceParticlePrefab;
    public GameObject fireParticlePrefab;
    public GameObject poisonParticlePrefab;

    private GameObject _activeParticles;
    private EnemyHP _enemyHP;

    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
        _enemyHP = GetComponent<EnemyHP>();

        if (_renderers.Length > 0)
        {
            _originalColor = _renderers[0].material.GetColor("_BaseColor");
        }
    }

    public SpellType GetCurrentEffect() => currentEffect;
    public float GetIncomingDamageMultiplier() => incomingDamageMultiplier;

    public void ApplyEffect(SpellType newEffect)
    {
        if (currentEffect == SpellType.Poison && (newEffect == SpellType.Fire || newEffect == SpellType.Ice))
        {
            ForceStopEffect();
        }
        else if (currentEffect == SpellType.Fire && newEffect == SpellType.Ice)
        {
            ForceStopEffect();
        }
        else if (currentEffect == SpellType.Ice && newEffect == SpellType.Fire)
        {
            if (_enemyHP != null)
            {
                _enemyHP.ai.nav.speed = _enemyHP.originalSpeed;
            }
            ForceStopEffect();
        }
        else if (currentEffect == newEffect)
        {
            return;
        }
        else if (currentEffect != SpellType.None)
        {
            ForceStopEffect();
        }

        activeEffect = StartCoroutine(HandleEffect(newEffect));
    }

    private void ForceStopEffect()
    {
        if (activeEffect != null)
        {
            StopCoroutine(activeEffect);
        }
        CleanupEffect(currentEffect);
        currentEffect = SpellType.None;
    }

    IEnumerator HandleEffect(SpellType effect)
    {
        currentEffect = effect;
        ApplyTint(effect);
        SpawnParticles(effect);

        if (effect == SpellType.Ice)
        {
            incomingDamageMultiplier = 1f;
            if (_enemyHP != null)
            {
                _enemyHP.ai.nav.speed = _enemyHP.originalSpeed * 0.3f;
            }
            yield return new WaitForSeconds(6f);
            if (_enemyHP != null)
            {
                _enemyHP.ai.nav.speed = _enemyHP.originalSpeed;
                _enemyHP.TakeDamage(50f);
            }
        }
        else if (effect == SpellType.Fire)
        {
            incomingDamageMultiplier = 1f;
            for (int i = 0; i < 5; i++)
            {
                if (_enemyHP != null)
                {
                    _enemyHP.TakeDamage(35f);
                }
                yield return new WaitForSeconds(1.5f);
            }
        }
        else if (effect == SpellType.Poison)
        {
            incomingDamageMultiplier = 1.5f;
            for (int i = 0; i < 15; i++)
            {
                if (_enemyHP != null)
                {
                    _enemyHP.TakeDamage(5f);
                }
                yield return new WaitForSeconds(1f);
            }
            incomingDamageMultiplier = 1f;
        }

        CleanupEffect(effect);
        currentEffect = SpellType.None;
    }

    void SpawnParticles(SpellType effect)
    {
        if (_activeParticles != null)
        {
            Destroy(_activeParticles);
        }

        GameObject prefab = null;

        if (effect == SpellType.Ice)
        {
            prefab = iceParticlePrefab;
        }
        else if (effect == SpellType.Fire)
        {
            prefab = fireParticlePrefab;
        }
        else if (effect == SpellType.Poison)
        {
            prefab = poisonParticlePrefab;
        }

        if (prefab != null)
        {
            _activeParticles = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            _activeParticles.transform.localPosition = new Vector3(0f, 1f, 0f);
        }
    }

    void ApplyTint(SpellType effect)
    {
        if (_renderers == null || _renderers.Length == 0)
        {
            return;
        }

        Color tint = _originalColor;

        if (effect == SpellType.Ice)
        {
            tint = new Color(0.4f, 0.7f, 1f);
        }
        else if (effect == SpellType.Fire)
        {
            tint = new Color(1f, 0.3f, 0.1f);
        }
        else if (effect == SpellType.Poison)
        {
            tint = new Color(0.3f, 1f, 0.3f);
        }

        foreach (Renderer r in _renderers)
        {
            r.material.SetColor("_BaseColor", tint);
        }
    }

    void CleanupEffect(SpellType effect)
    {
        if (_renderers != null)
        {
            foreach (Renderer r in _renderers)
            {
                r.material.SetColor("_BaseColor", _originalColor);
            }
        }

        if (_activeParticles != null)
        {
            ParticleSystem ps = _activeParticles.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
            }
            Destroy(_activeParticles, 2f);
            _activeParticles = null;
        }

        incomingDamageMultiplier = 1f;
    }

    public void TakeDamage(float dmg)
    {
        if (_enemyHP != null)
        {
            _enemyHP.TakeDamage(dmg * incomingDamageMultiplier);
        }
    }
}