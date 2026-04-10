using UnityEngine;

public class CasterSkeletonEnemy : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float followRange = 12f;
    public float attackRange = 6f;
    public float loseRange = 16f;
    public float returnStopDistance = 0.2f;

    [Header("Spider Spawn")]
    public GameObject spiderPrefab;
    public Transform spiderSpawnParent;
    public float spiderSpawnInterval = 5f;

    [Header("Skull Shoot")]
    public GameObject skullPrefab;
    public Transform firePoint;
    public float skullShootInterval = 2f;

    private float spiderTimer;
    private float skullTimer;

    private Vector3 startPosition;
    private bool isReturning = false;

    public float HP = 100f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        spiderTimer = spiderSpawnInterval;
        skullTimer = skullShootInterval;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (player == null) return;

        if (HP <= 0)
        {
            Die();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > loseRange)
        {
            isReturning = true;
        }

        if (isReturning)
        {
            ReturnToStart();
            return;
        }

        if (distanceToPlayer <= followRange)
        {
            FacePlayer();

            if (distanceToPlayer > attackRange)
            {
                MoveToPlayer();
            }
            else
            {
                if (animator != null)
                    animator.SetBool("Moving", false);

                HandleSpiderSpawn();
                HandleSkullShoot();
            }
        }
        else
        {
            if (animator != null)
                animator.SetBool("Moving", false);
        }
    }

    private void MoveToPlayer()
    {
        if (animator != null)
            animator.SetBool("Moving", true);

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
    }

    private void ReturnToStart()
    {
        if (animator != null)
            animator.SetBool("Moving", true);

        Vector3 dir = startPosition - transform.position;
        dir.y = 0f;

        if (dir.magnitude <= returnStopDistance)
        {
            transform.position = new Vector3(startPosition.x, transform.position.y, startPosition.z);
            isReturning = false;

            if (animator != null)
                animator.SetBool("Moving", false);

            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);

        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleSpiderSpawn()
    {
        spiderTimer -= Time.deltaTime;

        if (spiderTimer <= 0f)
        {
            spiderTimer = spiderSpawnInterval;

            if (animator != null)
                animator.SetTrigger("Attack");

            if (spiderPrefab == null || spiderSpawnParent == null) return;

            foreach (Transform spawnPoint in spiderSpawnParent)
            {
                Instantiate(spiderPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }

    private void HandleSkullShoot()
    {
        skullTimer -= Time.deltaTime;

        if (skullTimer <= 0f)
        {
            skullTimer = skullShootInterval;

            if (animator != null)
                animator.SetTrigger("Attack");

            if (skullPrefab == null || firePoint == null) return;

            GameObject skull = Instantiate(skullPrefab, firePoint.position, Quaternion.identity);

            Skull projectile = skull.GetComponent<Skull>();
            if (projectile != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                projectile.SetDirection(dir);
            }
        }
    }

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
    }

    public void TakeDamage()
    {
        Debug.Log("Caster TakeDamage");
        HP -= 20;
        Invoke("HealthBarFill", 0.5f);
    }

    void Die()
    {
        if (animator != null)
            animator.SetBool("Moving", false);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Knife"))
        {
            Destroy(collision.gameObject);
            TakeDamage();
        }
    }
}