using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    string Weapon = "sword";

    public GameObject[] SwordObjs;
    public PlayerAttack attack;

    public GameObject[] BowObjs;
    public Projectile shoot;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Switch();
        }

        if (Weapon == "sword")
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
        }
        else
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
