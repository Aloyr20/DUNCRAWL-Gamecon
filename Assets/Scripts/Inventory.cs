using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject _inventory;

    public StoreTrigger _store;
  

    public List<TextMeshProUGUI> _numOwn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _store = FindAnyObjectByType<StoreTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && _store._ableInventory == true)
        {
            _inventory.SetActive(!_inventory.activeSelf);
            _store._ableInteractive =!_store._ableInteractive;
    Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


    }

}
