using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float turnSpeed = 1.0f;

    public Transform playerTransform;
    public NavMeshAgent nav;

    public Transform _playerPosition;
    public Transform _mobPosition;

    public Vector3 point;
    bool WPointS;
    public float WPointR;

    public float attackR, sightR;

    public bool hasDamaged;
    public bool isKicking;

    public float distanceFromPlayer;
    public float AttackRange;
    public float AttackCooldown;
    float LastAttack = 0;

    public int damage;

    Animator animator;
    PlayerHealth playerHealth;

    public float leniencyAngle;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        playerHealth = playerTransform.GetComponent<PlayerHealth>();
        sightR = 80f;
        LastAttack = Time.time + AttackCooldown;
        Patrol();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Die");
            animator.SetBool("Moving", false);
        }
    }

    private void Update()
    {
        distanceFromPlayer = Vector3.Distance(_playerPosition.position, _mobPosition.position);

        if (distanceFromPlayer <= sightR)
        {
            Vector3 direction = (_playerPosition.position - _mobPosition.position).normalized;
            RaycastHit hit;

            if (Physics.Raycast(_mobPosition.position, direction, out hit, sightR))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Chase();
                }
                else
                {
                    Patrol();
                }
            }
        }
        else
        {
            Patrol();
        }

        if (distanceFromPlayer <= AttackRange && Time.time > LastAttack && !isKicking)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        bool isMoving = nav.velocity.sqrMagnitude > 0.01f && !isKicking;
        animator.SetBool("Moving", isMoving);

        RotateSkeleton();
    }

    private void Patrol()
    {
        if (!WPointS)
        {
            LookForWalkPoint();
        }

        if (WPointS)
        {
            nav.SetDestination(point);
        }

        float sqrDist = (transform.position - point).sqrMagnitude;
        if (sqrDist < 1f)
        {
            WPointS = false;
        }
    }

    private void LookForWalkPoint()
    {
        float rX = Random.Range(-WPointR, WPointR);
        float rZ = Random.Range(-WPointR, WPointR);

        point = new Vector3(transform.position.x + rX, transform.position.y, transform.position.z + rZ);

        if (Physics.Raycast(point, Vector3.down, 2f, isGround))
        {
            WPointS = true;
        }
    }

    public LayerMask isGround;

    private void Chase()
    {
        if (isKicking)
        {
            return;
        }

        nav.SetDestination(playerTransform.position);
    }

    public void OnKickHit()
    {
        if (!hasDamaged && distanceFromPlayer <= AttackRange)
        {
            hasDamaged = true;
            playerHealth.TakeDamage(damage);
        }
    }

    public void ResetAttack()
    {
        isKicking = false;
        hasDamaged = false;
        LastAttack = Time.time + AttackCooldown;
    }

    public void StopMove()
    {
        isKicking = true;
    }

    public void StartMove()
    {
        isKicking = false;
    }

    public void RotateSkeleton()
    {
        Vector3 vectorToPlayer = (playerTransform.position - transform.position).normalized;
        float angleDiff = Vector3.SignedAngle(transform.forward, vectorToPlayer, Vector3.up);

        if (angleDiff > leniencyAngle)
        {
            transform.Rotate(0f, angleDiff - leniencyAngle, 0f);
        }
        else if (angleDiff < -leniencyAngle)
        {
            transform.Rotate(0f, angleDiff + leniencyAngle, 0f);
        }
        else
        {
            Quaternion targetRot = Quaternion.LookRotation(vectorToPlayer);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed);
        }
    }
}