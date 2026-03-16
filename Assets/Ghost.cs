using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum State { Patrol, Chase, Return }

    [Header("References")]
    public Transform player;                 
    public Transform firePoint;              
    public GameObject skullProjectilePrefab; 

    [Header("Detection")]
    public float detectionRange = 12f;       
    public LayerMask obstacleMask;           

    [Header("Movement")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 3.5f;
    public float returnSpeed = 3.0f;
    public float flyingBobAmplitude = 0.35f; 
    public float flyingBobFrequency = 1.8f;  

    [Header("Patrol (circle)")]
    public float patrolRadius = 3f;          
    public float patrolAngularSpeed = 1.2f;  

    [Header("Chase rules")]
    public float lostLoSChaseTime = 3.0f;    
    public float stopDistance = 1.2f;       

    [Header("Attack")]
    public float shootCooldown = 1.2f;      
    public float projectileSpeed = 10f;
    public int projectileDamage = 1;

    [Header("Debug")]
    public bool drawGizmos = true;

    private State state = State.Patrol;
    private Vector3 originPos;
    private float patrolAngle;
    private float shootTimer;
    private float lostTimer;                
    private float baseY;

    private void Awake()
    {
        originPos = transform.position;
        baseY = originPos.y;
        patrolAngle = Random.Range(0f, Mathf.PI * 2f);
        shootTimer = Random.Range(0f, shootCooldown);
    }

    private void Update()
    {
        if (player == null) return;

        float dt = Time.deltaTime;

        bool inRange = Vector3.Distance(transform.position, player.position) <= detectionRange;
        bool hasLoS = HasLineOfSightToPlayer();

        
        switch (state)
        {
            case State.Patrol:
                if (inRange && hasLoS)
                {
                    state = State.Chase;
                    lostTimer = 0f;
                }
                break;

            case State.Chase:
                if (hasLoS)
                {
                    lostTimer = 0f;
                }
                else
                {
                    lostTimer += dt;
                    if (lostTimer >= lostLoSChaseTime)
                    {
                        state = State.Return;
                    }
                }
                break;

            case State.Return:
                if (Vector3.Distance(transform.position, originPos) <= 0.2f)
                {
                    state = State.Patrol;
                }
                
                else if (inRange && hasLoS)
                {
                    state = State.Chase;
                    lostTimer = 0f;
                }
                break;
        }

        switch (state)
        {
            case State.Patrol:
                PatrolMove(dt);
                break;

            case State.Chase:
                ChaseMove(dt);

                //only can do this when in line of sight
                shootTimer -= dt;
                if (hasLoS && shootTimer <= 0f)
                {
                    //ShootSkull();
                    shootTimer = shootCooldown;
                }
                break;

            case State.Return:
                ReturnMove(dt);
                break;
        }

        ApplyFlyingBob(dt);
        FacePlayerIfChasing();
    }

    private bool HasLineOfSightToPlayer()
    {
        Vector3 start = (firePoint != null) ? firePoint.position : transform.position;
        Vector3 end = player.position;

        if (Physics.Linecast(start, end, out RaycastHit hit, obstacleMask))
        {
            return false;
        }
        return true;
    }

    private void PatrolMove(float dt)
    {
        patrolAngle += patrolAngularSpeed * dt;

        Vector3 target = originPos + new Vector3(
            Mathf.Cos(patrolAngle) * patrolRadius,
            0f,
            Mathf.Sin(patrolAngle) * patrolRadius
        );

        MoveTowards(target, patrolSpeed, dt);
    }

    private void ChaseMove(float dt)
    {
        Vector3 target = player.position;
        float dist = Vector3.Distance(transform.position, target);

        if (dist <= stopDistance) return;

        MoveTowards(target, chaseSpeed, dt);
    }

    private void ReturnMove(float dt)
    {
        MoveTowards(originPos, returnSpeed, dt);
    }

    private void MoveTowards(Vector3 target, float speed, float dt)
    {
        Vector3 pos = transform.position;

        target.y = baseY;

        Vector3 newPos = Vector3.MoveTowards(pos, target, speed * dt);
        transform.position = newPos;
    }

    private void ApplyFlyingBob(float dt)
    {
        Vector3 p = transform.position;
        float bob = Mathf.Sin(Time.time * flyingBobFrequency) * flyingBobAmplitude;
        p.y = baseY + bob;
        transform.position = p;
    }

    private void FacePlayerIfChasing()
    {
        if (state != State.Chase) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir.normalized),
            Time.deltaTime * 8f
        );
    }

   

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? originPos : transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? originPos : transform.position, patrolRadius);
    }
}