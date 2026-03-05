using UnityEngine;
using UnityEngine.UI;

public class SpellUIController : MonoBehaviour
{
    public Image spellIcon;
    public Image iceCooldownFill;
    public Image fireCooldownFill;
    public Image poisonCooldownFill;

    public Sprite iceSprite;
    public Sprite fireSprite;
    public Sprite poisonSprite;

    private float iceSpellCooldown;
    private float iceSpellTimer;
    private float fireSpellCooldown;
    private float fireSpellTimer;
    private float poisonSpellCooldown;
    private float poisonSpellTimer;

    private float currentSwapCooldown;
    private float swapCooldownTimer;
    private Image activeCooldownFill;
    private bool isInSwapCooldown = false;

    void Start()
    {
        iceCooldownFill.fillAmount = 1f;
        fireCooldownFill.fillAmount = 1f;
        poisonCooldownFill.fillAmount = 1f;

        iceCooldownFill.gameObject.SetActive(false);
        fireCooldownFill.gameObject.SetActive(false);
        poisonCooldownFill.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isInSwapCooldown && activeCooldownFill != null)
        {
            swapCooldownTimer -= Time.deltaTime;
            activeCooldownFill.fillAmount = 1f - (swapCooldownTimer / currentSwapCooldown);

            if (swapCooldownTimer <= 0f)
            {
                isInSwapCooldown = false;
                activeCooldownFill.fillAmount = 1f;
            }
        }
        else
        {
            if (iceSpellTimer > 0)
            {
                iceSpellTimer -= Time.deltaTime;
                if (activeCooldownFill == iceCooldownFill)
                {
                    iceCooldownFill.fillAmount = 1f - (iceSpellTimer / iceSpellCooldown);
                }
            }

            if (fireSpellTimer > 0)
            {
                fireSpellTimer -= Time.deltaTime;
                if (activeCooldownFill == fireCooldownFill)
                {
                    fireCooldownFill.fillAmount = 1f - (fireSpellTimer / fireSpellCooldown);
                }
            }

            if (poisonSpellTimer > 0)
            {
                poisonSpellTimer -= Time.deltaTime;
                if (activeCooldownFill == poisonCooldownFill)
                {
                    poisonCooldownFill.fillAmount = 1f - (poisonSpellTimer / poisonSpellCooldown);
                }
            }
        }
    }

    void UpdateSpellCooldownDisplay()
    {
        if (activeCooldownFill == iceCooldownFill)
        {
            if (iceSpellTimer > 0)
            {
                iceCooldownFill.fillAmount = 1f - (iceSpellTimer / iceSpellCooldown);
            }
            else
            {
                iceCooldownFill.fillAmount = 1f;
            }
        }
        else if (activeCooldownFill == fireCooldownFill)
        {
            if (fireSpellTimer > 0)
            {
                fireCooldownFill.fillAmount = 1f - (fireSpellTimer / fireSpellCooldown);
            }
            else
            {
                fireCooldownFill.fillAmount = 1f;
            }
        }
        else if (activeCooldownFill == poisonCooldownFill)
        {
            if (poisonSpellTimer > 0)
            {
                poisonCooldownFill.fillAmount = 1f - (poisonSpellTimer / poisonSpellCooldown);
            }
            else
            {
                poisonCooldownFill.fillAmount = 1f;
            }
        }
    }

    public void ShowSpell(string spellName)
    {
        iceCooldownFill.gameObject.SetActive(false);
        fireCooldownFill.gameObject.SetActive(false);
        poisonCooldownFill.gameObject.SetActive(false);

        switch (spellName)
        {
            case "Ice":
                spellIcon.sprite = iceSprite;
                activeCooldownFill = iceCooldownFill;
                iceCooldownFill.gameObject.SetActive(true);
                break;
            case "Fire":
                spellIcon.sprite = fireSprite;
                activeCooldownFill = fireCooldownFill;
                fireCooldownFill.gameObject.SetActive(true);
                break;
            case "Poison":
                spellIcon.sprite = poisonSprite;
                activeCooldownFill = poisonCooldownFill;
                poisonCooldownFill.gameObject.SetActive(true);
                break;
            case "None":
                spellIcon.sprite = null;
                activeCooldownFill = null;
                break;
        }

        UpdateSpellCooldownDisplay();
    }

    public void StartSwapCooldown(float cooldown)
    {
        if (activeCooldownFill == null)
        {
            return;
        }

        currentSwapCooldown = cooldown;
        swapCooldownTimer = cooldown;
        activeCooldownFill.fillAmount = 0f;
        isInSwapCooldown = true;
    }

    public void StartSpellCooldown(float cooldown)
    {
        if (activeCooldownFill == null)
        {
            return;
        }

        if (activeCooldownFill == iceCooldownFill)
        {
            iceSpellCooldown = cooldown;
            iceSpellTimer = cooldown;
        }
        else if (activeCooldownFill == fireCooldownFill)
        {
            fireSpellCooldown = cooldown;
            fireSpellTimer = cooldown;
        }
        else if (activeCooldownFill == poisonCooldownFill)
        {
            poisonSpellCooldown = cooldown;
            poisonSpellTimer = cooldown;
        }

        isInSwapCooldown = false;
        UpdateSpellCooldownDisplay();
    }
}