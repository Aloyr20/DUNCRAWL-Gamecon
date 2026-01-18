using UnityEngine;
using System.Collections;

public class SpellEffectReceiver : MonoBehaviour
{
    public enum SpellType { None, Ice, Fire, Poison }
    private SpellType currentEffect = SpellType.None;

    public float enemySpeed = 5f;
    private float originalSpeed;
    private Coroutine activeEffect;

    public float health = 200f;

    void Start()
    {
        originalSpeed = enemySpeed;
    }

    public void ApplyEffect(SpellType newEffect)
    {
        if (currentEffect != SpellType.None)
        {
            if (currentEffect == newEffect)
            {
                return;
            }

            StopCoroutine(activeEffect);
        }

        activeEffect = StartCoroutine(HandleEffect(newEffect));
    }

    IEnumerator HandleEffect(SpellType effect)
    {
        currentEffect = effect;

        if (effect == SpellType.Ice)
        {
            enemySpeed = originalSpeed * 0.5f;
            yield return new WaitForSeconds(5f);
            enemySpeed = originalSpeed;
            TakeDamage(50f);
        }
        else if (effect == SpellType.Fire)
        {
            for (int i = 0; i < 3; i++)
            {
                TakeDamage(20f);
                yield return new WaitForSeconds(2f);
            }
        }
        else if (effect == SpellType.Poison)
        {
            for (int i = 0; i < 15; i++)
            {
                TakeDamage(5f);
                yield return new WaitForSeconds(1f);
            }
        }

        currentEffect = SpellType.None;
    }

    void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
