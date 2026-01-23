using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    public Inventory _inv;

    public GameObject _store;

    bool _ableInteractive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
        

    }



    public void sRed()
    {
        _inv.sRed++;
    }
   public void sBlue()
    {
        _inv.sBlue++;
    }

    public void sPurple()
    {
        _inv.sPurple++;
    }

    public void bRed()
    {
        _inv.bRed++;
    }

    public void bBlue()
    {
        _inv.bBlue++;
    }

    public void bPurple()
    {
        _inv.bPurple++;
    }
}
