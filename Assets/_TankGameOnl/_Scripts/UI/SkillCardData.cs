using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SkillCardData : MonoBehaviour
{
    [SerializeField] private Image iconItem;
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetData(Sprite icon, string name)
    {
        if (iconItem != null)
            iconItem.sprite = icon;
        if (textName != null)
            textName.text = name;
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
}
