using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public GameObject _inventory;
    public TurnScript _turnScript;
    public StoreTrigger _store;
    public DialogueManager _dialogueManager;
    public SlotSpawn _slotSpawn;
    public Canvas _canvas;

    [Header("Drag Settings")]
    public float dragIconScale = 0.6f;

    [Header("Active Item Slot")]
    public SlotUi activeSlotUi;
    public float activeSlotNormalScale = 0.7f;
    public float activeSlotInventoryScale = 1.2f;

    public static bool IsDragging = false;

    private Dictionary<string, int> _itemCounts = new Dictionary<string, int>();
    private Dictionary<string, Sprite> _itemIcons = new Dictionary<string, Sprite>();
    private Dictionary<Sprite, string> _iconToName = new Dictionary<Sprite, string>();

    private bool _inventoryOpen = false;

    private GameObject _dragIconObject;
    private Image _dragIconImage;
    private int _dragFromSlot = -1;
    private Sprite _dragSprite;
    private bool _dragFromActive = false;

    private string _activeItemName = null;
    private int _activeItemCount = 0;

    private void Start()
    {
        if (_store == null)
        {
            _store = FindAnyObjectByType<StoreTrigger>();
        }

        if (_dialogueManager == null)
        {
            _dialogueManager = FindAnyObjectByType<DialogueManager>();
        }

        if (_slotSpawn == null)
        {
            _slotSpawn = FindAnyObjectByType<SlotSpawn>();
        }

        if (_canvas == null)
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null)
            {
                _canvas = FindAnyObjectByType<Canvas>();
            }
        }

        if (_slotSpawn != null)
        {
            _slotSpawn.Initialize(this);
        }

        if (activeSlotUi != null)
        {
            activeSlotUi.SetupAsActiveSlot(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool storeBlocking = _store != null && _store._storeOpen;
            bool dialogueBlocking = _dialogueManager != null && _dialogueManager.dialogueActive;

            if (!storeBlocking && !dialogueBlocking)
            {
                ToggleInventory();
            }
        }

        if (_inventoryOpen)
        {
            if (_dialogueManager != null && _dialogueManager.dialogueActive)
            {
                CloseInventory();
            }
        }
    }

    private void ToggleInventory()
    {
        _inventoryOpen = !_inventoryOpen;

        if (_inventory != null)
        {
            _inventory.SetActive(_inventoryOpen);
        }

        if (_turnScript != null)
        {
            _turnScript.enabled = !_inventoryOpen;
        }

        if (activeSlotUi != null)
        {
            float scale = _inventoryOpen ? activeSlotInventoryScale : activeSlotNormalScale;
            activeSlotUi.transform.localScale = Vector3.one * scale;
        }

        if (_inventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void CloseInventory()
    {
        _inventoryOpen = false;

        if (_inventory != null)
        {
            _inventory.SetActive(false);
        }

        if (_turnScript != null)
        {
            _turnScript.enabled = true;
        }

        if (activeSlotUi != null)
        {
            activeSlotUi.transform.localScale = Vector3.one * activeSlotNormalScale;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsInventoryOpen()
    {
        return _inventoryOpen;
    }

    private void RefreshActiveSlot()
    {
        if (activeSlotUi == null)
        {
            return;
        }

        if (_activeItemName == null || _activeItemCount <= 0)
        {
            _activeItemName = null;
            _activeItemCount = 0;
            activeSlotUi.SetIcon(null, 0);
            return;
        }

        Sprite sprite = _itemIcons.ContainsKey(_activeItemName) ? _itemIcons[_activeItemName] : null;
        activeSlotUi.SetIcon(sprite, _activeItemCount);
    }

    public void AddItem(ItemPickup item)
    {
        if (item == null)
        {
            return;
        }

        if (_itemCounts.ContainsKey(item.itemName))
        {
            _itemCounts[item.itemName]++;
        }
        else
        {
            _itemCounts[item.itemName] = 1;
            _itemIcons[item.itemName] = item.itemIcon;
            _iconToName[item.itemIcon] = item.itemName;
        }

        if (_slotSpawn != null)
        {
            _slotSpawn.AddItem(item.itemIcon, _itemCounts[item.itemName]);
        }

        if (_activeItemName == item.itemName)
        {
            RefreshActiveSlot();
        }
    }

    public void ConsumeItem(string type)
    {
        if (!_itemCounts.ContainsKey(type) || _itemCounts[type] <= 0)
        {
            return;
        }

        _itemCounts[type]--;

        if (_itemCounts[type] <= 0)
        {
            _itemCounts.Remove(type);
        }

        if (_slotSpawn != null && _itemIcons.ContainsKey(type))
        {
            int remaining = _itemCounts.ContainsKey(type) ? _itemCounts[type] : 0;
            _slotSpawn.UpdateItem(_itemIcons[type], remaining);
        }

        if (type == "Health")
        {
            PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.GiveHP(50);
            }
        }

        if (type == "Speed")
        {
            PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                StopCoroutine("SpeedBoost");
                StartCoroutine(playerMovement.SpeedBoost(5f, 5f));
            }
        }
    }

    public int GetItemCount(string type)
    {
        if (_itemCounts.ContainsKey(type))
        {
            return _itemCounts[type];
        }
        return 0;
    }

    public void UseActiveSlotItem()
    {
        if (_activeItemName == null || _activeItemCount <= 0)
        {
            return;
        }

        _activeItemCount--;
        ConsumeItem(_activeItemName);

        if (_activeItemCount <= 0)
        {
            _activeItemName = null;
            _activeItemCount = 0;
        }

        RefreshActiveSlot();
    }

    public void BeginDrag(int slotIndex, Sprite sprite)
    {
        IsDragging = true;
        _dragFromSlot = slotIndex;
        _dragSprite = sprite;
        _dragFromActive = false;

        CreateDragIcon(sprite);
    }

    public void BeginDragFromActiveSlot(Sprite sprite)
    {
        IsDragging = true;
        _dragFromSlot = -1;
        _dragSprite = sprite;
        _dragFromActive = true;

        CreateDragIcon(sprite);
    }

    private void CreateDragIcon(Sprite sprite)
    {
        _dragIconObject = new GameObject("DragIcon");
        _dragIconObject.transform.SetParent(_canvas.transform, false);
        _dragIconObject.transform.SetAsLastSibling();

        _dragIconImage = _dragIconObject.AddComponent<Image>();
        _dragIconImage.sprite = sprite;
        _dragIconImage.raycastTarget = false;
        _dragIconImage.preserveAspect = true;

        RectTransform rect = _dragIconObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(80f, 80f);
        rect.localScale = Vector3.one * dragIconScale;
    }

    public void UpdateDrag(Vector2 position)
    {
        if (_dragIconObject != null)
        {
            _dragIconObject.transform.position = position;
        }
    }

    public void EndDrag()
    {
        IsDragging = false;

        if (_dragIconObject != null)
        {
            Destroy(_dragIconObject);
            _dragIconObject = null;
            _dragIconImage = null;
        }

        _dragFromSlot = -1;
        _dragSprite = null;
        _dragFromActive = false;
    }

    public void DropOnSlot(int targetSlot)
    {
        if (_dragSprite == null)
        {
            return;
        }

        if (_dragFromActive)
        {
            if (_activeItemName == null || _activeItemCount <= 0)
            {
                return;
            }

            Sprite[] icons = _slotSpawn.GetSlotIcons();
            SlotUi[] slots = _slotSpawn.GetSlots();
            Sprite targetIcon = icons[targetSlot];

            if (targetIcon != null)
            {
                return;
            }

            string name = _activeItemName;
            int count = _activeItemCount;
            Sprite sprite = _itemIcons.ContainsKey(name) ? _itemIcons[name] : null;

            _slotSpawn.SetSlotIcon(targetSlot, sprite);
            slots[targetSlot].SetIcon(sprite, count);

            if (!_itemCounts.ContainsKey(name))
            {
                _itemCounts[name] = count;
            }
            else
            {
                _itemCounts[name] += count;
            }

            _activeItemName = null;
            _activeItemCount = 0;
            RefreshActiveSlot();

            return;
        }

        if (_dragFromSlot < 0 || _dragFromSlot == targetSlot)
        {
            return;
        }

        if (_slotSpawn == null)
        {
            return;
        }

        SlotUi[] slotsArr = _slotSpawn.GetSlots();
        Sprite[] iconsArr = _slotSpawn.GetSlotIcons();

        Sprite fromIcon = iconsArr[_dragFromSlot];
        Sprite toIcon = iconsArr[targetSlot];

        int fromCount = 0;
        int toCount = 0;

        if (fromIcon != null && _iconToName.ContainsKey(fromIcon))
        {
            string fromName = _iconToName[fromIcon];
            fromCount = _itemCounts.ContainsKey(fromName) ? _itemCounts[fromName] : 0;
        }

        if (toIcon != null && _iconToName.ContainsKey(toIcon))
        {
            string toName = _iconToName[toIcon];
            toCount = _itemCounts.ContainsKey(toName) ? _itemCounts[toName] : 0;
        }

        _slotSpawn.SetSlotIcon(_dragFromSlot, toIcon);
        _slotSpawn.SetSlotIcon(targetSlot, fromIcon);

        slotsArr[_dragFromSlot].SetIcon(toIcon, toCount);
        slotsArr[targetSlot].SetIcon(fromIcon, fromCount);
    }

    public void DropOnActiveSlot()
    {
        if (_dragSprite == null || _dragFromActive)
        {
            return;
        }

        if (_dragFromSlot < 0)
        {
            return;
        }

        Sprite[] icons = _slotSpawn.GetSlotIcons();
        SlotUi[] slots = _slotSpawn.GetSlots();

        Sprite fromIcon = icons[_dragFromSlot];
        if (fromIcon == null)
        {
            return;
        }

        string fromName = null;
        if (_iconToName.ContainsKey(fromIcon))
        {
            fromName = _iconToName[fromIcon];
        }

        if (fromName == null)
        {
            return;
        }

        int totalCount = _itemCounts.ContainsKey(fromName) ? _itemCounts[fromName] : 0;
        if (totalCount <= 0)
        {
            return;
        }

        if (_activeItemName != null && _activeItemName != fromName)
        {
            return;
        }

        _activeItemName = fromName;
        _activeItemCount = totalCount;

        _itemCounts.Remove(fromName);

        _slotSpawn.SetSlotIcon(_dragFromSlot, null);
        slots[_dragFromSlot].SetIcon(null, 0);

        RefreshActiveSlot();
    }
}