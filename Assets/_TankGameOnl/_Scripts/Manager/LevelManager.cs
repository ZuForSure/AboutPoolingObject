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

    private void Start()
    {
        currentLevelIndex = levels[0].levelIndex;
        currentExpRequired = levels[currentLevelIndex-1].expRequired;
        //Hello NIga
        // test
        //Alo12324434665475367
    }

    private void Awake()
    {
        Instance = this;
    }
    public void AddExp(int exp)
    {
        Debug.Log($"AddExp - Sever : {NetworkServer.active} - Client: {NetworkClient.active}");
        
        currentExp += exp;
        Debug.Log($"AddExp - Exp: {exp} - CurrentExp: {currentExp} - CurrentLevelIndex: {currentLevelIndex}");
        CheckCurrentLever();
        //UiManager.Instance.SetSliderExp(currentExp, currentExpRequired);
    }    
    private void CheckCurrentLever()
    {
        if(currentLevelIndex >= levels.Count) return;
        if(currentExp >= levels[currentLevelIndex-1].expRequired)
        {
            currentExp -= levels[currentLevelIndex -1].expRequired;
            currentLevelIndex++;
            currentExpRequired = levels[currentLevelIndex-1].expRequired;
            Debug.Log($"Level Up! New Level: {currentLevelIndex}");
            UiManager.Instance.SetTextLevel(currentLevelIndex);
        }
    }
    //public void SyncExp(int exp)
    //{
    //    Debug.Log($"SyncExp - Sever : {NetworkServer.active} - Client: {NetworkClient.active}");
    //    currentExp = exp;
    //}
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
}
