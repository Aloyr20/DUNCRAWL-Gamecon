using Unity.VisualScripting;
using UnityEngine;

public class SkullProjectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 10f;
    public float lifeTime = 3f;


    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}