using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotSpawn : MonoBehaviour
{
    public Transform parent;       
    public GameObject slotPrefab;  
    public int slotCount = 6;
    public int slotIndex = 0;
  // public Sprite test1;
    SlotUi[] slotUis;


    public Sprite[] icon;


    void Start()
    {



        slotUis = new SlotUi[slotCount];

        for (int i = 0; i < slotCount; i++)
        {

            GameObject a = Instantiate(slotPrefab, parent);
            slotUis[i] = a.GetComponent<SlotUi>();

        }
        //slotUis[0].SetIcon(test1);
       
    }

    

    public void AddItem(Sprite itemIcon, int NumberOwn)
    {

        if (itemIcon == null) 
        { 
            return; 
        }

        if (slotIndex >= slotCount)
        {
            return; //full

        }

        for (int i = 0; i < slotCount; i++)
        {
            if (icon[i] == itemIcon)
            {
                slotUis[i].SetIcon(itemIcon, NumberOwn);
                return;
            }

        }


        icon[slotIndex] = itemIcon;


        slotUis[slotIndex].SetIcon(itemIcon,NumberOwn);

        slotIndex++;


    }


    
}
