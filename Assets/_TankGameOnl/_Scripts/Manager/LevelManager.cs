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

public class LevelManager : ZuSingleton<LevelManager>
{
    [SerializeField] private List<LevelData> levels;
    [SerializeField] private int currentLevelIndex = 0; public int CurrentLevelIndex => currentLevelIndex;
    [SerializeField] private int currentExp = 0; public int CurrentExp => currentExp;
    [SerializeField] private int currentExpRequired => currentLevelIndex < levels.Count ? levels[currentLevelIndex].expRequired : 0;
    public int CurrentExpRequired => currentExpRequired;


    public void AddExp(int exp)
    {
        currentExp += exp;
        CheckCurrentLever();
        UiManager.instance.SetSliderExp(currentExp, currentExpRequired);
    }    
    private void CheckCurrentLever()
    {
        if(currentLevelIndex >= levels.Count) return;
        if(currentExp >= levels[currentLevelIndex].expRequired)
        {
            currentExp -= levels[currentLevelIndex].expRequired;
            currentLevelIndex++;
            Debug.Log($"Level Up! New Level: {currentLevelIndex}");
            UiManager.instance.SetTextLevel(currentLevelIndex);
        }
    }    
}
