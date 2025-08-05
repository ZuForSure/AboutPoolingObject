using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : ZuSingleton<UiManager>
{  
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup popupHeal;
    [SerializeField] private CanvasGroup popupButtonReady;
    [SerializeField] private Button buttonStart;
    [SerializeField] private TextMeshProUGUI textReady;
    private Action onReadyClick;
    public Action<bool> OnReadyGame;

    private void Start()
    {
        OnReadyGame += SetText;
    }
    private void OnDestroy()
    {
        OnReadyGame -= SetText;
    }
    public void ShowUiHeal(bool isShow)
    {
        SetupCanvasGroup(isShow, popupHeal);
    }
    public void ShowUiButton(bool isShow)
    {
        SetupCanvasGroup(isShow, popupButtonReady);
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
    private void SetText(bool isReady)
    {
        textReady.text = isReady ? "Ready" : "Not Ready";
        buttonStart.interactable = isReady;
    }
    //public void RegisterReadyButton(Action callback)
    //{
    //    onReadyClick = callback;
    //    buttonStart.onClick.RemoveAllListeners(); // Clear listener cũ (nếu có)
    //    buttonStart.onClick.AddListener(() =>
    //    {
    //        onReadyClick?.Invoke();
    //    });
    //}
}
