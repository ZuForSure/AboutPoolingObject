using DG.Tweening;
using Mirror;
using Mirror.Examples.Tanks;
using System;
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
    public RectTransform[] slotRects;   // gán từng slot UI trong Inspector
    public RectTransform uiRoot;        // RectTransform của Canvas
    public GameObject itemIconPrefab;   // prefab UI có Image + RectTransform

    private void Start()
    {
        ShowInventory(false);
    }


    public void AddItemUI(int index, int value)
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

    public void PlayFlyTween(int itemId, int slotIndex, Vector3 worldPos)
    {
        Debug.Log($"[PlayFlyTween] itemId:{itemId}, slotIndex:{slotIndex}, worldPos:{worldPos}");

        if (slotIndex < 0 || slotIndex >= slotRects.Length)
        {
            Debug.LogWarning($"slotIndex {slotIndex} out of range");
            return;
        }
        if (itemId < 0 || itemId >= itemIcons.Length)
        {
            Debug.LogWarning($"itemId {itemId} out of range");
            return;
        }

        // 1) tạo icon và set parent
        var go = Instantiate(itemIconPrefab, uiRoot); // <-- parent vào uiRoot
        var img = go.GetComponent<Image>();
        var rt = go.GetComponent<RectTransform>();
        if (!img || !rt) { Debug.LogWarning("Prefab thiếu Image/RectTransform"); return; }
        img.sprite = itemIcons[itemId];


        // 2) World -> UI (cùng hệ toạ độ của uiRoot) -> vì là object trong thế giới nên cần dùng camera.main để chuyển đổi
        Vector2 screen = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiRoot, screen, null, out var uiStart);
        rt.anchoredPosition = screen;

        // 3) đích đến: dùng anchoredPosition của slot -> vì object là UI element nên dùng RectTransformUtility để chuyển đổi
        var target = slotRects[slotIndex];
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiRoot, RectTransformUtility.WorldToScreenPoint(null, target.position), null, out Vector2 uiEnd);

        var seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPos(uiEnd, 0.8f).SetEase(Ease.OutCubic))
           .OnStart(() => Debug.Log("[DOTween] start"))
           .OnComplete(() => { Debug.Log("[DOTween] complete"); Destroy(go); });
    }
}
