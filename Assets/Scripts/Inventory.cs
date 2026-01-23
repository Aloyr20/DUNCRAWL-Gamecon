using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
   public GameObject _inventory;

    public StoreTrigger _store;
    public int sRed;
    public int sBlue;
    public  int sPurple;
    public int bRed;
    public int bBlue;
    public int bPurple;

    public List<TextMeshProUGUI> _numOwn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      _store = FindAnyObjectByType<StoreTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && _store._ableInventory == true )
        {
            _inventory.SetActive(!_inventory.activeSelf);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        NumberUpdate();
    }

    void NumberUpdate()
    {
        _numOwn[0].text = sRed.ToString();
        _numOwn[1].text = sBlue.ToString();
        _numOwn[2].text = sPurple.ToString();
        _numOwn[3].text = bRed.ToString();
        _numOwn[4].text = bBlue.ToString();
        _numOwn[5].text = bPurple.ToString();
    }
}
