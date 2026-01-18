using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    public Transform playerobject;
    public NavMeshAgent nav;
    public LayerMask isPlayer, isGround;

    public Transform _playerPosition;
    public Transform _mobPosition;

    // These variables are for enemy Patroling
    public Vector3 point;
    bool WPointS;
    public float WPointR;

    // These variables are for enemy states
    public float attackR, sightR;

    //Enemy attacking variables
    bool Attacked;
    public float betweenAttack;

    public float distanceFromPlayer;
    public float AttackRange;
    public float AttackCooldown;
    float LastAttack = 0;

    Vector3 PrevPos;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        playerobject = GameObject.Find("Player").transform;
        Patrol();
        PrevPos = transform.position;
    }
    private void Update()
    {
        distanceFromPlayer = Vector3.Distance(_playerPosition.position, _mobPosition.position);
        //Debug.Log(distanceFromPlayer);

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

        //attack
        if (distanceFromPlayer <= AttackRange && Time.time > LastAttack)
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("Attack");
            LastAttack = Time.time + AttackCooldown;
        }

        //moving/idle anim
        if ((transform.position - PrevPos).magnitude != 0)
        {    
            gameObject.GetComponentInChildren<Animator>().SetBool("Moving", true);
        }
        else
        {
            gameObject.GetComponentInChildren<Animator>().SetBool("Moving", false);
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
        nav.SetDestination(playerobject.position);
    }


}
