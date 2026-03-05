using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [SerializeField, Range (0,1)] float turnSpeed = 1.0f;

    public Transform playerTransform;
    public GameObject player;
    public NavMeshAgent nav;
    public LayerMask isPlayer, isGround;

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
    public float lookAngleLeniance;
    float LastAttack = 0;

    public int damage;

    Vector3 PrevPos;
    public Vector3 lastVectorToPlayer;

    Animator animator;

    public float leniencyAngle;

    Camera cam;
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        playerTransform = GameObject.Find("Player").transform;
        Patrol();
        PrevPos = transform.position;
        cam = Camera.main;
    }
    private void Update()
    {
        RotateSkeleton();

        distanceFromPlayer = Vector3.Distance(_playerPosition.position, _mobPosition.position);
        Vector3 direction = (_playerPosition.position - _mobPosition.position);
        RaycastHit hit;

        if (Physics.Raycast(_mobPosition.position, direction.normalized, out hit, 80f))
        {

            if (hit.collider.CompareTag("Player"))
            {
                Chase();
            }
            else if ((hit.point - _mobPosition.position).normalized.magnitude < direction.magnitude)
            {
                Patrol();
            }
        }

        if (distanceFromPlayer <= AttackRange && Time.time > LastAttack && !isKicking)
        {
            animator.SetTrigger("Attack");
        }

        if (nav.velocity.magnitude > 0.1f && !isKicking)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }


        PrevPos = transform.position;

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

        Vector3 distancePoint = transform.position - point;

        if (distancePoint.magnitude < 1f)
        {
            WPointS = false;
        }
    }

    private void LookForWalkPoint()
    {
        float rX = Random.Range(-WPointR, WPointR);
        float rZ = Random.Range(-WPointR, WPointR);

        point = new Vector3(-transform.position.x + rX, transform.position.y, transform.position.z + rZ);

        if (Physics.Raycast(point, -transform.up, 2f, isGround))
        {
            WPointS = true;
        }

    }
    private void Chase()
    {
        if (isKicking)
        {
            return;
        }

        Vector3 dirToPlayer = playerTransform.position - transform.position;
        dirToPlayer.y = 0;

        nav.SetDestination(playerTransform.position);
    }

    public void OnKickHit()
    {
        if (!hasDamaged && distanceFromPlayer <= AttackRange)
        {
            hasDamaged = true;
            playerTransform.GetComponent<PlayerHealth>().TakeDamage(damage);
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

        float angleDiffCheck = Vector3.SignedAngle(transform.forward, vectorToPlayer, Vector3.up);

        //If the skeleton has to turn right to be at lenience
        if (Vector3.SignedAngle(transform.forward, vectorToPlayer, Vector3.up) > leniencyAngle)
        {
            //Rotate just enough to be at the lenience angle
            transform.Rotate(new Vector3(0, angleDiffCheck - leniencyAngle, 0));
        }
        //Else if the skeleton has to turn left to be at lenience
        else if (Vector3.SignedAngle(transform.forward, vectorToPlayer, Vector3.up) < -leniencyAngle)
        {
            //Rotate just enough to be at the lenience angle
            transform.Rotate(new Vector3(0, angleDiffCheck + leniencyAngle, 0));
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), turnSpeed);
        }


    }
}

