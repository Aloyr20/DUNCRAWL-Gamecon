using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public GameObject _inventory;
    public StoreTrigger _store;
    public DialogueManager _dialogueManager;

    public List<TextMeshProUGUI> _numOwn;
    public List<Image> _itemSlots;
    private List<ItemPickup> _items = new List<ItemPickup>();

    private bool _inventoryOpen = false;
    private ItemPickup _draggingItem = null;
    private Image _draggingIcon = null;
    private int _selectedSlot = 0;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_store != null)
            {
                if (!_store._storeOpen)
                {
                    if (_dialogueManager != null)
                    {
                        if (!_dialogueManager.dialogueActive)
                        {
                            ToggleInventory();
                        }
                    }
                }
            }
        }

        if (_inventoryOpen)
        {
            if (_dialogueManager != null)
            {
                if (_dialogueManager.dialogueActive)
                {
                    CloseInventory();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_items.Count > 0)
                {
                    if (_selectedSlot < _items.Count)
                    {
                        ConsumePotion(_items[_selectedSlot].itemName);
                    }
                }
            }
        }

        HandleDragging();
    }

    private void ToggleInventory()
    {
        _inventoryOpen = !_inventoryOpen;

        if (_inventory != null)
        {
            _inventory.SetActive(_inventoryOpen);
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsInventoryOpen()
    {
        return _inventoryOpen;
    }

    public void AddItem(ItemPickup item)
    {
        if (item != null)
        {
            _items.Add(item);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            if (i < _items.Count)
            {
                if (_items[i].itemIcon != null)
                {
                    
                    _itemSlots[i].enabled = true;
                }
            }
            else
            {

                _itemSlots[i].enabled = false;
            }
        }
    }

    private void ConsumePotion(string type)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] != null)
            {
                if (_items[i].itemName == type)
                {
                    _items.RemoveAt(i);
                    UpdateUI();

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

                    break;
                }
            }
        }
    }

    private void HandleDragging()
    {
        if (_draggingItem != null)
        {
            if (_draggingIcon != null)
            {
                _draggingIcon.transform.position = Input.mousePosition;

                if (Input.GetMouseButtonUp(0))
                {
                    for (int i = 0; i < _itemSlots.Count; i++)
                    {
                        if (_itemSlots[i] != null)
                        {
                            RectTransform slotRect = _itemSlots[i].GetComponent<RectTransform>();
                            if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, Input.mousePosition))
                            {
                                ItemPickup temp = null;

                                if (i < _items.Count)
                                {
                                    temp = _items[i];
                                }

                                _items[i] = _draggingItem;

                                if (temp != null)
                                {
                                    _items.Add(temp);
                                }

                                UpdateUI();
                                break;
                            }
                        }
                    }

                    Destroy(_draggingIcon.gameObject);
                    _draggingItem = null;
                    _draggingIcon = null;
                }
            }
        }
    }

    public void BeginDrag(int slotIndex)
    {
        if (slotIndex < _items.Count)
        {
            if (_items[slotIndex] != null)
            {
                _draggingItem = _items[slotIndex];

                GameObject icon = new GameObject("DraggingIcon");
                if (icon != null)
                {
                    if (_inventory != null)
                    {
                        icon.transform.SetParent(_inventory.transform);
                        icon.transform.SetAsLastSibling();

                        _draggingIcon = icon.AddComponent<Image>();
                        if (_draggingIcon != null)
                        {
                            _draggingIcon.raycastTarget = false;
                        }
                    }
                }
            }
        }
    }
}