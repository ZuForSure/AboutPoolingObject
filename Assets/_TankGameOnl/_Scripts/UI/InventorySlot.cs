using DG.Tweening;
using Mirror;
using Mirror.Examples.Tanks;
using Mono.CecilX.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
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
    }
    private int CheckSlotEmpty()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i].sprite == null)
            {
                return i; // trả về index của slot trống
            }
        }
        return -1; // không có slot trống
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
    private Vector2 ConvertToRectangle(RectTransform rectParent, Vector3 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectParent, RectTransformUtility.WorldToScreenPoint(null, pos), null, out Vector2 uiPos);
        return uiPos;

    }
    private RectTransform InitItemPrefUI(int itemID)
    {
        var go = Instantiate(itemIconPrefab, uiRoot); // <-- parent vào uiRoot
        var img = go.GetComponent<Image>();
        var rt = go.GetComponent<RectTransform>();

        if (img == null || rt == null || go == null)
        {
            Debug.LogWarning("Item icon prefab must have Image and RectTransform components.");
            return null;
        }
        img.sprite = itemIcons[itemID];
        return rt;
    }

    public void PlayFlyTween(float timeTween, int itemId, Vector3 worldPos)
    {
        int indexSlotEmpty = CheckSlotEmpty();
        if (indexSlotEmpty < 0)
        {
            Debug.LogWarning("No empty slot available");
            return;
        }
        if (itemId < 0 || itemId >= itemIcons.Length)
        {
            Debug.LogWarning($"itemId {itemId} out of range");
            return;
        }

        // 1) tạo icon và set parent
        var rt = InitItemPrefUI(itemId);
        if (rt == null)
        {
            Debug.LogWarning("Failed to initialize item UI prefab");
            return;
        }

        // 2) World -> UI (cùng hệ toạ độ của uiRoot) -> vì là object trong thế giới nên cần dùng camera.main để chuyển đổi
        Vector2 screen = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiRoot, screen, null, out var uiStart);
        rt.anchoredPosition = uiStart;

        // 3) đích đến: dùng anchoredPosition của slot -> vì object là UI element nên dùng RectTransformUtility để chuyển đổi
        var target = slotRects[indexSlotEmpty];
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    uiRoot, RectTransformUtility.WorldToScreenPoint(null, target.position), null, out Vector2 uiEnd);
        Vector2 uiEnd = ConvertToRectangle(uiRoot, target.position);

        var seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPos(uiEnd, timeTween).SetEase(Ease.OutCubic))
           .OnStart(() => Debug.Log("[DOTween] start"))
           .OnComplete(() =>
           {
               Debug.Log("[DOTween] complete");
               Destroy(rt.gameObject);
           });
    }
    public void ItemFlyToUiHeal(int itemID, int slotIndex, float timer)
    {
        var target = slotRects[slotIndex];
        Vector2 uiStart = ConvertToRectangle(uiRoot, target.position);

        var rt = InitItemPrefUI(itemID);
        rt.anchoredPosition = uiStart;

        var rectHealUI = UiManager.Instance.GetRectUiHeal();
        var endUi = ConvertToRectangle(uiRoot, rectHealUI.position);

        var seq = DOTween.Sequence();
        seq.Append(rt.DOAnchorPos(endUi, timer).SetEase(Ease.OutCubic))
           .OnStart(() => Debug.Log("[DOTween] start"))
           .OnComplete(() =>
           {
               Debug.Log("[DOTween] complete");
               Destroy(rt.gameObject);
           });


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        string nametag = "";
        GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
        if (gameObject == null) Debug.LogWarning("GameObject is null in OnPointerClick");

        Sprite sprite = gameObject.GetComponent<Image>().sprite;

        if (sprite == null) Debug.LogWarning("Sprite is null in OnPointerClick");
        else
        {
            switch (gameObject.tag)
            {
                case "Slot1":
                    Debug.Log("Clicked on Slot1");
                    nametag = "Slot1";
                    break;
                case "Slot2":
                    Debug.Log("Clicked on Slot2");
                    nametag = "Slot2";
                    break;
                case "Slot3":
                    Debug.Log("Clicked on Slot2");
                    nametag = "Slot3";
                    break;
                default:
                    Debug.Log($"Clicked on {gameObject.name}");
                    // Thực hiện hành động khác nếu không phải Slot1 hoặc Slot2
                    break;
            }
        }
        TankGameManager.Instance.OnSendEventClickItem?.Invoke(nametag);

    }
}
