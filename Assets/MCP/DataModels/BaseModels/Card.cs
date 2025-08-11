using System;
using MCP.DataModel.Utils;
using System.Collections.Generic;

namespace MCP.DataModels.BaseModels
{
    [System.Serializable]
    public class Card : BaseItem
    {
        public EquipmentType equipmentType = EquipmentType.Hair;
        public RareType rareType = RareType.Common;
        public Stat[] stats = new Stat[0];
        /// <summary>
        /// public Skill[] skills = new Skill[0];
        /// </summary>
        public bool canDrop = true;
        public bool canUpgrade = false;
        public bool useForCraft = false;
        public bool craftable = false;
        public bool canEquip = true;
        public bool canSummon = false;
        public bool canSell = true;
        public int[] craftOutputIds = new int[0];
        public int[] eventCraftOutputIds = new int[0];
        public int[] buildingParts = new int[0];
        public SkillType skillType = SkillType.None;
        public CurrencyData[] upgradeCost = new CurrencyData[0];
        public float upgradeLevelRate = 1;


        public CurrencyData[] GetUpgradeCost(int level)
        {
            CurrencyData[] cost = new CurrencyData[0];
            foreach (CurrencyData currencyData in upgradeCost)
            {
                CurrencyData newCurrencyData = new CurrencyData();
                DataUtils.CopyObject(newCurrencyData, currencyData);
                newCurrencyData.value = (int)(newCurrencyData.value * Math.Pow(upgradeLevelRate, level));
                cost = DataUtils.Add(newCurrencyData, cost);
            }
            return cost;
        }

        public float GetStat(StatDataType statDataType)
        {
            foreach(Stat stat in stats)
            {
                if(stat.statDataType == statDataType)
                {
                    return stat.GetValue();
                }
            }
            return 0;
        }

        public float GetStat(StatType statType)
        {
            foreach (Stat stat in stats)
            {
                if (stat.statType == statType)
                {
                    return stat.GetValue();
                }
            }
            return 0;
        }

        public bool IsCollection()
        {
            return equipmentType == EquipmentType.SkinAttack || equipmentType == EquipmentType.SkinRun || equipmentType == EquipmentType.SkinIdle
                || equipmentType == EquipmentType.SkinDeath || equipmentType == EquipmentType.SkinClan || equipmentType == EquipmentType.SkinTitle;
        }
    }

    [System.Serializable]
    public class Stat
    {
        public StatType statType = StatType.Health;
        public float value = 0;
        public float minValue = 0;
        public float maxValue = 0;
        public StatDataType statDataType = StatDataType.Normal;

        public Stat()
        {

        }

        public Stat(Stat stat)
        {
            this.value = stat.value;
            this.maxValue = stat.maxValue;
            this.minValue = stat.minValue;
            this.statType = stat.statType;
            this.statDataType = stat.statDataType;
        }

        public string StatName()
        {
            return statType.ToString();
        }

        public float GetValue(int level = 0)
        {
            if (level > 9)
                level = 9;
            return value * MathF.Pow(1.1f, level);
        }
    }


    [System.Serializable]
    public class StatValue
    {
        public StatType statType = StatType.Health;
        public float value = 0;

        public StatValue()
        {

        }

        public StatValue(Stat stat)
        {
            this.value = stat.value;
            this.statType = stat.statType;
        }


        public StatValue(StatValue stat)
        {
            this.value = stat.value;
            this.statType = stat.statType;
        }

        public StatValue(StatValue stat,int level)
        {
            this.value = stat.GetValue(level);
            this.statType = stat.statType;
        }

        public string StatName()
        {
            return statType.ToString();
        }

        public float GetValue(int level = 0)
        {
            if (level > 9)
                level = 0;

            if (statType == StatType.CoolDown)
                return value;
            else
                return value * MathF.Pow(1.1f, level);
        }
    }

    [System.Serializable]
    public class StatData
    {
        public List<StatValue> stats = new List<StatValue>();
        public StatValue[] finalStats = new StatValue[0];

        public StatData()
        {
            Initialize();
        }

        
        void Initialize(int level = 0)
        {
            finalStats = new StatValue[0];
            //Hard code here 
            for (int i=0;i<29;i++)
            {
                StatValue stat = new StatValue();
                stat.statType = (StatType)i;
                finalStats = DataUtils.Add(stat, finalStats);
            }
        }

        public StatValue GetStat(StatType statType)
        {
            return finalStats[(int)statType];
        }

        public void AddStats(StatValue[] addStats)
        {
            stats.AddRange(addStats);
            UpdateStats();
        }

        public void AddEffect(StatData effectData)
        {
            for (int i = 0; i < finalStats.Length; i++)
            {
                for (int j = 0; j < effectData.finalStats.Length; j++)
                {
                    if (finalStats[i].statType == effectData.finalStats[j].statType)
                    {
                        if (finalStats[i].statType == StatType.Heal)
                        {
                            finalStats[i].value += effectData.finalStats[j].GetValue();
                        }
                        //DoT stats
                        else if (finalStats[i].statType == StatType.PoisonDoT)
                        {   
                            finalStats[i].value += effectData.finalStats[j].GetValue();
                        }
                        else if (finalStats[i].statType == StatType.HitSpeed)
                        {
                            /*
                            var effectValue = 1 - effectData.finalStats[j].GetValue() / 100;
                            finalStats[i].value *= effectValue;
                            if (finalStats[i].value <= 0.1)
                            {
                                finalStats[i].value = 0.1f;
                            }*/
                        }
                        else
                        {
                            var effectValue = 1 + effectData.finalStats[j].GetValue() / 100;
                            finalStats[i].value *= effectValue;
                        }
                    }
                        
                }
            }
        }

        public void AddBuff(StatData buffData)
        {
            for (int i = 0; i < finalStats.Length; i++)
            {
                for (int j = 0; j < buffData.finalStats.Length; j++)
                {
                    if (finalStats[i].statType == buffData.finalStats[j].statType)
                    {                      
                        finalStats[i].value += buffData.finalStats[j].GetValue();
                    }

                }
            }
        }

        public void AddStats(Stat[] addStats)
        {
            foreach(Stat stat in addStats)
            {
                StatValue statValue = new StatValue(stat);
                stats.Add(statValue);
            }
            UpdateStats();
        }

        public void ResetStats()
        {
            stats = new List<StatValue>();

            for (int i = 0; i < finalStats.Length; i++)
            {
                finalStats[i].value = 0;
            }
        }


        public void UpdateStats()
        {
            //Update Normal
            for (int i = 0; i < finalStats.Length; i++)
            {
                finalStats[i].value = 0;
                for (int j = 0; j < stats.Count; j++)
                {
                    if (finalStats[i].statType == stats[j].statType)
                        finalStats[i].value += stats[j].GetValue(); 
                }
            }
        }
    }
}




