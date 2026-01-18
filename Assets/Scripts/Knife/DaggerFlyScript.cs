using UnityEngine;

public class DaggerFlyScript : MonoBehaviour
{
    public float flySpeed;
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.up * flySpeed);
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        
        Stop();
    }

    void Stop()
    {
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        gameObject.GetComponent<Collider>().enabled = false;
    }
}
