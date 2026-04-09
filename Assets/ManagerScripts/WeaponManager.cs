using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public string Weapon = "sword";
    public GameObject[] SwordObjs;
    public PlayerAttack attack;
    public GameObject[] BowObjs;
    public Projectile shoot;

    private bool wasSword = false;
    private bool wasBow = false;

    void Update()
    {
        if (Inventory.IsDragging)
        {
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Switch();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Inventory inventory = FindAnyObjectByType<Inventory>();
            if (inventory != null)
            {
                inventory.UseActiveSlotItem();
            }
        }

        if (Weapon == "sword" && !wasSword)
        {
            shoot.enabled = false;
            attack.enabled = true;
            foreach (GameObject obj in SwordObjs)
            {
                obj.SetActive(true);
            }
            foreach (GameObject obj in BowObjs)
            {
                obj.SetActive(false);
            }
            wasSword = true;
            wasBow = false;
        }
        else if (Weapon == "bow" && !wasBow)
        {
            shoot.enabled = true;
            attack.enabled = false;
            foreach (GameObject obj in SwordObjs)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in BowObjs)
            {
                obj.SetActive(true);
            }
            wasBow = true;
            wasSword = false;
        }
    }

    void Switch()
    {
        if (Weapon == "sword")
        {
            Weapon = "bow";
        }
        else
        {
            Weapon = "sword";
        }
    }
}