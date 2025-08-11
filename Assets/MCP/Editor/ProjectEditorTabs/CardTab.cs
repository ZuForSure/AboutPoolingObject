

using System.Collections.Generic;
using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace MCP.Editor
{
	public class CardTab : BaseTab
	{
        int statId = 0;

        public CardTab(ProjectWindow pw) : base(pw)
		{
			this.Reload();
		}


        public override void SaveTab()
        {
            base.SaveTab();
            DataHolder.Instance().SaveData<Card>();
        }

        public override void ShowTab()
		{
            
            base.ShowTab();
			AddToolBarEditor<Card>();
			
			if (DataHolder.Instance().GetDataCount<Card>(currentKey) > 0)
			{				
                Card item = DataHolder.Instance().GetData<Card>(selection, currentKey);
                //Debug.Log(currentKey + "  " + item.languageItem[0].Name);
                AddItemEditor(item);

                EditorGUI.BeginChangeCheck();

                //EditorGUILayout.LabelField("GUID: " + item.guId);
                item.rareType = (RareType)EditorTab.EnumToolbar("Rare Type", (int)item.rareType, typeof(RareType), pw.mWidth);
                item.equipmentType = (EquipmentType)EditorGUILayout.EnumPopup("Equipment Type",item.equipmentType, GUILayout.Width(pw.mWidth));
                item.canDrop = EditorGUILayout.Toggle("Can Drop", item.canDrop, GUILayout.Width(pw.mWidth / 3 * 2));
                item.useForCraft = EditorGUILayout.Toggle("Use For Craft", item.useForCraft, GUILayout.Width(pw.mWidth / 3 * 2));

                if (item.useForCraft)
                {
                    EditorGUILayout.BeginVertical("box");
                    fold5 = EditorGUILayout.Foldout(fold5, "Event Craft Outputs");
                    if (fold5)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Event Craft Output", GUILayout.Width(pw.mWidth / 3)))
                        {
                            item.eventCraftOutputIds = DataUtils.Add<int>(0, item.eventCraftOutputIds);
                        }
                        EditorGUILayout.EndHorizontal();

                        for (int i = 0; i < item.eventCraftOutputIds.Length; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            item.eventCraftOutputIds[i] = (int)EditorGUILayout.Popup(item.eventCraftOutputIds[i], DataHolder.Instance().GetDataNameList<Card>(), GUILayout.Width(pw.mWidth / 2));
                            if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                            {
                                item.eventCraftOutputIds = DataUtils.Remove<int>(i, item.eventCraftOutputIds);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                if (item.useForCraft)
                {
                    item.canSummon = false;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Craft Output", GUILayout.Width(pw.mWidth / 3)))
                    {
                        item.craftOutputIds = DataUtils.Add<int>(0, item.craftOutputIds);
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < item.craftOutputIds.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        item.craftOutputIds[i] = (int)EditorGUILayout.Popup(item.craftOutputIds[i], DataHolder.Instance().GetDataNameList<Card>(), GUILayout.Width(pw.mWidth / 2));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                        {
                            item.craftOutputIds = DataUtils.Remove<int>(i, item.craftOutputIds);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                item.craftable = EditorGUILayout.Toggle("Craftable", item.craftable, GUILayout.Width(pw.mWidth / 3 * 2));
                item.canEquip = EditorGUILayout.Toggle("Can Equip", item.canEquip, GUILayout.Width(pw.mWidth / 3 * 2));
                item.canSell = EditorGUILayout.Toggle("Can Sell", item.canSell, GUILayout.Width(pw.mWidth / 3 * 2));
                item.canSummon = EditorGUILayout.Toggle("Can Summon", item.canSummon, GUILayout.Width(pw.mWidth / 3 * 2));
                if (item.canSummon)
                {
                    item.useForCraft = false;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Summon Output", GUILayout.Width(pw.mWidth / 2)))
                    {
                        item.craftOutputIds = DataUtils.Add<int>(0, item.craftOutputIds);
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < item.craftOutputIds.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        item.craftOutputIds[i] = (int)EditorGUILayout.Popup(item.craftOutputIds[i], DataHolder.Instance().GetDataNameList<Card>(), GUILayout.Width(pw.mWidth / 2));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                        {
                            item.craftOutputIds = DataUtils.Remove<int>(i, item.craftOutputIds);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                item.canUpgrade = EditorGUILayout.Toggle("Can Upgrade", item.canUpgrade, GUILayout.Width(pw.mWidth / 3 * 2));
                
                if (item.canUpgrade)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Currency", GUILayout.Width(pw.mWidth / 3)))
                    {
                        item.upgradeCost = DataUtils.Add<CurrencyData>(new CurrencyData(), item.upgradeCost);
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < item.upgradeCost.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        item.upgradeCost[i].value = EditorGUILayout.IntField("Upgrade Cost", item.upgradeCost[i].value, GUILayout.Width(pw.mWidth));
                        item.upgradeCost[i].currencyId = (int)EditorGUILayout.Popup(item.upgradeCost[i].currencyId, DataHolder.Instance().GetDataNameList<Currency>(), GUILayout.Width(pw.mWidth / 2));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                        {
                            item.upgradeCost = DataUtils.Remove<CurrencyData>(i, item.upgradeCost);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    item.upgradeLevelRate = EditorGUILayout.FloatField("Upgrade Rate", item.upgradeLevelRate, GUILayout.Width(pw.mWidth / 3 * 2));

                }

                if (item.equipmentType == EquipmentType.Building)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Building Part", GUILayout.Width(pw.mWidth / 3)))
                    {
                        item.buildingParts = DataUtils.Add<int>(0, item.buildingParts);
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < item.buildingParts.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        item.buildingParts[i] = (int)EditorGUILayout.Popup(item.buildingParts[i], DataHolder.Instance().GetDataNameList<Card>(), GUILayout.Width(pw.mWidth / 2));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                        {
                            item.buildingParts = DataUtils.Remove<int>(i, item.buildingParts);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }else if(item.equipmentType == EquipmentType.Artifact)
                {
                    item.skillType = (SkillType)EditorGUILayout.EnumPopup("Skill Type", item.skillType, GUILayout.Width(pw.mWidth));
                }

                if (EditorGUI.EndChangeCheck())
                    isChanged = true;
                
                //Stat
                EditorGUILayout.BeginVertical("box");
                fold4 = EditorGUILayout.Foldout(fold4, "Stats");
                if (fold4)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Stat", GUILayout.Width(pw.mWidth / 3)))
                    {
                        Stat stat = new Stat();
                        item.stats = DataUtils.Add<Stat>(stat, item.stats);
                    }
                    EditorGUILayout.EndHorizontal();


                    for (int i = 0; i < item.stats.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        item.stats[i].statType = (StatType)EditorGUILayout.EnumPopup(item.stats[i].statType, GUILayout.Width(pw.mWidth / 3));
                        item.stats[i].value = EditorGUILayout.FloatField(item.stats[i].value, GUILayout.Width(pw.mWidth/3*2));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                        {
                            item.stats = DataUtils.Remove<Stat>(i, item.stats);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        if (item.stats.Length > i)
                        {
                            item.stats[i].minValue = EditorGUILayout.FloatField("Min", item.stats[i].minValue, GUILayout.Width(pw.mWidth / 3 * 2));
                            item.stats[i].maxValue = EditorGUILayout.FloatField("Max", item.stats[i].maxValue, GUILayout.Width(pw.mWidth / 3 * 2));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (EditorGUI.EndChangeCheck())
                        isChanged = true;
                }
                EditorGUILayout.EndVertical();

                /*
                //Skill
                EditorGUILayout.BeginVertical("box");
                fold5 = EditorGUILayout.Foldout(fold5, "Skills");
                if (fold5)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Skill", GUILayout.Width(pw.mWidth / 3)))
                    {
                        Skill skill = new Skill();
                        item.skills = DataUtils.Add<Skill>(skill, item.skills);
                    }
                    EditorGUILayout.EndHorizontal();


                    for (int i = 0; i < item.skills.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        item.skills[i].skillType = (SkillType)EditorGUILayout.EnumPopup(item.skills[i].skillType, GUILayout.Width(pw.mWidth / 3));
                        item.skills[i].value = EditorGUILayout.FloatField(item.skills[i].value, GUILayout.Width(pw.mWidth / 3 * 2));
                        if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                        {
                            item.skills = DataUtils.Remove<Skill>(i, item.skills);
                        }
                        EditorGUILayout.EndHorizontal();

                        if (item.skills.Length > i)
                        {
                            EditorGUILayout.BeginHorizontal();
                            item.skills[i].minValue = EditorGUILayout.FloatField("Min", item.skills[i].minValue, GUILayout.Width(pw.mWidth / 3 * 2));
                            item.skills[i].maxValue = EditorGUILayout.FloatField("Max", item.skills[i].maxValue, GUILayout.Width(pw.mWidth / 3 * 2));
                            EditorGUILayout.EndHorizontal();
                        }

                    }
                    if (EditorGUI.EndChangeCheck())
                        isChanged = true;
                }
                EditorGUILayout.EndVertical();
                */

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
			this.EndTab();


            
        }
	}
}
