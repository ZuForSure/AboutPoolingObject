using System.Collections.Generic;
using UnityEditor;
using System.IO;
using MCP.DataModels.BaseModels;
using MCP.DataModel.Utils;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using TMPro;
using UnityEngine.UI;

namespace MCP.Editor
{
    public class MCPToolEditor
    {
        

        [MenuItem("MCP/Remove Missing Script Entries")]
        private static void FindAndRemoveMissingInSelected()
        {
            var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            int compCount = 0;
            int goCount = 0;
            foreach (var o in deepSelection)
            {
                if (o is GameObject go)
                {
                    int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                    if (count > 0)
                    {
                        // Edit: use undo record object, since undo destroy wont work with missing
                        Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        compCount += count;
                        goCount++;
                    }
                }
            }
            Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
        }

        [MenuItem("MCP/FixData")]
        private static void FixData()
        {
            DataHolder.Instance().Init();
            DataHolder.Instance().FixDatas();
            DataHolder.Instance().SaveAll();
            AssetDatabase.Refresh();
        }

        
        #region Set TextMeshPro Text Size to 0
        /*
        [MenuItem("MCP/Set TextMeshPro Text Size to 0")]
        public static void SetTextMeshProTextSizeToZero()
        {
            string folderPath = "Assets/Game/Character/Pets";
            string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"Prefab not found at path: {prefabPath}");
                    continue;
                }

                bool prefabModified = false;
                TextMeshPro[] textComponents = prefab.GetComponentsInChildren<TextMeshPro>(true);

                foreach (var textComponent in textComponents)
                {
                    if (textComponent.fontSize != 0)
                    {
                        textComponent.fontSize = 0;
                        prefabModified = true;
                    }
                }

                if (prefabModified)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                    Debug.Log($"Modified and saved prefab: {prefab.name}");
                }
            }

            Debug.Log("All prefabs processed.");


            folderPath = "Assets/Game/Character/BodyPrefabs";
            prefabPaths = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"Prefab not found at path: {prefabPath}");
                    continue;
                }

                bool prefabModified = false;
                TextMeshPro[] textComponents = prefab.GetComponentsInChildren<TextMeshPro>(true);

                foreach (var textComponent in textComponents)
                {
                    if (textComponent.fontSize != 0)
                    {
                        textComponent.fontSize = 0;
                        prefabModified = true;
                    }
                }

                if (prefabModified)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                    Debug.Log($"Modified and saved prefab: {prefab.name}");
                }
            }

            Debug.Log("All prefabs processed.");
        }
        */
        #endregion

        #region AddEventTime
        /*
        [MenuItem("MCP/Add Event Time")]
        public static void AddEventTime()
        {
            DataHolder.Instance().Init();

            //Set id
            int eventTarget = 10;
            

            var gameEvent = DataHolder.Instance().GetData<GameEvent>(eventTarget);
            gameEvent.haveTime = true;

            gameEvent.gameEventTimes = new GameEventTime[23];


            for (int i = 0; i < 23; i++)
            {
                if (i < 10)
                {
                    gameEvent.gameEventTimes[i] = new GameEventTime()
                    {
                        dayOfWeek = -1,
                        timeStart = $"0{i}:00:00.000Z",
                        timeEnd = $"0{i}:{30}:00.000Z"
                    };
                }
                else
                {
                    gameEvent.gameEventTimes[i] = new GameEventTime()
                    {
                        dayOfWeek = -1,
                        timeStart = $"{i}:00:00.000Z",
                        timeEnd = $"{i}:{30}:00.000Z"
                    };
                }

            }

            DataHolder.Instance().SaveData<GameEvent>();
            AssetDatabase.Refresh();

            Debug.Log("Game Event " + gameEvent.gameEventId + "Add Time completed");
        }

        */
        #endregion

       

