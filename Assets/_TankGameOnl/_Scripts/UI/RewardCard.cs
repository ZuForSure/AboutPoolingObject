using MCP.DataModels.BaseModels;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour
{
    [SerializeField] private RectTransform[] arrayCardPos;
    [SerializeField] private SkillCardData[] arraySkillCard;
    [SerializeField] private GameObject cardPref;
    [SerializeField] private GameObject parentPos;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Dropdown languageDropDown;
    [SerializeField] private string[] languageItems;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        LevelManager.Instance.OnHandlerActive += Init;
        SetCanvasGroup(false);
        languageItems = DataHolder.Instance().GetLanguageNameList();
        InitLanguageDropDown();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnHandlerActive -= Init;

    }
    private void Init()
    {
        InitArrayRect();
        InitCardPref();
        InitDataSkillCard();
    }
    private void InitLanguageDropDown()
    {
        //languageDropDown.ClearOptions();
        List<string> options = new(languageItems);
        //languageDropDown.AddOptions(options);
        //languageDropDown.onValueChanged.AddListener(OnDropdownChanged);

    }
    private void InitArrayRect()
    {
        int count = parentPos.transform.childCount;

        arrayCardPos = new RectTransform[count];
        for (int i = 0; i < count; i++)
        {
            arrayCardPos[i] = parentPos.transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    private void InitCardPref()
    {
        List<SkillCardData> skillCards = new();
        for (int i = 0; i < arrayCardPos.Length; i++)
        {
            GameObject card = Instantiate(cardPref, arrayCardPos[i].position, Quaternion.identity, arrayCardPos[i]);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            if (card.TryGetComponent(out SkillCardData skillCard))
            {
                skillCard.SetCanvasGroup(true);
                skillCards.Add(skillCard);
            }
            else
            {
                Debug.LogWarning("SkillCardData component not found on the card prefab.");
            }
        }
        if (skillCards.Count > 0)
        {
            InitArraySkillCard(skillCards.ToArray());
        }
        else
        {
            Debug.LogWarning("No SkillCardData components found in the instantiated cards.");
        }
    }
    public void InitArraySkillCard(SkillCardData[] skillCards)
    {
        arraySkillCard = skillCards;

    }
    public void InitDataSkillCard()
    {
        if (arraySkillCard == null || arraySkillCard.Length == 0)
        {
            Debug.LogWarning("ArraySkillCard is not initialized or empty.");
            return;
        }

        for (int i = 0; i < arraySkillCard.Length; i++)
        {
            int index = i; // tránh closure bug
            string nameCard = LevelManager.Instance.arrayCard[index].languageItem[0].Name;
            string contentCard = LevelManager.Instance.arrayCard[index].languageItem[0].Description;
            int idCard = LevelManager.Instance.arrayCard[index].id;

            AssetManager.instance.LoadSprite(LevelManager.Instance.GetNameNoExtCard(index),
                (sprite) =>
                {
                    arraySkillCard[index].SetData(sprite, contentCard, nameCard, idCard);
                });
        }
    }

    public void SetCanvasGroup(bool isShow)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isShow ? 1f : 0f;
            canvasGroup.interactable = isShow;
            canvasGroup.blocksRaycasts = isShow;
        }
    }
    private void OnDropdownChanged(int index)
    {
        string selected = languageDropDown.options[index].text;
        Debug.Log("Bạn đã chọn: " + selected);
    }
    private void ChangeLanguage(string language)
    {
        //for (int i = 0; i < arraySkillCard.Length; i++)
        //{
        //    arraySkillCard[i].
        //}
    }

}
