using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;

    [Header("AoE Settings")]
    public bool AoE = false;
    public float AoERadius = 3f;

    [Header("Spell Effect")]
    public SpellEffectReceiver.SpellType spellType;

    [Header("Effects")]
    public GameObject dropEffect;
    public GameObject burnDecal;

    private ParticleSystem particles;
    private Rigidbody rb;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        particles = GetComponent<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
        {
            return;
        }
        hasHit = true;

        if (AoE)
        {
            Collider[] hits = new Collider[20];
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, AoERadius, hits, LayerMask.GetMask("Enemy"));

            for (int i = 0; i < hitCount; i++)
            {
                ApplyToEnemy(hits[i].gameObject);
            }
        }
        else
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                ApplyToEnemy(collision.gameObject);
            }
        }

        if (particles != null)
        {
            particles.Stop();
        }

        if (dropEffect != null)
        {
            GameObject impact = Instantiate(dropEffect, transform.position, Quaternion.identity);
            Destroy(impact, 0.75f);
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        Light l = GetComponent<Light>();
        if (l != null)
        {
            l.enabled = false;
        }

        if (burnDecal != null)
        {
            GameObject spawnedDecal = Instantiate(burnDecal, null, true);
            spawnedDecal.transform.position = transform.position;
            float multOf90 = Mathf.Round((transform.rotation.eulerAngles.y - 90) / 90) * 90;
            spawnedDecal.transform.Rotate(new Vector3(0, multOf90, 0));
        }

        Destroy(gameObject, 1.5f);
    }

    private void ApplyToEnemy(GameObject enemy)
    {
        SpellEffectReceiver receiver = enemy.GetComponent<SpellEffectReceiver>();
        if (receiver != null)
        {
            receiver.ApplyEffect(spellType);
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
}