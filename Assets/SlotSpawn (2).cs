using UnityEngine;

public class SlotSpawn : MonoBehaviour
{
    public Transform parent;
    public GameObject slotPrefab;
    public int slotCount = 12;
    private SlotUi[] slotUis;
    private Sprite[] slotIcons;

    public SlotUi[] GetSlots()
    {
        return slotUis;
    }

    public Sprite[] GetSlotIcons()
    {
        return slotIcons;
    }

    public void SetSlotIcon(int index, Sprite icon)
    {
        if (index >= 0 && index < slotIcons.Length)
        {
            slotIcons[index] = icon;
        }
    }

    public void Initialize(Inventory inventory)
    {
        slotUis = new SlotUi[slotCount];
        slotIcons = new Sprite[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject a = Instantiate(slotPrefab, parent);
            if (a != null)
            {
                slotUis[i] = a.GetComponent<SlotUi>();
                slotUis[i].Setup(i, inventory);
            }
        }
    }

    public void AddItem(Sprite itemIcon, int numberOwn)
    {
        if (itemIcon == null)
        {
            return;
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (slotIcons[i] == itemIcon)
            {
                slotUis[i].SetIcon(itemIcon, numberOwn);
                return;
            }
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (slotIcons[i] == null)
            {
                slotIcons[i] = itemIcon;
                slotUis[i].SetIcon(itemIcon, numberOwn);
                return;
            }
        }
    }

    public void UpdateItem(Sprite itemIcon, int numberOwn)
    {
        if (itemIcon == null)
        {
            return;
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (slotIcons[i] == itemIcon)
            {
                if (numberOwn <= 0)
                {
                    slotIcons[i] = null;
                    slotUis[i].SetIcon(null, 0);
                }
                else
                {
                    slotUis[i].SetIcon(itemIcon, numberOwn);
                }
                return;
            }
        }
    }

    public void RemoveItem(Sprite itemIcon)
    {
        if (itemIcon == null)
        {
            return;
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (slotIcons[i] == itemIcon)
            {
                slotIcons[i] = null;
                slotUis[i].SetIcon(null, 0);
                return;
            }
        }
    }
}