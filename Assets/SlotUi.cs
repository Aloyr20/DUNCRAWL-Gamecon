using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image icon;
    public Image duoP1;
    public Image duoP2;
    public TextMeshProUGUI numberText;
    private Inventory inventory;
    private int slotIndex;
    private bool isActiveSlot = false;

    public void Setup(int index, Inventory inv)
    {
        slotIndex = index;
        inventory = inv;

        if (icon != null)
        {
            icon.gameObject.SetActive(false);
        }
        if (duoP1 != null)
        {
            duoP1.gameObject.SetActive(false);
        }
        if (duoP2 != null)
        {
            duoP2.gameObject.SetActive(false);
        }
        if (numberText != null)
        {
            numberText.gameObject.SetActive(false);
        }
    }

    public void SetupAsActiveSlot(Inventory inv)
    {
        slotIndex = -1;
        inventory = inv;
        isActiveSlot = true;

        if (icon != null)
        {
            icon.gameObject.SetActive(false);
        }
        if (duoP1 != null)
        {
            duoP1.gameObject.SetActive(false);
        }
        if (duoP2 != null)
        {
            duoP2.gameObject.SetActive(false);
        }
        if (numberText != null)
        {
            numberText.gameObject.SetActive(false);
        }
    }

    public void SetIcon(Sprite sprite, int numberOwn)
    {
        if (sprite == null || numberOwn <= 0)
        {
            if (icon != null)
            {
                icon.sprite = null;
                icon.gameObject.SetActive(false);
            }
            if (duoP1 != null)
            {
                duoP1.sprite = null;
                duoP1.gameObject.SetActive(false);
            }
            if (duoP2 != null)
            {
                duoP2.sprite = null;
                duoP2.gameObject.SetActive(false);
            }
            if (numberText != null)
            {
                numberText.gameObject.SetActive(false);
            }
            return;
        }

        if (numberOwn >= 2)
        {
            if (icon != null)
            {
                icon.gameObject.SetActive(false);
            }
            if (duoP1 != null)
            {
                duoP1.gameObject.SetActive(true);
                duoP1.sprite = sprite;
            }
            if (duoP2 != null)
            {
                duoP2.gameObject.SetActive(true);
                duoP2.sprite = sprite;
            }
        }
        else
        {
            if (icon != null)
            {
                icon.gameObject.SetActive(true);
                icon.sprite = sprite;
            }
            if (duoP1 != null)
            {
                duoP1.gameObject.SetActive(false);
            }
            if (duoP2 != null)
            {
                duoP2.gameObject.SetActive(false);
            }
        }

        if (numberText != null)
        {
            numberText.gameObject.SetActive(true);
            numberText.text = numberOwn.ToString();
        }
    }

    public Sprite GetCurrentSprite()
    {
        if (icon != null && icon.gameObject.activeSelf && icon.sprite != null)
        {
            return icon.sprite;
        }
        if (duoP1 != null && duoP1.gameObject.activeSelf && duoP1.sprite != null)
        {
            return duoP1.sprite;
        }
        return null;
    }

    public int GetSlotIndex()
    {
        return slotIndex;
    }

    public bool IsActiveSlot()
    {
        return isActiveSlot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Sprite sprite = GetCurrentSprite();
        if (sprite != null)
        {
            if (inventory != null)
            {
                if (isActiveSlot)
                {
                    inventory.BeginDragFromActiveSlot(sprite);
                }
                else
                {
                    inventory.BeginDrag(slotIndex, sprite);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventory != null)
        {
            inventory.UpdateDrag(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventory != null)
        {
            inventory.EndDrag();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (inventory != null)
        {
            if (isActiveSlot)
            {
                inventory.DropOnActiveSlot();
            }
            else
            {
                inventory.DropOnSlot(slotIndex);
            }
        }
    }
}