        /*
        [MenuItem("MCP/Change TMP to Text")]
        private static void ChangeToNormalText()
        {
            string fontPath = "Assets/Game/UI/Font/SVN-Determination Sans.ttf";

            Font loadedFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (loadedFont == null)
            {
                Debug.Log("NO FOnt");
            }
            else
                Debug.Log(loadedFont.name);
            var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            Debug.Log(deepSelection.Length);
            foreach (var o in deepSelection)
            {
                if (o is GameObject go)
                { 
                    TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
                    if(text != null)
                    {
                        
                        string t = text.text;
                        Color c = text.color;
                        float size = text.fontSize;
                        bool isBestfit = text.enableAutoSizing;
                        TextAlignment textAlignment = (TextAlignment)text.alignment;
                        GameObject.DestroyImmediate(text);
                        if(go.transform.childCount > 0)
                        {
                            Transform child = go.transform.GetChild(0);
                            if (child != null)
                                GameObject.DestroyImmediate(child.gameObject);
                        }

                        Text newText = go.AddComponent<Text>();
                        newText.text = t;
                        newText.fontSize = (int)size;
                        newText.color = c;
                        newText.alignment = TextAnchor.MiddleCenter;
                        newText.resizeTextForBestFit = isBestfit;
                        newText.resizeTextMaxSize = (int)size;
                        newText.font = Font.CreateDynamicFontFromOSFont("Minecraft.ttf", (int)size); ;
                        EditorUtility.SetDirty(newText);
                        Debug.Log("Change TExt success " + newText.text);
                    }

                    Text textUI = go.GetComponent<Text>();
                    if (textUI != null)
                    {
                        textUI.font = loadedFont;
                        EditorUtility.SetDirty(textUI);
                        Debug.Log("Change font success " + textUI.text);
                    }
                }
            }

            //GameObject selectedPrefabInstance = Selection.activeGameObject;
            //PrefabUtility.ApplyPrefabInstance(selectedPrefabInstance, InteractionMode.UserAction);

        }
        */

        /*
        [MenuItem("MCP/Change Font")]
        private static void ChangeFont()
        {
            string fontPath = "Assets/Game/UI/Font/FVF Fernando 08.ttf";

            Font loadedFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (loadedFont == null)
            {
                Debug.Log("NO FOnt");
            }
            else
                Debug.Log(loadedFont.name);
            var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
            Debug.Log(deepSelection.Length);
            foreach (var o in deepSelection)
            {
                if (o is GameObject go)
                {

                    Text textUI = go.GetComponent<Text>();
                    if (textUI != null)
                    {
                        textUI.font = loadedFont;
                        EditorUtility.SetDirty(textUI);
                        Debug.Log("Change font success " + textUI.text);
                    }
                }
            }

            GameObject selectedPrefabInstance = Selection.activeGameObject;
            PrefabUtility.ApplyPrefabInstance(selectedPrefabInstance, InteractionMode.UserAction);

        }
        /*

        /*
        [MenuItem("MCP/Fix Dialog")]
        private static void FixDialogs()
        {
            for(int i = 0; i < DataHolder.Instance().GetDataCount<Dialog>(); i++)
            {
                DataHolder.Instance().GetData<Dialog>(i).SetName(0, DataHolder.Instance().GetData<Dialog>(i).GetName(0).Replace("-", "\n-"));
                DataHolder.Instance().GetData<Dialog>(i).isTranslateName = true;
            }

            DataHolder.Instance().SaveData<Dialog>();

            AssetDatabase.Refresh();
        }
        */

