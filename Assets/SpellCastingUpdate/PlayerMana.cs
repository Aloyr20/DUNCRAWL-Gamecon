using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    public float maxMana = 100f;
    public float currentMana = 100f;
    public Slider manaBar;

    public float regenRate = 4f;
    public float maxTime = 0.1f;

    float t = 0f;
    float startValue;
    float targetValue;

    void Awake()
    {
        manaBar.maxValue = maxMana;
        manaBar.minValue = 0;
        manaBar.value = currentMana;
        startValue = currentMana;
        targetValue = currentMana;
    }

    void Update()
    {
        RegenerateMana();

        if (t < maxTime)
        {
            t += Time.deltaTime;
            float time = t / maxTime;
            time = 1f - Mathf.Pow(1f - time, 3f);
            manaBar.value = Mathf.Lerp(startValue, targetValue, time);
        }
    }

    void RegenerateMana()
    {
        float newMana = currentMana + regenRate * Time.deltaTime;
        newMana = Mathf.Clamp(newMana, 0, maxMana);

        if (newMana != currentMana)
        {
            startValue = manaBar.value;
            targetValue = newMana;
            t = 0f;
            currentMana = newMana;
        }
    }

    public bool TryUseMana(float amount)
    {
        if (currentMana < amount)
        {
            return false;
        }

        float newMana = currentMana - amount;
        startValue = manaBar.value;
        targetValue = Mathf.Clamp(newMana, 0, maxMana);
        t = 0f;
        currentMana = newMana;

        return true;
    }

    public void AddMaxMana(float amount)
    {
        maxMana += amount;
        currentMana = maxMana;
        manaBar.maxValue = maxMana;

        startValue = manaBar.value;
        targetValue = currentMana;
        t = 0f;
    }
}
