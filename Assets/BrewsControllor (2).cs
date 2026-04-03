using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BrewsControllor : MonoBehaviour
{
    public SlotSpawn SS;

    public List<Sprite> itemIcon;

    public int[] OwnNumber;

    public TextMeshPro[] numbersText;
    void Start()
    {
        SS = FindAnyObjectByType<SlotSpawn>();
    }

    public void AddRedS()
    {
        OwnNumber[0]++;
        SS.AddItem(itemIcon[0], OwnNumber[0]);
       

    }

    public void AddPurpleS()
    {
        OwnNumber[1]++;
        SS.AddItem(itemIcon[1], OwnNumber[1]);
        
    }

    public void AddBlueS()
    {
        OwnNumber[2]++;
        SS.AddItem(itemIcon[2], OwnNumber[2] );
           

    }

    public void AddRedL()
    {
        OwnNumber[3]++;
        SS.AddItem(itemIcon[3], OwnNumber[3]);
      
    }

    public void AddPurpleL()
    {
        OwnNumber[4]++;
        SS.AddItem(itemIcon[4], OwnNumber[4]);
        
    }

    public void AddBlueL()
    {
        OwnNumber[5]++;
        SS.AddItem(itemIcon[5], OwnNumber[5]);
      

    }

    
}