        /*
        [MenuItem("MCP/Create Equipments")]
        public static void CreateEquipments()
        {
            DataHolder.Instance().Init();
            foreach(EquipmentType equipmentType in System.Enum.GetValues(typeof(EquipmentType)))
            {
                int maxid = 3;
                if (equipmentType == EquipmentType.Hair)
                {
                    maxid = 10;
                }
                else if (equipmentType == EquipmentType.FaceHair)
                {
                    maxid = 6;
                }
                else if (equipmentType == EquipmentType.Helmet)
                {
                    maxid = 3;
                }
                else if (equipmentType == EquipmentType.Cloth)
                {
                    maxid = 3;
                }
                else if (equipmentType == EquipmentType.Boot)
                {
                    maxid = 3;
                }
                else if (equipmentType == EquipmentType.Back)
                {
                    maxid = 3;
                }
                else if (equipmentType == EquipmentType.Weapons)
                {
                    maxid = 3;
                }
                else if (equipmentType == EquipmentType.Item)
                {
                    maxid = 1;
                }
                else if (equipmentType == EquipmentType.Body)
                {
                    maxid = 1;
                }
                else
                    continue;




                for (int i = 0; i < maxid; i++)
                {
                    Card card = DataHolder.Instance().AddData<Card>();
                    card.SetName(0, equipmentType.ToString() + "_" + i.ToString());
                    card.equipmentType = equipmentType;
                    card.isStackable = false;
                    card.iconUrl = "Icons/Items/" + ((int)equipmentType).ToString() + "_" + equipmentType.ToString() + "/" + equipmentType.ToString() + "_" + i.ToString();

                    CurrencyData currencyData = new CurrencyData();
                    currencyData.currencyId = 1;
                    currencyData.value = 10;
                    card.cost = DataUtils.Add(currencyData,card.cost);

                    if (equipmentType == EquipmentType.Boot)
                    {
                        card.canUpgrade = true;
                        if (i == 0)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 2;
                            stat1.maxValue = 20;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.25f;
                            stat2.maxValue = 2.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 1)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 4;
                            stat1.maxValue = 40;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.15f;
                            stat2.maxValue = 1.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 2)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 2;
                            stat1.maxValue = 20;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.15f;
                            stat2.maxValue = 1.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 2;
                            stat3.maxValue = 20;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }



                    }
                    else if (equipmentType == EquipmentType.Helmet)
                    {
                        card.canUpgrade = true;
                        if (i == 0)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 20;
                            stat1.maxValue = 200;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 1)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 10;
                            stat1.maxValue = 100;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 2;
                            stat3.maxValue = 20;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 2)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 10;
                            stat1.maxValue = 100;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.1f;
                            stat2.maxValue = 1f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }

                    }
                    else if (equipmentType == EquipmentType.Cloth)
                    {
                        card.canUpgrade = true;
                        if (i == 0)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 20;
                            stat1.maxValue = 200;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 1)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 10;
                            stat1.maxValue = 100;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 2;
                            stat3.maxValue = 20;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 2)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 10;
                            stat1.maxValue = 100;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.1f;
                            stat2.maxValue = 1f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                    }
                    else if (equipmentType == EquipmentType.Weapons)
                    {
                        card.canUpgrade = true;
                        if (i == 0)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 2;
                            stat1.maxValue = 20;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 5;
                            stat3.maxValue = 50;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 1)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 4;
                            stat1.maxValue = 40;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 3;
                            stat3.maxValue = 30;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 2)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 2;
                            stat1.maxValue = 20;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.1f;
                            stat2.maxValue = 1f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 3;
                            stat3.maxValue = 30;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                    }
                    else if (equipmentType == EquipmentType.Back)
                    {
                        card.canUpgrade = true;
                        if (i == 0)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 4;
                            stat1.maxValue = 40;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 1)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 2;
                            stat1.maxValue = 20;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.05f;
                            stat2.maxValue = 0.5f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 2;
                            stat3.maxValue = 20;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                        else if (i == 2)
                        {
                            Stat stat1 = new Stat();
                            stat1.statType = StatType.Health;
                            stat1.minValue = 2;
                            stat1.maxValue = 20;
                            card.stats = DataUtils.Add(stat1, card.stats);

                            Stat stat2 = new Stat();
                            stat2.statType = StatType.Speed;
                            stat2.minValue = 0.1f;
                            stat2.maxValue = 1f;
                            card.stats = DataUtils.Add(stat2, card.stats);

                            Stat stat3 = new Stat();
                            stat3.statType = StatType.Damage;
                            stat3.minValue = 1;
                            stat3.maxValue = 10;
                            card.stats = DataUtils.Add(stat3, card.stats);
                        }
                    }
                    else if (equipmentType == EquipmentType.Body)
                    {

                        Stat stat1 = new Stat();
                        stat1.statType = StatType.Health;
                        stat1.minValue = 100;
                        stat1.maxValue = 100;
                        card.stats = DataUtils.Add(stat1, card.stats);

                        Stat stat2 = new Stat();
                        stat2.statType = StatType.Speed;
                        stat2.minValue = 3f;
                        stat2.maxValue = 3f;
                        card.stats = DataUtils.Add(stat2, card.stats);

                        Stat stat3 = new Stat();
                        stat3.statType = StatType.Damage;
                        stat3.minValue = 20;
                        stat3.maxValue = 20;
                        card.stats = DataUtils.Add(stat3, card.stats);
                    }
                    else if (equipmentType == EquipmentType.Item)
                    {
                        card.canUpgrade = true;
                        Stat stat = new Stat();
                        stat.statType = StatType.Health;
                        stat.minValue = 10;
                        stat.maxValue = 100;
                        card.stats = DataUtils.Add(stat, card.stats);
                    }

                    if (card.canUpgrade)
                    {
                        CurrencyData upgradeCurrency = new CurrencyData();
                        upgradeCurrency.currencyId = 1;
                        if (equipmentType == EquipmentType.Item)
                            upgradeCurrency.value = 5;
                        else
                            upgradeCurrency.value = 10;
                        card.upgradeCost = DataUtils.Add(upgradeCurrency, card.cost);
                        card.upgradeLevelRate = 1.2f;
                    }
                }
            }

            DataHolder.Instance().SaveData<Card>();

            AssetDatabase.Refresh();
        }*/


