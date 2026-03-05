using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum State { Patrol, Chase, Return }

    public State state;

    public Transform player;                 
    public Transform firePoint;              
    public GameObject skullProjectilePrefab;

    public float detectionRange = 12f;      
    public LayerMask obstacleMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
