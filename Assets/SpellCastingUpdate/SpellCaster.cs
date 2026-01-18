using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [Header("GameObjectReferences")]
    public ProjectileSpells projectileLauncher;
    public PlayerMana mana;
    public SpellUIController ui;

    [Header("Prefabs of the Spells")]
    public GameObject[] spellPrefabs;

    [Header("Mana Per Spell Costs")]
    public int iceMana = 25;
    public int fireMana = 30;
    public int poisonMana = 45;

    [Header("Cooldowns")]
    public float iceCooldown = 5f;
    public float fireCooldown = 10f;
    public float poisonCooldown = 15f;
    public float swapCooldown = 1f;

    private float lastIceTime = -999f;
    private float lastFireTime = -999f;
    private float lastPoisonTime = -999f;
    private float lastSwapTime = -999f;

    private float currentCooldownLeft = 0f;
    private bool cooldownReadyPrinted = false;

    public enum SpellType { None, Ice, Fire, Poison }
    public SpellType currentSpell = SpellType.None;

    void Start()
    {
        lastSwapTime = -swapCooldown;
        lastIceTime = -iceCooldown;
        lastFireTime = -fireCooldown;
        lastPoisonTime = -poisonCooldown;
    }

    void Update()
    {
        HandleSpellSwitch();
        HandleCooldownDisplay();
        HandleCasting();
    }

    void HandleSpellSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentSpell = SpellType.None;
            ui.ShowSpell("None");
            StartSwap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentSpell = SpellType.Ice;
            ui.ShowSpell("Ice");
            StartSwap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentSpell = SpellType.Fire;
            ui.ShowSpell("Fire");
            StartSwap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentSpell = SpellType.Poison;
            ui.ShowSpell("Poison");
            StartSwap();
        }
    }

    void StartSwap()
    {
        lastSwapTime = Time.time;
        ui.StartSwapCooldown(swapCooldown);
    }

    void HandleCooldownDisplay()
    {
        if (currentSpell == SpellType.None) return;

        float cooldown = 0f;
        float lastTime = 0f;

        switch (currentSpell)
        {
            case SpellType.Ice: cooldown = iceCooldown; lastTime = lastIceTime; break;
            case SpellType.Fire: cooldown = fireCooldown; lastTime = lastFireTime; break;
            case SpellType.Poison: cooldown = poisonCooldown; lastTime = lastPoisonTime; break;
        }

        currentCooldownLeft = cooldown - (Time.time - lastTime);

        if (currentCooldownLeft > 0f)
        {
            cooldownReadyPrinted = false;
        }
        else
        {
            if (!cooldownReadyPrinted)
            {
                cooldownReadyPrinted = true;
            }
        }
    }

    void HandleCasting()
    {
        if (currentSpell == SpellType.None)
        {
            return;
        }
        if (!Input.GetMouseButton(1))
        {
            return;
        }
        if (Time.time - lastSwapTime <= swapCooldown)
        {
            return;
        }

        int spellIndex = (int)currentSpell - 1;
        GameObject prefabToFire = spellPrefabs[spellIndex];
        int manaCost = 0;
        float cooldown = 0f;
        float lastTime = 0f;

        switch (currentSpell)
        {
            case SpellType.Ice:
                manaCost = iceMana;
                cooldown = iceCooldown;
                lastTime = lastIceTime;
                break;
            case SpellType.Fire:
                manaCost = fireMana;
                cooldown = fireCooldown;
                lastTime = lastFireTime;
                break;
            case SpellType.Poison:
                manaCost = poisonMana;
                cooldown = poisonCooldown;
                lastTime = lastPoisonTime;
                break;
        }

        float timeSince = Time.time - lastTime;
        float cdLeft = cooldown - timeSince;

        if (cdLeft > 0f)
        {
            return;
        }

        if (!mana.TryUseMana(manaCost))
        {
            return;
        }

        projectileLauncher.Fire(prefabToFire, projectileLauncher._speed);

        ui.StartSpellCooldown(cooldown);

        switch (currentSpell)
        {
            case SpellType.Ice: lastIceTime = Time.time; break;
            case SpellType.Fire: lastFireTime = Time.time; break;
            case SpellType.Poison: lastPoisonTime = Time.time; break;
        }
    }
}