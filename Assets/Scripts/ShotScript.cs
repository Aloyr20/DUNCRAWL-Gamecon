using UnityEngine;

public class ShotScript : MonoBehaviour
{
    ParticleSystem particles;
    public GameObject DropEffect;
    public bool AoE;
    public float AoEradius;
    public int damage;
    public bool stick;
    public float charge = 0f;

    private Collider _collider;
    private Rigidbody _rigidbody;
    private Light _light;
    private int _enemyLayer;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _light = GetComponent<Light>();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
    }

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        CancelInvoke();

        _collider.enabled = true;
        _rigidbody.isKinematic = false;

        if (_light != null)
        {
            _light.enabled = true;
        }

        if (particles != null)
        {
            particles.Play();
        }

        Invoke(nameof(ReturnToPool), 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Arrow hit: " + collision.gameObject.name + " Layer: " + LayerMask.LayerToName(collision.gameObject.layer));

        CancelInvoke(nameof(ReturnToPool));

        int finalDamage = Mathf.RoundToInt(damage + 2 * (damage * charge));

        if (collision.gameObject.layer == _enemyLayer)
        {
            if (AoE)
            {
                Collider[] hits = new Collider[20];
                int hitCount = Physics.OverlapSphereNonAlloc(
                    transform.position, AoEradius, hits, LayerMask.GetMask("Enemy"));

                for (int i = 0; i < hitCount; i++)
                {
                    DealDamageToEnemy(hits[i].gameObject, finalDamage);
                }
            }

            DealDamageToEnemy(collision.gameObject, finalDamage);

            if (stick)
            {
                transform.SetParent(collision.transform, true);
            }
        }

        if (particles != null)
        {
            particles.Stop();
        }

        if (DropEffect != null)
        {
            GameObject blast = Instantiate(DropEffect, transform.position, Quaternion.identity);
            Destroy(blast, 0.5f);
        }

        if (_light != null)
        {
            _light.enabled = false;
        }

        _collider.enabled = false;
        _rigidbody.isKinematic = true;

        if (stick)
        {
            Invoke(nameof(ReturnToPool), 1.5f);
        }
        else
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        CancelInvoke();
        transform.SetParent(null);

        if (ArrowPool.Instance != null)
        {
            ArrowPool.Instance.Return(gameObject);
        }
    }

    private void DealDamageToEnemy(GameObject enemy, int dmg)
    {
        if (enemy.CompareTag("Dummy"))
        {
            DummyEnemy dummy = enemy.GetComponent<DummyEnemy>();
            if (dummy != null)
            {
                dummy.TakeDamage();
            }
        }
        else if (enemy.CompareTag("Ghost"))
        {
            GhostHP ghost = enemy.GetComponent<GhostHP>();
            if (ghost != null)
            {
                ghost.TakeDamage(dmg);
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
        else if (enemy.CompareTag("Caster"))
        {
            CasterSkeletonEnemy caster = enemy.GetComponent<CasterSkeletonEnemy>();
            if (caster != null)
            {
                caster.TakeDamage();
            }
        }
        else
        {
            EnemyHP hp = enemy.GetComponent<EnemyHP>();
            if (hp != null)
            {
                hp.TakeDamage(dmg);
            }
        }
    }
}