using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public Slider healthBar;

    public void TakeDamage()
    {
        healthBar.value -= 20;
        gameObject.GetComponentInChildren<ParticleSystem>().Play();
        Alive();
    }
    void Alive()
    {
        if (healthBar.value <= 0)
        {
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<EnemyAi>().enabled = false;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 5f);
        }
    }
}
