using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;

    [Header("AoE Settings")]
    public bool AoE = false;
    public float AoERadius = 3f;

    [Header("Spell Effect Receiver")]
    public SpellEffectReceiver.SpellType spellType;

    [Header("Effects")]
    public GameObject dropEffect;
    private ParticleSystem particles;

    private Rigidbody rb;
    private bool hasHit = false;

    public GameObject burnDecal;

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
            Collider[] hits = Physics.OverlapSphere(transform.position, AoERadius, LayerMask.GetMask("Enemy"));
            foreach (Collider col in hits)
            {
                EnemyHP hp = col.GetComponent<EnemyHP>();
                if (hp != null)
                {
                    hp.ApplySpellEffect((EnemyHP.SpellType)spellType);
                }
            }
        }
        else
        {
            EnemyHP hp = collision.collider.GetComponent<EnemyHP>();
            if (hp != null)
            {
                hp.ApplySpellEffect((EnemyHP.SpellType)spellType);
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

        Collider col2 = GetComponent<Collider>();
        if (col2 != null)
        {
            col2.enabled = false;
        }

        Light l = GetComponent<Light>();
        if (l != null)
        {
            l.enabled = false;
        }

        if (burnDecal != null)
        {
            Vector3 hitPosition = GetComponent<Transform>().position;
            Quaternion hitRotation = GetComponent<Transform>().rotation;
            GameObject spawnedDecal = Instantiate(burnDecal, null, true);
            spawnedDecal.transform.position = hitPosition;
            float multOf90 = Mathf.Round((hitRotation.eulerAngles.y - 90) / 90) * 90;
            spawnedDecal.transform.Rotate(new Vector3(0, multOf90, 0));
        }

        Destroy(gameObject, 1.5f);
    }

}
