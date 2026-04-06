using UnityEngine;
using UnityEngine.UI;

public class DummyEnemy : MonoBehaviour
{
    public Slider healthBar;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Knife"))
        {
            Destroy(collision.gameObject);
            TakeDamage();
        }
    }

    void HealthBarFill()
    {
        healthBar.value += 20;
    }

    public void TakeDamage()
    {
        healthBar.value -= 20;
        gameObject.GetComponentInChildren<ParticleSystem>().Play();
        Invoke("HealthBarFill", 0.5f);
    }


}
