using UnityEngine;
using UnityEngine.UI;

public class NewSpellUIController : MonoBehaviour
{
    public enum SpellType { Ice, Fire, Poison }

    [Header("UI")]
    public Image baseGrey;   
    public Image iceFill;    
    public Image fireFill;
    public Image poisonFill;

    [Header("Cooldown Seconds")]
    public float iceCooldown = 3f;
    public float fireCooldown = 2f;
    public float poisonCooldown = 4f;

    [Header("Default Spell")]
    public SpellType defaultSpell = SpellType.Ice;

    private SpellType currentSpell;
    private bool isCoolingDown = false;

    private float cdTimer = 0f;
    private float cdDuration = 1f;

    void Start()
    {
        if (baseGrey != null) baseGrey.gameObject.SetActive(true);

        SetFillActive(iceFill, false);
        SetFillActive(fireFill, false);
        SetFillActive(poisonFill, false);

        SetSpell(defaultSpell);
    }

    void Update()
    {
        if (!isCoolingDown)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetSpell(SpellType.Ice);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetSpell(SpellType.Fire);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetSpell(SpellType.Poison);

            if (Input.GetMouseButtonDown(1))
            {
                StartCooldown();
            }
        }

        if (isCoolingDown)
        {
            cdTimer -= Time.deltaTime;
            if (cdTimer < 0f) cdTimer = 0f;

            Image active = GetActiveFill();
            if (active != null)
            {
                active.fillAmount = Mathf.Clamp01(cdTimer / cdDuration); // 1 -> 0
            }

            if (cdTimer <= 0f)
            {
                isCoolingDown = false;

                Image a = GetActiveFill();
                if (a != null) a.fillAmount = 1f;   
            }
        }
    }

    void SetSpell(SpellType spell)
    {
        currentSpell = spell;

        SetFillActive(iceFill, spell == SpellType.Ice);
        SetFillActive(fireFill, spell == SpellType.Fire);
        SetFillActive(poisonFill, spell == SpellType.Poison);

        Image active = GetActiveFill();
        if (active != null) active.fillAmount = 1f;
    }

    void StartCooldown()
    {
        cdDuration = GetCooldownSeconds(currentSpell);
        if (cdDuration <= 0f) return;

        cdTimer = cdDuration;
        isCoolingDown = true;

        Image active = GetActiveFill();
        if (active != null) active.fillAmount = 1f;
    }

    float GetCooldownSeconds(SpellType spell)
    {
        return spell switch
        {
            SpellType.Ice => iceCooldown,
            SpellType.Fire => fireCooldown,
            SpellType.Poison => poisonCooldown,
            _ => 1f
        };
    }

    Image GetActiveFill()
    {
        return currentSpell switch
        {
            SpellType.Ice => iceFill,
            SpellType.Fire => fireFill,
            SpellType.Poison => poisonFill,
            _ => null
        };
    }

    void SetFillActive(Image img, bool on)
    {
        if (img == null) return;
        img.gameObject.SetActive(on);
    }
}