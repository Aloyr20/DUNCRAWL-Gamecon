using UnityEngine;

public class SpiderEnemy : MonoBehaviour
{
    [Header("Target")]
    private Transform player;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float followRange = 10f;
    public float stopDistance = 1.1f;
    public float turnSpeed = 8f;

    [Header("Attack")]
    public int damage = 1;
    public float damageCooldown = 1f;

    [Header("HP")]
    public float HP = 2;

    private float damageTimer;
    private bool isCollidingWithPlayer;
    private PlayerHealth currentPlayerHealth;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        FindPlayer();
    }

    private void Update()
    {
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }

        if (player == null)
        {
            FindPlayer();

            if (animator != null)
                animator.SetBool("Moving", false);

            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange)
        {
            FacePlayer();

            if (distanceToPlayer > stopDistance)
            {
                MoveToPlayer();
            }
            else
            {
                if (animator != null)
                    animator.SetBool("Moving", false);
            }
        }
        else
        {
            if (animator != null)
                animator.SetBool("Moving", false);
        }

        if (isCollidingWithPlayer && damageTimer <= 0f && currentPlayerHealth != null)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            currentPlayerHealth.TakeDamage(damage);
            damageTimer = damageCooldown;
        }

        if (HP <= 0)
        {
            Die();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
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

    private void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            currentPlayerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (damageTimer <= 0f && currentPlayerHealth != null)
            {
                if (animator != null)
                    animator.SetTrigger("Attack");

                currentPlayerHealth.TakeDamage(damage);
                damageTimer = damageCooldown;
            }
        }

        if (collision.gameObject.CompareTag("Knife"))
        {
            Destroy(collision.gameObject);
            TakeDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
            currentPlayerHealth = null;
        }
    }

    public void TakeDamage()
    {
        HP -= 30;
    }

    private void Die()
    {
        if (animator != null)
            animator.SetBool("Moving", false);

        Destroy(gameObject);
    }
}