        #region Create Totem

        /*
        [MenuItem("MCP/Create Totem")]
        public static void Totem()
        {
            DataHolder.Instance().Init();

            for (int i = 0; i < 36; i++)
            {
                Card card = DataHolder.Instance().AddData<Card>();
                card.SetName(0, "Totem_" + i.ToString());
                card.isTradable = true;
                card.canUpgrade = true;
                card.canEquip = true;
                card.canSell = true;
                card.canDrop = true;
                card.useForCraft = false;
                CurrencyData currencyData = new CurrencyData();
                currencyData.currencyId = 2;
                currencyData.value = 50;
                card.cost = DataUtils.Add(currencyData, card.cost);
                CurrencyData upgradeCost = new CurrencyData();
                upgradeCost.currencyId = 2;
                upgradeCost.value = 20;
                card.upgradeCost = DataUtils.Add(upgradeCost, card.upgradeCost);
            }

            DataHolder.Instance().SaveData<Card>();

            AssetDatabase.Refresh();
        }
        */

        #endregion

        #region Fix Quest
        /*
        [MenuItem("MCP/Fix Quest")]
        public static void FixQuest()
        {
            DataHolder.Instance().Init();
            Quest refQuest = DataHolder.Instance().GetData<Quest>(22);

            for (int i = 23; i < 44; i++)
            {
                Quest quest = DataHolder.Instance().GetData<Quest>(i);
                quest.stats = new Stat[0];
                if (quest.equipmentType == EquipmentType.Back || quest.equipmentType == EquipmentType.Weapons || quest.equipmentType == EquipmentType.Helmet
                    || quest.equipmentType == EquipmentType.Cloth || quest.equipmentType == EquipmentType.Boot)
                {
                    foreach (Stat stat in refQuest.stats)
                    {
                        Stat stat1 = new Stat(stat);
                        quest.stats = DataUtils.Add(stat1, quest.stats);
                    }
                }
                else
                {
                    if (quest.equipmentType == EquipmentType.Horse || quest.equipmentType == EquipmentType.Pet)
                    {
                        foreach (Stat stat in refQuest.stats)
                        {
                            Stat stat1 = new Stat(stat);
                            stat1.minValue *= 1.4f;
                            stat1.maxValue *= 1.4f;
                            quest.stats = DataUtils.Add(stat1, quest.stats);
                        }
                    }
                    else
                    {
                        foreach (Stat stat in refQuest.stats)
                        {
                            Stat stat1 = new Stat(stat);
                            quest.stats = DataUtils.Add(stat1, quest.stats);
                        }
                    }
                }
            }

            DataHolder.Instance().SaveData<Quest>();
            AssetDatabase.Refresh();
        }

        */
        #endregion

