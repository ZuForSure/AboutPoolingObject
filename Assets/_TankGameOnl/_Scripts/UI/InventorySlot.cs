using Mirror;
using Mirror.Examples.Tanks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image[] slotImages; // Gán trong Inspector
    public Sprite[] itemIcons; // itemIcons[ itemID ]

   

    public void UpdateUI(Tank tank)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < tank.inventory.Count)
            {
                int itemID = tank.inventory[i];
                Debug.Log($"Slot {i} - Item ID: {itemID}"); 
                slotImages[i].sprite = itemIcons[itemID];
                Debug.Log($"Slot {i} - Icon: {itemIcons[itemID].name}");
            }
            else
            {
                slotImages[i].sprite = null;
            }
        }
    }

    //// G?i t? Button OnClick()
    //public void OnClickSlot(int slotIndex)
    //{
    //    if (tank != null)
    //    {
    //        targetInventory.CmdUseItem(slotIndex);
    //    }
    //}
}
