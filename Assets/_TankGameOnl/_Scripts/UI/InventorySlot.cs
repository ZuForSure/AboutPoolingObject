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


    public void AddItemUI(int index , int value )
    {
        if (index < 0 || index >= slotImages.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for slotImages array.");
            return;
        }

        Debug.Log($"additemUI - {slotImages[index].sprite} ");
        if (slotImages[index].sprite == null)
        {
          
            Debug.Log($"AddItemUI - index: {index} - value: {value}");
            slotImages[index].sprite = itemIcons[value]; // Gán icon mặc định nếu chưa có item
            ShowAlpha(true, slotImages[index]); // Hiển thị icon
        }

        //for (int i = 0; i < slotImages.Length; i++)
        //{
        //    if (i < tank.inventory.Count)
        //    {
        //        int itemID = tank.inventory[i];
        //        Debug.Log($"Slot {i} - Item ID: {itemID}");
        //        slotImages[i].sprite = itemIcons[itemID];
        //        ShowAlpha(true, slotImages[i]);
        //        Debug.Log($"Slot {i} - Icon: {itemIcons[itemID].name}");
        //    }
        //    else
        //    {
        //        slotImages[i].sprite = null;
        //        ShowAlpha(false, slotImages[i]); // ?n icon n?u không có item
        //    }

        //}
    }
    public void RemoveItemUI(SyncList<int> inventory)
    {
        Debug.Log($"RemoveItemUI - inventory count: {inventory.Count}");

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < inventory.Count)
            {
                int itemID = inventory[i];
                slotImages[i].sprite = itemIcons[itemID];
                ShowAlpha(true, slotImages[i]);
            }
            else
            {
                slotImages[i].sprite = null;
                ShowAlpha(false, slotImages[i]);
            }
        }
    }
    public void ResetUiItem()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].sprite = null;
            ShowAlpha(false, slotImages[i]); ;
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