        [MenuItem("MCP/Fix Collections")]
        public static void FixCollections()
        {
            DataHolder.Instance().Init();

            for (int i = 0; i < DataHolder.Instance().GetDataCount<Card>(); i++)
            {
                Card card = DataHolder.Instance().GetData<Card>(i);
                if (card.IsCollection())
                {
                    card.useForCraft = true;
                    card.canDrop = false;
                    card.isTradable = true;
                    Debug.Log("Fix Card " + i);
                }
            }
            DataHolder.Instance().SaveData<Card>();
            AssetDatabase.Refresh();
            Debug.Log("Fix Collections Completed");
        }

        [MenuItem("MCP/Add Collections")]
        public static void AddCollections()
        {
            DataHolder.Instance().Init();

            //Attack
            for (int n = 12; n < 18; n++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Card card = DataHolder.Instance().AddData<Card>();
                    card.SetName(0, "Skin Horse " + n.ToString() + " Attack_" + (i + 1).ToString());
                    if (n == 12)
                        card.SetDescription(0, "Change attack animation when riding a Bear.");
                    else if (n == 13)
                        card.SetDescription(0, "Change attack animation when riding a Rhino.");
                    else if (n == 14)
                        card.SetDescription(0, "Change attack animation when riding a Elephant.");
                    else if (n == 15)
                        card.SetDescription(0, "Change attack animation when riding a Ox.");
                    else if (n == 16)
                        card.SetDescription(0, "Change attack animation when riding a Turtle.");
                    else if (n == 17)
                        card.SetDescription(0, "Change attack animation when riding a Gorilla.");

                    card.equipmentType = EquipmentType.SkinAttack;
                    card.canDrop = false;
                    card.useForCraft = true;
                    card.craftOutputIds = new int[] { 651 + n };
                    card.canEquip = true;
                    card.canSell = true;

                    CurrencyData currencyData = new CurrencyData();
                    currencyData.currencyId = 4;
                    currencyData.value = 200;

                    card.cost = new CurrencyData[] { currencyData };
                }

                for (int i = 0; i < 2; i++)
                {
                    Card card = DataHolder.Instance().AddData<Card>();
                    card.SetName(0, "Skin Horse " + n.ToString() + " Idle_" + (i + 1).ToString());
                    if (n == 12)
                        card.SetDescription(0, "Change attack animation when riding a Bear.");
                    else if (n == 13)
                        card.SetDescription(0, "Change attack animation when riding a Rhino.");
                    else if (n == 14)
                        card.SetDescription(0, "Change attack animation when riding a Elephant.");
                    else if (n == 15)
                        card.SetDescription(0, "Change attack animation when riding a Ox.");
                    else if (n == 16)
                        card.SetDescription(0, "Change attack animation when riding a Turtle.");
                    else if (n == 17)
                        card.SetDescription(0, "Change attack animation when riding a Gorilla.");

                    card.equipmentType = EquipmentType.SkinIdle;
                    card.canDrop = false;
                    card.useForCraft = true;
                    card.craftOutputIds = new int[] { 651 + n };
                    card.canEquip = true;
                    card.canSell = true;

                    CurrencyData currencyData = new CurrencyData();
                    currencyData.currencyId = 4;
                    currencyData.value = 200;

                    card.cost = new CurrencyData[] { currencyData };
                }

                for (int i = 0; i < 4; i++)
                {
                    Card card = DataHolder.Instance().AddData<Card>();
                    card.SetName(0, "Skin Horse " + n.ToString() + " Run_" + (i + 1).ToString());
                    if (n == 12)
                        card.SetDescription(0, "Change attack animation when riding a Bear.");
                    else if (n == 13)
                        card.SetDescription(0, "Change attack animation when riding a Rhino.");
                    else if (n == 14)
                        card.SetDescription(0, "Change attack animation when riding a Elephant.");
                    else if (n == 15)
                        card.SetDescription(0, "Change attack animation when riding a Ox.");
                    else if (n == 16)
                        card.SetDescription(0, "Change attack animation when riding a Turtle.");
                    else if (n == 17)
                        card.SetDescription(0, "Change attack animation when riding a Gorilla.");

                    card.equipmentType = EquipmentType.SkinRun;
                    card.canDrop = false;
                    card.useForCraft = true;
                    card.craftOutputIds = new int[] { 651 + n };
                    card.canEquip = true;
                    card.canSell = true;

                    CurrencyData currencyData = new CurrencyData();
                    currencyData.currencyId = 4;
                    currencyData.value = 200;

                    card.cost = new CurrencyData[] { currencyData };
                }
            }

