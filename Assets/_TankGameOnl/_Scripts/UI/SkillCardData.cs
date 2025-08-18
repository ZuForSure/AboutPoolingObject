using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class SkillCardData : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler , IPointerClickHandler
{
    [SerializeField] private Image iconItem;
    [SerializeField] private TextMeshProUGUI textContentItem;
    [SerializeField] private TextMeshProUGUI textNameItem;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Outline outline;
    [SerializeField] private int idItem;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        outline = GetComponent<Outline>();
    }
   
    public void SetData(Sprite icon, string contentItem , string nameItem, int idItem)
    {
        if (iconItem != null)
            iconItem.sprite = icon;
        if (textNameItem != null)
            textNameItem.text = nameItem;
        if(textContentItem != null)
            textContentItem.text = contentItem; // Assuming content is the same as name, adjust as needed
        this.idItem = idItem;
    }
    public void SetCanvasGroup(bool isShow)
    {
       if(canvasGroup != null)
        {
            canvasGroup.alpha = isShow ? 1f : 0f;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(outline != null)
        {
            SetAlphaOutline(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outline != null)
        {
            SetAlphaOutline(false);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        UiManager.Instance.rewardCard.SetCanvasGroup(false);
        LevelManager.Instance.OnClickItemReward?.Invoke(idItem);
    }

    private void SetAlphaOutline(bool isShow)
    {
        Color color = outline.effectColor;
        color.a = isShow ? 1f : 0f;
        outline.effectColor = color; // Set the alpha of the outline color

    }

  
}
