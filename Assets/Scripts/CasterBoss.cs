using UnityEngine;
using UnityEngine.UI;

public class CasterBoss : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Boss Stats")]
    public int maxHP = 80;
    public int currentHP;

    [Header("Movement")]
    public float turnSpeed = 5f;

    public float keepDistance = 7f;
    public float distanceTolerance = 1.5f;

    public float backAwaySpeed = 2f;
    public float approachSpeed = 2.5f;

    public float repositionSpeed = 6f;
    public float repositionDistance = 6f;
    public float repositionStopDistance = 0.2f;

    [Header("Reposition")]
    public int hitsBeforeReposition = 3;
    public float repositionCooldown = 4f;
    public float randomSideOffset = 2f;

    [Header("Spider Spawn")]
    public GameObject spiderPrefab;
    public Transform spiderSpawnParent;
    public float spiderSpawnInterval = 2.5f;
    public float spiderScaleMultiplier = 1.5f;

    [Header("Skull Shoot")]
    public GameObject skullPrefab;
    public Transform firePoint;
    public float skullShootInterval = 0.8f;
    public float projectileScaleMultiplier = 1.5f;

    [Header("Boss UI")]
    public Image bossHealthBar;

    private float spiderTimer;
    private float skullTimer;
    private float repositionTimer;

    private int hitCounter = 0;
    private bool isRepositioning = false;
    private Vector3 repositionTarget;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        currentHP = maxHP;
        spiderTimer = spiderSpawnInterval;
        skullTimer = skullShootInterval;
        repositionTimer = 0f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.fillAmount = 1f;
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        if (repositionTimer > 0f)
        {
            repositionTimer -= Time.deltaTime;
        }

        FacePlayer();

        if (isRepositioning)
        {
            if (animator != null)
                animator.SetBool("Moving", true);

            MoveToRepositionTarget();
            return;
        }

        HandleCombatMovement();
        HandleSpiderSpawn();
        HandleSkullShoot();
    }

    private void HandleCombatMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        float minDistance = keepDistance - distanceTolerance;
        float maxDistance = keepDistance + distanceTolerance;

        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0f;

        if (dirToPlayer.sqrMagnitude < 0.001f) return;

        bool isMoving = false;

        if (distanceToPlayer < minDistance)
        {
            Vector3 awayDir = -dirToPlayer.normalized;
            transform.position += awayDir * backAwaySpeed * Time.deltaTime;
            isMoving = true;
        }
        else if (distanceToPlayer > maxDistance)
        {
            Vector3 forwardDir = dirToPlayer.normalized;
            transform.position += forwardDir * approachSpeed * Time.deltaTime;
            isMoving = true;
        }

        if (animator != null)
            animator.SetBool("Moving", isMoving);
    }

    private void MoveToRepositionTarget()
    {
        Vector3 dir = repositionTarget - transform.position;
        dir.y = 0f;

        if (dir.magnitude <= repositionStopDistance)
        {
            isRepositioning = false;

            if (animator != null)
                animator.SetBool("Moving", false);

            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

        transform.position += dir.normalized * repositionSpeed * Time.deltaTime;
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
                GameObject spider = Instantiate(spiderPrefab, spawnPoint.position, spawnPoint.rotation);
                spider.transform.localScale *= spiderScaleMultiplier;
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
            skull.transform.localScale *= projectileScaleMultiplier;

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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        hitCounter++;

        if (currentHP < 0)
        {
            currentHP = 0;
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.fillAmount = (float)currentHP / maxHP;
        }

        if (hitCounter >= hitsBeforeReposition && repositionTimer <= 0f)
        {
            StartReposition();
            hitCounter = 0;
            repositionTimer = repositionCooldown;
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void StartReposition()
    {
        if (player == null) return;

        Vector3 awayDir = (transform.position - player.position).normalized;
        awayDir.y = 0f;

        if (awayDir.sqrMagnitude < 0.001f)
        {
            awayDir = transform.forward;
        }

        Vector3 sideDir = Vector3.Cross(Vector3.up, awayDir).normalized;
        float randomSide = Random.Range(-randomSideOffset, randomSideOffset);

        Vector3 finalDir = (awayDir + sideDir * randomSide).normalized;
        repositionTarget = transform.position + finalDir * repositionDistance;
        repositionTarget.y = transform.position.y;

        isRepositioning = true;
    }

    private void Die()
    {
        if (animator != null)
            animator.SetBool("Moving", false);

        Destroy(gameObject);
    }
}