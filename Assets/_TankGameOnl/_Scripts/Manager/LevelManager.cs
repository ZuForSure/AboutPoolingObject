using MCP.DataModels.BaseModels;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct LevelData
{
    public int levelIndex;
    public int expRequired;
}

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private List<LevelData> levels;
    [SyncVar(hook = nameof(OnChangeLevel))]
    [SerializeField] private int currentLevelIndex; 
    public int CurrentLevelIndex => currentLevelIndex;
    [SyncVar(hook = nameof(OnExpChanged))]
    [SerializeField] private int currentExp = 0; 
    public int CurrentExp => currentExp;
    [SyncVar]
    [SerializeField] private int currentExpRequired;
    public int CurrentExpRequired => currentExpRequired;
    [SyncVar]
    public bool isLevelUp;

    [Header("Reward")]
    public Card[] arrayCard;

    public Action OnHandlerActive;
    public Action<int> OnClickItemReward;

    private void OnEnable()
    {
        arrayCard = DataHolder.Instance().GetDatas<Card>();
        OnHandlerActive?.Invoke();
    }


    private void Start()
    {
        currentLevelIndex = levels[0].levelIndex;
        currentExpRequired = levels[currentLevelIndex-1].expRequired;
    }
   

    private void Awake()
    {
        Instance = this;
        
    }
    public void AddExp(int exp)
    {
        Debug.Log($"AddExp - Sever : {NetworkServer.active} - Client: {NetworkClient.active}");
        if (!NetworkServer.active) return;
        currentExp += exp;
        Debug.Log($"AddExp - Exp: {exp} - CurrentExp: {currentExp} - CurrentLevelIndex: {currentLevelIndex}");
        CheckCurrentLever();
        UiManager.Instance.SetSliderExp(currentExp, currentExpRequired);
    }
    private void CheckCurrentLever()
    {
        if (currentLevelIndex >= levels.Count) return;
        if(currentExp >= levels[currentLevelIndex-1].expRequired)
        {
            currentExp -= levels[currentLevelIndex -1].expRequired;
            currentLevelIndex++;
            RpcShowUiReward();
            isLevelUp = true;
            currentExpRequired = levels[currentLevelIndex-1].expRequired;
            Debug.Log($"Level Up! New Level: {currentLevelIndex}");
            UiManager.Instance.SetTextLevel(currentLevelIndex);
        }
    }
    [ClientRpc]
    private void RpcShowUiReward()
    {
        Debug.Log($"TargetShowUiReward - Sever : {NetworkServer.active} - Client: {NetworkClient.active}");
        UiManager.Instance.rewardCard.SetCanvasGroup(true);
    }    
    //public void SyncExp(int exp)
    //{
    //    Debug.Log($"SyncExp - Sever : {NetworkServer.active} - Client: {NetworkClient.active}");
    //    currentExp = exp;
    //}
    public bool IsLevelUp()
    {
        return isLevelUp;
    }    
    private void OnExpChanged(int oldExp, int newExp)
    {
        UiManager.Instance.SetSliderExp(newExp, currentExpRequired);
        Debug.Log($"OnExpChanged - OldExp: {oldExp} - NewExp: {newExp} - CurrentLevelIndex: {currentLevelIndex}");
    }
    private void OnChangeLevel(int oldLevel, int newLevel)
    {
        UiManager.Instance.SetTextLevel(newLevel);
        Debug.Log($"OnChangeLevel - OldLevel: {oldLevel} - NewLevel: {newLevel}");
    }
    public static string GetNameNoExt(string path)
    {
        // Tìm vị trí dấu chấm cuối cùng (trước phần mở rộng)
        int dotIndex = path.LastIndexOf('.');
        if (dotIndex == -1) return string.Empty;

        // Tìm vị trí dấu "/" gần nhất trước dấu chấm
        int slashIndex = path.LastIndexOf('/', dotIndex);
        if (slashIndex == -1) return string.Empty;

        // Cắt chuỗi từ sau "/" đến trước "."
        return path.Substring(slashIndex + 1, dotIndex - slashIndex - 1);
    }
    public string GetNameNoExtCard(int index)
    {
        if (index < 0 || index >= arrayCard.Length)
        {
            Debug.LogWarning($"Index {index} is out of bounds for arrayCard.");
            return string.Empty;
        }
        return GetNameNoExt(arrayCard[index].iconUrl);
    }
  

}
