using UnityEngine;

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
            SwordAnim.speed = 1f;
        }

        if ((info.IsName(currentstate) == false) && GetCurrentStateName(info) == "Slash")
        {
            if (!sourceSwing.isPlaying)
            {
                sourceSwing.PlayOneShot(sound[0], 1.5f);
            }
            EnemyDetermine();
        }

        if ((info.IsName(currentstate) == false) && GetCurrentStateName(info) == "Windup")
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

        for (int i = 0; i < hitCount; i++)
        {
            Collider enemy = _enemies[i];

            if (!sourceHit.isPlaying)
            {
                sourceHit.PlayOneShot(sound[1], 1.4f);
            }

            DealDamageToEnemy(enemy.gameObject);
        }
    }

    void DealDamageToEnemy(GameObject enemy)
    {
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
                ghost.TakeDamage(damage);
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
            EnemyHP hp = enemy.GetComponent<EnemyHP>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
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