using UnityEngine;
using UnityEngine.UI;

public class SimpleSpellUI : MonoBehaviour
{
    public Image iceFill;
    public Image fireFill;
    public Image poisonFill;

    float timer = 0f;
    float cooldown = 3f;

    Image currentFill;

    void Start()
    {
        currentFill = iceFill;

        iceFill.fillAmount = 1;
        fireFill.fillAmount = 0;
        poisonFill.fillAmount = 0;
    }

    void Update()
    {
        // 切换技能
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchSpell(iceFill);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchSpell(fireFill);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchSpell(poisonFill);

        // 右键使用
        if (Input.GetMouseButtonDown(1))
        {
            timer = cooldown;
        }

        // 倒数
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            currentFill.fillAmount = timer / cooldown;
        }
    }

    void SwitchSpell(Image newFill)
    {
        if (timer > 0) return; // CD期间不能切换

        iceFill.fillAmount = 0;
        fireFill.fillAmount = 0;
        poisonFill.fillAmount = 0;

        currentFill = newFill;
        currentFill.fillAmount = 1;
    }
}