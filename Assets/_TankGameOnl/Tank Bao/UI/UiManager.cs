using TMPro;
using UnityEngine;

public class UiManager : ZuSingleton<UiManager>
{  
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    public void ShowUiHeal(bool isShow)
    {
        canvasGroup.alpha = isShow ? 1 : 0;
        canvasGroup.interactable = isShow;
        canvasGroup.blocksRaycasts = isShow;
    }

    public void SetTextHeal(int heal)
    {
        text.text = $"Heal: {heal}";
    }
}
