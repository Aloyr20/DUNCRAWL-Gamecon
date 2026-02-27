using UnityEngine;

public class StoreTrigger : MonoBehaviour
{
    public GameObject _storeBlock;
    public Dialogue shopkeeperDialogue;
    public DialogueManager dialogueManager;
    public TurnScript turnScript;

    public bool _ableInteractive = false;
    public bool _ableInventory = true;

    public bool _storeOpen = false;
    public bool _inShopRange = false;

    bool _waitingForStore = false;

    private Inventory _inventory;

    void Start()
    {
        _inventory = Object.FindFirstObjectByType<Inventory>();
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (_storeBlock != null)
        {
            _storeBlock.SetActive(false);
        }
    }

    void Update()
    {
        if (_waitingForStore && !dialogueManager.IsDialogueActive())
        {
            OpenStore();
            _waitingForStore = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && _inShopRange && !_storeOpen )
        {
            if (!dialogueManager.IsDialogueActive() && !_inventory.IsInventoryOpen())
            {
                dialogueManager.StartDialogue(shopkeeperDialogue);
                _waitingForStore = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _storeOpen)
        {
            CloseStore();
        }
    }

    void OpenStore()
    {
        _storeOpen = true;
        turnScript.enabled = false;
        if (_storeBlock != null)
        {
            _storeBlock.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void CloseStore()
    {
        _storeOpen = false;
        turnScript.enabled = true;
        if (_storeBlock != null)
        {
            _storeBlock.SetActive(false);
            dialogueManager.EnableGame();
            Time.timeScale = 1f;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _waitingForStore = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShopKeeper"))
        {
            _ableInteractive = true;
            _inShopRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ShopKeeper"))
        {
            if (_storeOpen)
            {
                CloseStore();
            }
            _ableInteractive = false;
            _inShopRange = false;
        }
    }
}