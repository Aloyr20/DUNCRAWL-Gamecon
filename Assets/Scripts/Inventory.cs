using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject _inventory;
    public StoreTrigger _store;
    public DialogueManager _dialogueManager;
    public int sRed;
    public int sBlue;
    public int sPurple;
    public int bRed;
    public int bBlue;
    public int bPurple;
    public List<TextMeshProUGUI> _numOwn;

    bool _inventoryOpen = false;

    void Start()
    {
        _store = FindAnyObjectByType<StoreTrigger>();
        _dialogueManager = FindAnyObjectByType<DialogueManager>();

        sRed = 0;
        sBlue = 0;
        sPurple = 0;
        bRed = 0;
        bBlue = 0;
        bPurple = 0;

        UpdateAllNumbers();
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

    public void UpdateAllNumbers()
    {
        if (_numOwn.Count >= 6)
        {
            _numOwn[0].text = sRed.ToString();
            _numOwn[1].text = sBlue.ToString();
            _numOwn[2].text = sPurple.ToString();
            _numOwn[3].text = bRed.ToString();
            _numOwn[4].text = bBlue.ToString();
            _numOwn[5].text = bPurple.ToString();
        }
    }

    public bool IsInventoryOpen()
    {
        return _inventoryOpen;
    }
}