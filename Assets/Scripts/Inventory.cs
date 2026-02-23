using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject _inventory;
    public StoreTrigger _store;
    public DialogueManager _dialogueManager;
    public List<TextMeshProUGUI> _numOwn;

    bool _inventoryOpen = false;

    void Start()
    {
        _store = FindAnyObjectByType<StoreTrigger>();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !_store._storeOpen && !_dialogueManager.dialogueActive)
        {
            ToggleInventory();
        }

        if (_inventoryOpen && _dialogueManager.dialogueActive)
        {
            CloseInventory();
        }
    }

    void ToggleInventory()
    {
        _inventoryOpen = !_inventoryOpen;
        _inventory.SetActive(_inventoryOpen);

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

    void CloseInventory()
    {
        _inventoryOpen = false;
        _inventory.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsInventoryOpen()
    {
        return _inventoryOpen;
    }
}