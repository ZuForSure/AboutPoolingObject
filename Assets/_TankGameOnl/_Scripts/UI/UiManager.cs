using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : ZuSingleton<UiManager>
{
    [Header("Text Heal")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup cvgPopupHeal;

    [Header("Button Ready")]
    [SerializeField] private CanvasGroup cvgPopupButtonReady;
    [SerializeField] private Button buttonStart;
    [SerializeField] private bool buttonReadyValue; public bool ButtonReadyValue => buttonReadyValue;
    [SerializeField] private TextMeshProUGUI textReady;
    public Action OnButtonReadyClick;

    [Header("Slider Exp")]
    [SerializeField] private Slider sliderExp;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private CanvasGroup cvgPopupSlider;

    [Header("IventorySlot")]
    public InventorySlot inventorySlot;

    //[Header("UI Move Button")]
    //public UIMoveButton uiMoveButton;



    private void Start()
    {
        buttonStart.onClick.AddListener(OnButtonReady);
    }

    public void ShowUiHeal(bool isShow)
    {
        SetupCanvasGroup(isShow, cvgPopupHeal);
    }
    public void ShowUiButtonReady(bool isShow, bool ownerReady = true)
    {
        Debug.Log($"Sever : {NetworkServer.active} - Client : {NetworkClient.active}");
        SetupCanvasGroup(isShow, cvgPopupButtonReady);
        SetTextReady(ownerReady);

    }
    public void ShowUiSlider(bool isShow)
    {
        SetupCanvasGroup(isShow, cvgPopupSlider);
        SetTextLevel(LevelManager.Instance.CurrentLevelIndex);
        SetSliderExp(LevelManager.Instance.CurrentExp, LevelManager.Instance.CurrentExpRequired);
    }

    public void SetTextHeal(int heal)
    {
        text.text = $"Heal: {heal}";
    }
    private void SetupCanvasGroup(bool isShow, CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = isShow ? 1 : 0;
        canvasGroup.interactable = isShow;
        canvasGroup.blocksRaycasts = isShow;
    }
    public void SetTextReady(bool isReady)
    {
        textReady.text = isReady ? "Ready" : "Not Ready";
        buttonStart.interactable = !isReady;
    }
    #region Button Ready
    public void OnButtonReady()
    {
        buttonReadyValue = true;
        SetTextReady(buttonReadyValue);
        OnButtonReadyClick?.Invoke();
    }
    #endregion

    #region Slider Exp
    public void SetSliderExp(int currentExp, int currentExpRequired)
    {
        sliderExp.value = currentExp;
        sliderExp.maxValue = currentExpRequired;

    }
    public void SetTextLevel(int currentLevel)
    {
        textLevel.text = $"Level {currentLevel}";
    }
    #endregion
    public RectTransform GetRectUiHeal()
    {
        if (!text.TryGetComponent<RectTransform>(out var rect))
        {
            Debug.LogWarning("TextMeshProUGUI does not have a RectTransform component.");
            return null;
        }
        return rect;
    }
}