            DataHolder.Instance().SaveData<Card>();
            AssetDatabase.Refresh();
        }

        [MenuItem("MCP/Fix Viking Equipment")]
        public static void FixVikingEquipment()
        {
            DataHolder.Instance().Init();

            var cards = DataHolder.Instance().GetDatas<Card>();
            List<Card> vikingEquipmentCraftable = new();

            for (int i = 0; i < cards.Length; i++)
            {
                var type = cards[i].equipmentType;

                if (type != EquipmentType.Cloth && type != EquipmentType.Helmet && type != EquipmentType.Boot &&
                    type != EquipmentType.Back && type != EquipmentType.Weapons)
                {
                    continue;
                }

                var cardName = cards[i].GetName(0);

                if (cardName.Contains("Viking_") && cards[i].craftable)
                {
                    vikingEquipmentCraftable.Add(cards[i]);
                }
            }

            foreach (var item in vikingEquipmentCraftable)
            {
                var spilit = item.GetName(0).Split('_');

                if (spilit[^1] == "1")
                {
                    // Logic tier 1
                }
                else if (spilit[^1] == "2")
                {
                    // Logic tier 2

                }
                else if (spilit[^1] == "3")
                {
                    // Logic tier 3
                }
            }

            DataHolder.Instance().SaveData<Card>();
            AssetDatabase.Refresh();
        }

        public static void CopyFolder(string des, string src, string folderName)
        {
            FileUtil.DeleteFileOrDirectory(des + "/" + folderName);
            FileUtil.CopyFileOrDirectory(src + "/" + folderName, des + "/" + folderName);
        }

        public static string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
                @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
                System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }

        public static void DeleteFileTypeInDirectory(string src, string fileName)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(src);
            foreach (string filePath in fileEntries)
            {
                FileInfo fileInfo = new FileInfo(filePath);

                string[] t = fileInfo.Name.Split('.');

                if (t[t.Length - 1].Contains(fileName.Replace(".", "")))
                {
                    DeleteFile(filePath);
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(src);
            foreach (string subdirectory in subdirectoryEntries)
            {
                DeleteFileTypeInDirectory(subdirectory, fileName);
            }
        }

        // Insert logic for processing found files here.
        public static void DeleteFile(string path)
        {
            //Delete File
            File.Delete(path);
        }
    }
}
