using MCP.DataModels.BaseModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardCard : MonoBehaviour
{
    [SerializeField] private RectTransform[] arrayCardPos;
    [SerializeField] private SkillCardData[] arraySkillCard;
    [SerializeField] private GameObject cardPref;

    private void Start()
    {
        LevelManager.Instance.OnHandlerActive += Init;
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
    private void InitArrayRect()
    {
        int count = transform.childCount;

        arrayCardPos = new RectTransform[count];
        for (int i = 0; i < count; i++)
        {
            arrayCardPos[i] = transform.GetChild(i).GetComponent<RectTransform>();
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

            AssetManager.instance.LoadSprite(LevelManager.Instance.GetNameNoExtCard(index),
                (sprite) =>
                {
                    arraySkillCard[index].SetData(sprite, nameCard);
                });
        }
    }
}
