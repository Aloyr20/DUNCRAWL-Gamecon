using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    Collider[] _enemies;
    public float _radius = 5.0f;
    public LayerMask _enemyLayer;
    public float reach;
    public int damage = 20;
    public Transform PlayerTransform;
    public Animator SwordAnim;
    public float SwingDuration;
    string currentstate;
    bool stopSlash = true;
    public AudioSource sourceSwing;
    public AudioSource sourceHit;
    public AudioClip[] sound;

    [Header("Hit Stop")]
    public float hitStopDuration = 0.09f;
    public float hitStopTimeScale = 0.04f;
    private bool _inHitStop = false;

    [Header("Sword AOE Slow")]
    public float swordAoeRadius = 4.5f;

    void Start()
    {
        AnimatorStateInfo info = SwordAnim.GetCurrentAnimatorStateInfo(0);
        currentstate = GetCurrentStateName(info);
        _enemies = new Collider[10];
    }

    void Update()
    {
        if (Inventory.IsDragging)
        {
            return;
        }

        AnimatorStateInfo info = SwordAnim.GetCurrentAnimatorStateInfo(0);

        if (Input.GetMouseButtonDown(0) && stopSlash)
        {
            stopSlash = false;
        }

        if (stopSlash)
        {
            SwordAnim.speed = 0f;
        }
        else
        {
            SwordAnim.speed = _inHitStop ? 0f : 1f;
        }

        if (!info.IsName(currentstate) && GetCurrentStateName(info) == "Slash")
        {
            if (!sourceSwing.isPlaying)
            {
                sourceSwing.PlayOneShot(sound[0], 1.5f);
            }
            EnemyDetermine();
        }

        if (!info.IsName(currentstate) && GetCurrentStateName(info) == "Windup")
        {
            stopSlash = true;
        }

        currentstate = GetCurrentStateName(info);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + (PlayerTransform.forward * reach), _radius);
    }

    void EnemyDetermine()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position + (PlayerTransform.forward * reach),
            _radius, _enemies, _enemyLayer);

        bool hitAnything = false;

        for (int i = 0; i < hitCount; i++)
        {
            Collider enemy = _enemies[i];
            if (!sourceHit.isPlaying)
            {
                sourceHit.PlayOneShot(sound[1], 1.4f);
            }
            DealDamageToEnemy(enemy.gameObject);
            TrySwordAoeSlow(enemy.gameObject);
            hitAnything = true;
        }

        if (hitAnything && !_inHitStop)
        {
            StartCoroutine(DoHitStop());
        }
    }

    IEnumerator DoHitStop()
    {
        _inHitStop = true;
        float prevTimeScale = Time.timeScale;
        Time.timeScale = hitStopTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(hitStopDuration);

        Time.timeScale = prevTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        _inHitStop = false;
    }

    void TrySwordAoeSlow(GameObject hitEnemy)
    {
        SpellEffectReceiver receiver = hitEnemy.GetComponent<SpellEffectReceiver>();
        if (receiver == null || receiver.GetCurrentEffect() != SpellEffectReceiver.SpellType.Ice)
        {
            return;
        }

        Collider[] nearby = Physics.OverlapSphere(hitEnemy.transform.position, swordAoeRadius, _enemyLayer);
        foreach (Collider col in nearby)
        {
            if (col.gameObject == hitEnemy)
            {
                continue;
            }
            SpellEffectReceiver nearbyReceiver = col.GetComponent<SpellEffectReceiver>();
            if (nearbyReceiver != null)
            {
                nearbyReceiver.ApplyEffect(SpellEffectReceiver.SpellType.Ice);
            }
        }
    }

    void DealDamageToEnemy(GameObject enemy)
    {
        SpellEffectReceiver spellReceiver = enemy.GetComponent<SpellEffectReceiver>();
        float mult = spellReceiver != null ? spellReceiver.GetIncomingDamageMultiplier() : 1f;

        if (enemy.CompareTag("Dummy"))
        {
            DummyEnemy dummy = enemy.GetComponent<DummyEnemy>();
            if (dummy != null)
            {
                dummy.TakeDamage();
            }
        }
        else if (enemy.CompareTag("Caster"))
        {
            CasterSkeletonEnemy caster = enemy.GetComponent<CasterSkeletonEnemy>();
            if (caster != null)
            {
                caster.TakeDamage();
            }
        }
        else if (enemy.CompareTag("Ghost"))
        {
            GhostHP ghost = enemy.GetComponent<GhostHP>();
            if (ghost != null)
            {
                ghost.TakeDamage((int)(damage * mult));
            }
        }
        else if (enemy.CompareTag("Spider"))
        {
            SpiderEnemy spider = enemy.GetComponent<SpiderEnemy>();
            if (spider != null)
            {
                spider.TakeDamage();
            }
        }
        else
        {
            if (spellReceiver != null)
            {
                spellReceiver.TakeDamage(damage);
            }
            else
            {
                EnemyHP hp = enemy.GetComponent<EnemyHP>();
                if (hp != null)
                {
                    hp.TakeDamage((int)(damage * mult));
                }
            }
        }
    }

    string GetCurrentStateName(AnimatorStateInfo info)
    {
        if (info.IsName("Windup"))
        {
            return "Windup";
        }
        if (info.IsName("Slash"))
        {
            return "Slash";
        }
        if (info.IsName("Recovery"))
        {
            return "Recovery";
        }
        return "Unknown";
    }
}