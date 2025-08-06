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


    private void Start()
    {
        buttonStart.onClick.AddListener(OnButtonReady);
    }

    public void ShowUiHeal(bool isShow)
    {
        SetupCanvasGroup(isShow, cvgPopupHeal);
    }
    public void ShowUiButton(bool isShow)
    {
        SetupCanvasGroup(isShow, cvgPopupButtonReady);
        SetTextReady(!isShow);
       
    }
    public void ShowUiSlider(bool isShow)
    {
        SetupCanvasGroup(isShow, cvgPopupSlider);
        SetTextLevel(LevelManager.instance.CurrentLevelIndex);
        SetSliderExp(LevelManager.instance.CurrentExp, LevelManager.instance.CurrentExpRequired);
    }

    public void SetTextHeal(int heal)
    {
        text.text = $"Heal: {heal}";
    }
    private void SetupCanvasGroup(bool isShow,CanvasGroup canvasGroup)
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
    public void SetSliderExp(int currentExp, int currentExpRequired )
    {
        sliderExp.value = currentExp;
        sliderExp.maxValue = currentExpRequired;

    }
    public void SetTextLevel(int currentLevel)
    {
        textLevel.text = $"{currentLevel}";
    }    
    #endregion
}
