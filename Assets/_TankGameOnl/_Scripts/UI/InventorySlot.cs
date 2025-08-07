using Mirror;
using Mirror.Examples.Tanks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image[] slotImages; // Gán trong Inspector
    public Sprite[] itemIcons; // itemIcons[ itemID ]
    public CanvasGroup cvgInventorySlot;

    private void Start()
    {
        ShowInventory(false);
    }


    public void UpdateUI(Tank tank)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < tank.inventory.Count)
            {
                int itemID = tank.inventory[i];
                Debug.Log($"Slot {i} - Item ID: {itemID}");
                slotImages[i].sprite = itemIcons[itemID];
                ShowAlpha(true, slotImages[i]);
                Debug.Log($"Slot {i} - Icon: {itemIcons[itemID].name}");
            }
            else
            {
                slotImages[i].sprite = null;
                ShowAlpha(false, slotImages[i]); // ?n icon n?u không có item
            }
            
        }
    }
    public void ResetUiItem()
    { 
        for(int i = 0;i < slotImages.Length;i++)
        {
            slotImages[i].sprite = null;
            ShowAlpha(false , slotImages[i]); ;
        }
    }

    public void ShowInventory(bool show)
    {
        cvgInventorySlot.alpha = show ? 1f : 0f;
        cvgInventorySlot.interactable = show;
        cvgInventorySlot.blocksRaycasts = show;
    }
    private void ShowAlpha(bool isShow, Image image)
    {
        Color color = image.color;
        color.a = isShow ? 1f : 0f; // ??t alpha v? 1 ?? hi?n th? ho?c 0 ?? ?n
        image.color = color;
    }
}
