using UnityEngine;

public class SlotSpawn : MonoBehaviour
{
    public Transform parent;
    public GameObject slotPrefab;
    public int slotCount = 6;
    SlotUi[] slotUis;

    public Sprite[] icon;

    void Start()
    {
        slotUis = new SlotUi[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject a = Instantiate(slotPrefab, parent);
            if (a != null)
            {
                slotUis[i] = a.GetComponent<SlotUi>();
            }
        }
    }

    public void AddItem(Sprite itemIcon, int numberOwn)
    {
        if (itemIcon == null) return;

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (icon[i] == itemIcon)
            {
                if (slotUis[i] != null)
                {
                    slotUis[i].SetIcon(itemIcon, numberOwn);
                }
                return;
            }
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (icon[i] == null)
            {
                icon[i] = itemIcon;
                if (slotUis[i] != null)
                {
                    slotUis[i].SetIcon(itemIcon, numberOwn);
                }
                break;
            }
        }
    }
}