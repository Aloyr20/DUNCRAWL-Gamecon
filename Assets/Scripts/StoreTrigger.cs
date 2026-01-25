using Unity.VisualScripting;
using UnityEngine;

public class StoreTrigger : MonoBehaviour
{

    public GameObject _storeBlock;

    public bool _ableInteractive = false;
    public bool _ableInventory =true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _ableInteractive == true)
        {
            _ableInventory = !_ableInventory;
            _storeBlock.SetActive(!_storeBlock.activeSelf);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ShopKeeper"))
       {
            _ableInteractive = true;
       }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ShopKeeper"))
        {

            _ableInteractive = false;
        }
    }
}



