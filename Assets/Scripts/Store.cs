using UnityEngine;

public class Store : MonoBehaviour
{
    public Inventory _inv;
    private StoreTrigger _storeTrigger;

    void Start()
    {
        _storeTrigger = Object.FindFirstObjectByType<StoreTrigger>();
    }

    void Update()
    {
        if (_inv == null)
        {
            _inv = Object.FindFirstObjectByType<Inventory>();
        }
        if (_storeTrigger == null)
        {
            _storeTrigger = Object.FindFirstObjectByType<StoreTrigger>();
        }
    }

    void PurchaseItem(System.Action purchaseAction)
    {
        if (_storeTrigger != null && _storeTrigger._storeOpen && _inv != null)
        {
            purchaseAction();
            _inv.UpdateAllNumbers();
        }
    }

    public void sRed()
    {
        PurchaseItem(() => _inv.sRed++);
    }

    public void sBlue()
    {
        PurchaseItem(() => _inv.sBlue++);
    }

    public void sPurple()
    {
        PurchaseItem(() => _inv.sPurple++);
    }

    public void bRed()
    {
        PurchaseItem(() => _inv.bRed++);
    }

    public void bBlue()
    {
        PurchaseItem(() => _inv.bBlue++);
    }

    public void bPurple()
    {
        PurchaseItem(() => _inv.bPurple++);
    }
}