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

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        int finalDamage = Mathf.RoundToInt(damage + 2 * (damage * charge));

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (AoE)
            {
                Collider[] hits = new Collider[20];
                int hitCount = Physics.OverlapSphereNonAlloc(
                    transform.position, AoEradius, hits, LayerMask.GetMask("Enemy"));

                for (int i = 0; i < hitCount; i++)
                {
                    EnemyHP enemyHP = hits[i].GetComponent<EnemyHP>();
                    if (enemyHP != null)
                    {
                        enemyHP.TakeDamage(finalDamage);
                        continue;
                    }
                    GhostHP ghostHP = hits[i].GetComponent<GhostHP>();
                    if (ghostHP != null)
                    {
                        ghostHP.TakeDamage(finalDamage);
                    }
                }
            }

            if (collision.gameObject.CompareTag("Dummy"))
            {
                collision.gameObject.GetComponent<DummyEnemy>().TakeDamage();
            }
            else if (collision.gameObject.CompareTag("Ghost"))
            {
                collision.gameObject.GetComponent<GhostHP>().TakeDamage(finalDamage);
            }
            else
            {
                collision.gameObject.GetComponent<EnemyHP>().TakeDamage(finalDamage);
            }

            if (stick)
            {
                transform.SetParent(collision.transform, true);
            }
        }

        if (particles != null) particles.Stop();
        if (DropEffect != null)
        {
            GameObject blast = Instantiate(DropEffect, transform.position, Quaternion.identity);
            Destroy(blast, 0.5f);
        }
        if (GetComponent<Light>() != null) GetComponent<Light>().enabled = false;

        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(gameObject, 1.5f);
    }
}