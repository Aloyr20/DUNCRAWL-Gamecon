using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase,
        SearchLastKnown,
        Return
    }

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
    public float searchSpeed = 3.2f;

    [Header("Flying")]
    public float flyingBobAmplitude = 0.35f;
    public float flyingBobFrequency = 1.8f;

    [Header("Patrol (circle)")]
    public float patrolRadius = 3f;
    public float patrolAngularSpeed = 1.2f;

    [Header("Chase rules")]
    public float stopDistance = 1.2f;
    public float searchPositionReachDistance = 0.4f;

    [Header("Attack")]
    public float shootCooldown = 1.2f;
    public float projectileSpeed = 10f;
    public int projectileDamage = 1;

    [Header("Debug")]
    public bool drawGizmos = true;

    private State state = State.Patrol;
    private Vector3 originPos;
    private Vector3 lastKnownPlayerPosition;
    private bool hasLastKnownPosition = false;

    private float patrolAngle;
    private float shootTimer;

    private void Awake()
    {
        originPos = transform.position;
        patrolAngle = Random.Range(0f, Mathf.PI * 2f);
        shootTimer = Random.Range(0f, shootCooldown);
        lastKnownPlayerPosition = originPos;
    }

    private void Update()
    {
        if (player == null) return;

        float dt = Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool inRange = distanceToPlayer <= detectionRange;
        bool hasLoS = false;

        // if player in rnage, check LOS
        if (inRange)
        {
            hasLoS = HasLineOfSightToPlayer();
        }

        // If could see player, record last position
        if (inRange && hasLoS)
        {
            lastKnownPlayerPosition = player.position;
            hasLastKnownPosition = true;
        }


        switch (state)
        {
            case State.Patrol:
                if (inRange && hasLoS)
                {
                    state = State.Chase;
                }
                break;

            case State.Chase:

                if (!inRange || !hasLoS)
                {
                    if (hasLastKnownPosition)
                    {
                        state = State.SearchLastKnown;
                    }
                    else
                    {
                        state = State.Return;
                    }
                }
                break;

            case State.SearchLastKnown:
                // if seeing the player
                if (inRange && hasLoS)
                {
                    state = State.Chase;
                }
                else
                {
                    // Back to last position seen the player
                    if (Vector3.Distance(transform.position, lastKnownPlayerPosition) <= searchPositionReachDistance)
                    {
                        state = State.Return;
                    }
                }
                break;

            case State.Return:
                if (inRange && hasLoS)
                {
                    state = State.Chase;
                }
                else if (Vector3.Distance(transform.position, originPos) <= 0.2f)
                {
                    state = State.Patrol;
                    hasLastKnownPosition = false;
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

                shootTimer -= dt;
                if (inRange && hasLoS && shootTimer <= 0f)
                {
                    //ShootSkull();
                    shootTimer = shootCooldown;
                }
                break;

            case State.SearchLastKnown:
                SearchLastKnownMove(dt);
                break;

            case State.Return:
                ReturnMove(dt);
                break;
        }

        ApplyFlyingBob();
        FaceMovementDirection();
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

        // keep Y value
        target.y = originPos.y;

        MoveTowards3D(target, patrolSpeed, dt, false);
    }

    private void ChaseMove(float dt)
    {
        Vector3 target = player.position;
        float dist = Vector3.Distance(transform.position, target);

        if (dist <= stopDistance) return;


        MoveTowards3D(target, chaseSpeed, dt, true);
    }

    private void SearchLastKnownMove(float dt)
    {
        MoveTowards3D(lastKnownPlayerPosition, searchSpeed, dt, true);
    }

    private void ReturnMove(float dt)
    {

        MoveTowards3D(originPos, returnSpeed, dt, true);
    }

    private void MoveTowards3D(Vector3 target, float speed, float dt, bool allowVerticalMovement)
    {
        Vector3 pos = transform.position;

        if (!allowVerticalMovement)
        {
            target.y = originPos.y;
        }

        Vector3 newPos = Vector3.MoveTowards(pos, target, speed * dt);
        transform.position = newPos;
    }

    private void ApplyFlyingBob()
    {

        Vector3 p = transform.position;
        p.y += Mathf.Sin(Time.time * flyingBobFrequency) * flyingBobAmplitude * Time.deltaTime;
        transform.position = p;
    }

    private void FaceMovementDirection()
    {
        Vector3 dir = Vector3.zero;

        switch (state)
        {
            case State.Chase:
                dir = player.position - transform.position;
                break;

            case State.SearchLastKnown:
                dir = lastKnownPlayerPosition - transform.position;
                break;

            case State.Return:
                dir = originPos - transform.position;
                break;
        }

        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
    }



    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Vector3 center = Application.isPlaying ? originPos : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, patrolRadius);

        if (Application.isPlaying && hasLastKnownPosition)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastKnownPlayerPosition, 0.4f);
            Gizmos.DrawLine(transform.position, lastKnownPlayerPosition);
        }
    }
}