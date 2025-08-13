
using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using MCP.DataModel.Utils;
#if UNITY_2021_1_OR_NEWER
using UnityEngine;
#endif

using MCP.DataModels.Attributes;

namespace MCP.DataModels.BaseModels
{
    public partial class DataHolder
    {
        private static DataHolder instance;

        public Type[] dataNames = new Type[] {
            typeof(Card),
            typeof(Box),
            typeof(Currency),
            typeof(Bundle),
            typeof(Dialog),
            };

        public int[] languageIDs = new int[0];
        Hashtable datas = new Hashtable();

        private DataHolder()
        {
            if (instance != null)
            {
                //Debug.Log("There is already an instance of DataHolder!");
                return;
            }
            instance = this;
#if !UNITY_2017_1_OR_NEWER
            Init();
#endif
        }

        public static DataHolder Instance()
        {
            if (instance == null)
            {
                instance = new DataHolder();
            }

            return instance;
        }

        public void Init()
        {
            LoadLanguage();
            //Load all Other Data
            foreach (Type type in dataNames)
            {
                MethodInfo method = typeof(DataHolder).GetMethod("LoadData", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo generic = method.MakeGenericMethod(type);
                generic.Invoke(this, new object[1] { null });
            }
        }

        public void SaveAll()
        {
            foreach (Type type in dataNames)
            {
                MethodInfo method = typeof(DataHolder).GetMethod("SaveData", BindingFlags.Public | BindingFlags.Instance);
                MethodInfo generic = method.MakeGenericMethod(type);
                generic.Invoke(this, new object[1] { null });
            }
        }

        public void Reload()
        {
            datas.Clear();
            Init();
        }
        public void Reload(string key, string data)
        {
            if (data != "")
            {
                foreach (Type type in dataNames)
                {
                    if (key == type.Name)
                    {
                        MethodInfo method = typeof(DataHolder).GetMethod("LoadData", BindingFlags.NonPublic | BindingFlags.Instance);
                        MethodInfo generic = method.MakeGenericMethod(type);
                        generic.Invoke(this, new object[1] { data });
                    }

                }

            }
        }


        public virtual void SaveData<T>()
        {
            SaveFile(typeof(T).Name, JsonConvert.SerializeObject(datas[typeof(T).ToString()]));
        }

        public void FixDatas()
        {
            foreach (Type type in dataNames)
            {
                MethodInfo method = typeof(DataHolder).GetMethod("FixData", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo generic = method.MakeGenericMethod(type);
                generic.Invoke(this, null);
            }
        }

        void FixData<T>() where T : BaseData
        {
            for (int i = 0; i < Instance().GetDataCount<T>(); i++)
            {
                Instance().GetData<T>(i).id = i;
                //Instance().GetData<T>(i).itemId = Instance().gameConfig.bundleName.ToLower() + typeof(T).Name.ToLower() + "." + i;
            }

            Instance().SaveData<T>();
        }


        void LoadData<T>(string data)
        {
            string text = "";
            if (data != null)
                text = data;
            else
                text = LoadFile(typeof(T).Name);
            if (text != "")
            {
                T[] items = DataUtils.getJsonArray<T>(text);
                if (!datas.ContainsKey(typeof(T).ToString()))
                    datas.Add(typeof(T).ToString(), items);
                else datas[typeof(T).ToString()] = items;
            }
            else
            {
                T[] items = new T[0];
                if (!datas.ContainsKey(typeof(T).ToString()))
                    datas.Add(typeof(T).ToString(), items);
            }
        }

        public void ExportLanguage()
        {
            string exportData = "";

            //Header
            exportData += "ID;";
            for (int i = 0; i < DataHolder.Instance().languageIDs.Length; i++)
            {
                exportData += DataHolder.LanguageName[DataHolder.Instance().languageIDs[i]];
                if (i < DataHolder.Instance().languageIDs.Length - 1)
                {
                    exportData += ";";
                }
                else
                    exportData += "\n";
            }

            //exportData += Instance().ExportLanguage(Instance().GetDatas<Stat>(), true, false);
            exportData += Instance().ExportLanguage(Instance().GetDatas<Card>(), false, true);
            //exportData += Instance().ExportLanguage(Instance().GetDatas<Item>(), false, true);
            exportData += Instance().ExportLanguage(Instance().GetDatas<Dialog>(), true, false);
            //exportData += Instance().ExportLanguage(Instance().GetDatas<Quest>(), true, true);
            //exportData += Instance().ExportLanguage(Instance().GetDatas<Dialog>(), true, false);
            //exportData += Instance().ExportLanguage(Instance().GetDatas<Notification>(), true, true);
            //exportData += Instance().ExportLanguage(Instance().GetDatas<Message>(), true, true);
            //exportData += Instance().ExportLanguage(Instance().GetDatas<GameplayConfig>(), false, true);
            SaveFile("ExportLanguage", exportData, "/Game/ExportDatas/");
        }

        
        string ExportLanguage(BaseData[] datas, bool isTranslateName, bool isTranslateDescription)
        {
            string text = "";
            if (datas != null)
            {
                for (int i = 0; i < datas.Length; i++)
                {


                    if (datas[i].GetName(0) != "" && isTranslateName)
                    {
                        //text += datas[i].itemId + "_" + "Name" + ";";

                        for (int j = 0; j < DataHolder.Instance().languageIDs.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(datas[i].GetName(j)))
                                text += datas[i].GetName(j);
                            if (j < DataHolder.Instance().languageIDs.Length - 1)
                            {
                                text += ";";
                            }
                        }

                        text += "\n";
                    }

                    if (datas[i].GetDescription(0) != "" && isTranslateDescription)
                    {
                        //text += datas[i].itemId + "_" + "Description" + ";";
                        for (int j = 0; j < DataHolder.Instance().languageIDs.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(datas[i].GetDescription(j)))
                                text += datas[i].GetDescription(j);
                            if (j < DataHolder.Instance().languageIDs.Length - 1)
                            {
                                text += ";";
                            }
                        }
                        text += "\n";
                    }


                }
            }

            return text;
        }

        public void ImportLanguage(string text)
        {

            foreach (Type type in dataNames)
            {
                if (type.IsSubclassOf(typeof(BaseData)))
                {
                    MethodInfo method = typeof(DataHolder).GetMethod("ImportLanguage", BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo generic = method.MakeGenericMethod(type);
                    generic.Invoke(this, new object[1] { text });
                }
            }

        }
        /*
        void ImportLanguage<T>(string text) where T : BaseData
        {
            string[] lines = text.Split("\n"[0]);
            T[] datas = Instance().GetDatas<T>();
            if (datas != null && datas.Length > 0)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    for (int l = 1; l < lines.Length; l++)
                    {
                        string[] chars = DataUtils.SplitCsvLine(lines[l]); ;
                        if (chars[0] == datas[i].itemId + "_Name")
                        {
                            for (int j = 0; j < Instance().languageIDs.Length; j++)
                            {
                                if (j + 1 < chars.Length)
                                {
                                    datas[i].SetName(j, chars[j + 1]);
                                }
                            }
                        }
                        else if (chars[0] == datas[i].itemId + "_Description")
                        {
                            for (int j = 0; j < Instance().languageIDs.Length; j++)
                            {
                                if (j + 1 < chars.Length)
                                {
                                    datas[i].SetDescription(j, chars[j + 1]);
                                }
                            }
                        }
                    }
                }
            }

        }*/

        public string LoadFile(string fileName)
        {
#if UNITY_2021_1_OR_NEWER
            TextAsset textFile = Resources.Load("Data/" + fileName) as TextAsset;
            if (textFile != null)
                return textFile.text;
            else
                return "";
#else
            if (File.Exists(DataUtils.GetLocalRootPath() + "/Data/" + fileName + ".txt"))
                return File.ReadAllText(DataUtils.GetLocalRootPath() + "/Data/" + fileName + ".txt");
            else
                return "";
#endif
        }

        public void SaveFile(string fileName, string text, string path = "/Game/Resources/Data/")
        {
#if UNITY_2021_1_OR_NEWER
            string savePath = Application.dataPath + path + fileName + ".txt";
            if (!File.Exists(savePath))
            {
                FileStream file = File.Open(savePath, FileMode.Create);
                file.Close();
            }

            File.Delete(savePath);
            File.WriteAllText(savePath, text);
#endif
        }



        public T[] GetDatas<T>()
        {
            if (datas.Contains(typeof(T).ToString()))
            {
                return (T[])datas[typeof(T).ToString()];
            }
            else
                return null;
        }

        public T[] GetDatas<T>(string keySearch) where T : BaseData
        {
            List<T> searchs = new List<T>();
            T[] list = GetDatas<T>();
           
            if (list != null)
            {
                foreach (T t in list)
                {
                    if (keySearch == "" ||
                        (t.languageItem != null && t.languageItem.Length > 0 && t.languageItem[0].Name.ToLower().Contains(keySearch.ToLower())))
                        searchs.Add(t);
                }
            }

            return searchs.ToArray();
        }

        public T GetData<T>(int index)
        {
            if (GetDatas<T>() != null && GetDatas<T>().Length > index && index >= 0)
                return (T)(GetDatas<T>()[index]);
            else
                return default(T);
        }

        public T GetData<T>(int index, string keySearch) where T : BaseData
        {
            T[] searchs = GetDatas<T>(keySearch);
            if (searchs != null && searchs.Length > index && index >= 0)
                return (T)(searchs[index]);
            else
                return default(T);
        }

        public Dialog GetDialog(int index, string keySearch, DialogType dialogType)
        {
            Dialog[] searchs = GetDialogs(keySearch, dialogType);
            if (searchs != null && searchs.Length > index && index >= 0)
                return (searchs[index]);
            else
                return null;
        }

        public int GetIdFromName<T>(string n) where T : BaseData
        {
            T[] list = GetDatas<T>();
            if (list != null)
            {
                foreach (T t in list)
                {
                    if (String.Equals(t.GetName(0), n, StringComparison.InvariantCultureIgnoreCase))
                        return t.Id();
                }
            }

            return -1;
        }

        public int GetDataCount<T>()
        {
            if (GetDatas<T>() != null)
                return GetDatas<T>().Length;
            else
                return 0;
        }

        public int GetDataCount<T>(string keySearch) where T : BaseData
        {
            if (GetDatas<T>(keySearch) != null)
                return GetDatas<T>(keySearch).Length;
            else
                return 0;
        }

        public int GetDialogCount(string keySearch, DialogType dialogType)
        {
            if (GetDialogNameSearchList(keySearch) != null)
                return GetDialogNameSearchList(keySearch, dialogType).Length;
            else
                return 0;
        }

        public T AddData<T>()
        {
            T[] list = GetDatas<T>();
            object data = Activator.CreateInstance(typeof(T));
            list = DataUtils.Add<T>((T)data, list);
            datas[typeof(T).ToString()] = list;
            return (T)data;
        }

        public void RemoveData<T>(int index)
        {
            T[] list = GetDatas<T>();
            if (list.Length > index)
            {
                list = DataUtils.Remove<T>(index, list);
                datas[typeof(T).ToString()] = list;
            }
        }

        public void RemoveAllData<T>()
        {
            if (datas.Contains(typeof(T).ToString()))
            {
                datas[typeof(T).ToString()] = null;
            }
        }

        public void CopyData<T>(int index) where T : BaseData
        {
            T[] list = GetDatas<T>();
            T data = GetData<T>(index);
            T newData = (T)Activator.CreateInstance(typeof(T));
            DataUtils.CopyObject(newData, data);
            newData.Init<T>();
            Debug.Log(index);
            //string[] count = data.GetName(0).Split("_");
            //int newIndex = int.Parse(count[count.Length - 1]) + 1;
            //newData.SetName(0, count[0] + "_" + newIndex);
            newData.SetName(0, data.GetName(0) + " Copy");
            newData.SetDescription(0, data.GetDescription(0));
            list = DataUtils.Add<T>((T)newData, list);
            datas[typeof(T).ToString()] = list;
        }

        public string[] GetDataNameList<T>() where T : BaseData
        {
            List<string> names = new List<string>();
            T[] list = GetDatas<T>();
            if (list != null)
            {
                int index = 0;
                foreach (T t in list)
                {
                    names.Add(index + "_" + t.GetName(0));
                    index++;
                }
            }
            return names.ToArray();
        }

        /*
        public string[] GetDataNameList<T>(EquipmentType equipmentType) where T : Card
        {
            List<string> names = new List<string>();
            T[] list = GetDatas<T>();
            if (list != null)
            {
                int index = 0;
                foreach (T t in list)
                {   
                    if(t.equipmentType == equipmentType)
                    {
                        names.Add(index + "_" + t.GetName(0));
                        index++;
                    }
                }
            }
            return names.ToArray();
        }*/

        public string[] GetDataNameSearchList<T>(string keySearch) where T : BaseData
        {
            List<string> names = new List<string>();
            T[] list = GetDatas<T>();
            if (list != null)
            {
                int index = 0;
                foreach (T t in list)
                {
                    if (keySearch == "" ||
                        (t.languageItem != null && t.languageItem.Length > 0 && t.languageItem[0].Name.ToLower().Contains(keySearch.ToLower())))
                        names.Add(index + "_" + t.GetName(0));
                    index++;
                }
            }

            return names.ToArray();
        }
        public string[] GetDialogNameSearchList(string keySearch)
        {
            List<string> names = new List<string>();
            Dialog[] list = GetDatas<Dialog>();
            if (list != null)
            {
                int index = 0;
                foreach (Dialog t in list)
                {
                    if (t.dialogType == DialogType.Dialog && (keySearch == "" ||
                        (t.languageItem != null && t.languageItem.Length > 0 && t.languageItem[0].Name.ToLower().Contains(keySearch.ToLower()))))
                        names.Add(index + "_" + t.GetName(0));
                    index++;
                }
            }

            return names.ToArray();
        }
        public Dialog GetDialog(int index, string keySearch)
        {
            Dialog[] searchs = GetDialogs(keySearch);
            if (searchs != null && searchs.Length > index && index >= 0)
                return (Dialog)(searchs[index]);
            else
                return default(Dialog);
        }


        public Dialog[] GetDialogs(string keySearch)
        {
            List<Dialog> searchs = new List<Dialog>();
            Dialog[] list = GetDatas<Dialog>();
            if (list != null)
            {
                foreach (Dialog t in list)
                {
                    if (t.dialogType == DialogType.Dialog && (keySearch == "" ||
                        (t.languageItem != null && t.languageItem.Length > 0 && t.languageItem[0].Name.ToLower().Contains(keySearch.ToLower()))))
                        searchs.Add(t);
                }
            }

            return searchs.ToArray();
        }

        public Dialog[] GetDialogs(string keySearch, DialogType dialogType)
        {
            List<Dialog> searchs = new List<Dialog>();
            Dialog[] list = GetDatas<Dialog>();
            if (list != null)
            {
                foreach (Dialog t in list)
                {
                    if ((t.dialogType == dialogType || dialogType == DialogType.All) && (keySearch == "" ||
                        (t.languageItem != null && t.languageItem.Length > 0 && t.languageItem[0].Name.ToLower().Contains(keySearch.ToLower()))))
                        searchs.Add(t);
                }
            }

            return searchs.ToArray();
        }

        public string[] GetDialogNameSearchList(string keySearch, DialogType dialogType)
        {
            List<string> names = new List<string>();
            Dialog[] list = GetDatas<Dialog>();
            if (list != null)
            {
                int index = 0;
                foreach (Dialog t in list)
                {
                    if ((t.dialogType == dialogType || dialogType == DialogType.All) && (keySearch == "" ||
                        (t.languageItem != null && t.languageItem.Length > 0 && t.languageItem[0].Name.ToLower().Contains(keySearch.ToLower()))))
                        names.Add(index + "_" + t.GetName(0));
                    index++;
                }
            }

            return names.ToArray();
        }

        public string[] GetDataNameListByItemClass(ItemClass itemClass)
        {
            foreach (Type type in dataNames)
            {
                if (type.Name.Contains(itemClass.ToString()))
                {
                    MethodInfo method = typeof(DataHolder).GetMethod("GetDataNameList");
                    MethodInfo generic = method.MakeGenericMethod(type);
                    return (string[])generic.Invoke(this, null);
                }
            }

            return null;
        }

        public List<Card> GetCards(RareType rareType)
        {
            List<Card> cards = new List<Card>();

            foreach (Card card in DataHolder.Instance().GetDatas<Card>())
            {
                if (card.isAvailable && card.rareType == rareType)
                {
                    cards.Add(card);
                }

            }
            return cards;
        }

        public List<Card> GetCards(EquipmentType equipmentType)
        {
            List<Card> cards = new List<Card>();

            foreach (Card card in DataHolder.Instance().GetDatas<Card>())
            {
                if (card.isAvailable && card.equipmentType == equipmentType)
                {
                    cards.Add(card);
                }

            }
            return cards;
        }

        public List<int> GetCardIds(EquipmentType equipmentType)
        {
            List<int> ids = new List<int>();

            foreach (Card card in DataHolder.Instance().GetDatas<Card>())
            {
                if (card.isAvailable && card.equipmentType == equipmentType)
                {
                    ids.Add(card.Id());
                }

            }
            return ids;
        }



        public List<int> GetNonDropCardIds(EquipmentType equipmentType)
        {
            List<int> ids = new List<int>();

            foreach (Card card in DataHolder.Instance().GetDatas<Card>())
            {
                if (card.isAvailable && card.equipmentType == equipmentType && !card.canDrop)
                {
                    ids.Add(card.Id());
                }

            }
            return ids;
        }

        public List<int> GetCraftableCardIds(EquipmentType equipmentType)
        {
            List<int> ids = new List<int>();

            foreach (Card card in DataHolder.Instance().GetDatas<Card>())
            {
                if (card.isAvailable && card.equipmentType == equipmentType && card.craftable)
                {
                    ids.Add(card.Id());
                }

            }
            return ids;
        }



        public List<int> GetCraftableCards()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < DataHolder.Instance().GetDataCount<Card>(); i++)
            {
                Card card = DataHolder.Instance().GetData<Card>(i);
                if (card.isAvailable && card.craftable)
                {
                    ids.Add(i);
                }
            }
            return ids;
        }

        public List<Card> GetAvailableCards()
        {
            List<Card> cards = new List<Card>();
            Card[] allCards = GetDatas<Card>();
            foreach (Card card in allCards)
            {
                if (card.isAvailable)
                    cards.Add(card);
            }
            return cards;
        }


        public Card GetRandomCard(RareType rareType, int seed)
        {
            List<Card> cards = GetCards(rareType);
            if (cards.Count > 0)
            {
                System.Random RNG = new System.Random(seed);
                int k = RNG.Next(0, cards.Count);
                return cards[k];
            }
            else
                return null;
        }

        public Card GetRandomCard(int seed)
        {
            System.Random RNG = new System.Random(seed);
            int k = RNG.Next(0, 10000);
            if (k < 8900)
                return GetRandomCard(RareType.Common, seed);
            else if (k < 9900)
                return GetRandomCard(RareType.Rare, seed);
            else if (k < 9980)
                return GetRandomCard(RareType.Epic, seed);
            else
                return GetRandomCard(RareType.Legendary, seed);
        }

        

        //Currency
        public int GetCurrencyId(string code)
        {
            Currency[] currencies = GetDatas<Currency>();
            for (int i = 0; i < currencies.Length; i++)
            {
                if (currencies[i].Code == code)
                    return i;
            }

            return 0;
        }

        public int GetCurrencyIdByName(string n)
        {
            Currency[] currencies = GetDatas<Currency>();
            for (int i = 0; i < currencies.Length; i++)
            {
                if (String.Equals(currencies[i].GetName(0), n, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }

            return 0;
        }

        #region Language

        //Language
        public void AddLanguage()
        {
            languageIDs = DataUtils.Add<int>(0, languageIDs);

            //add language to other datas
            foreach (Type type in dataNames)
            {
                MethodInfo method = typeof(DataHolder).GetMethod("AddDataLanguage");
                MethodInfo generic = method.MakeGenericMethod(type);
                generic.Invoke(this, null);
            }
        }

        public void AddDataLanguage<T>() where T : BaseData
        {
            T[] list = GetDatas<T>();
            if (list != null)
            {
                foreach (T t in list)
                {
                    t.AddLanguageItem();
                }
            }
        }

        public void RemoveLanguage(int index)
        {
            if (languageIDs.Length > index)
                languageIDs = DataUtils.Remove<int>(index, languageIDs);

            //remove language to other datas
            foreach (Type type in dataNames)
            {
                MethodInfo method = typeof(DataHolder).GetMethod("RemoveDataLanguage");
                MethodInfo generic = method.MakeGenericMethod(type);
                generic.Invoke(this, new object[] { index });
            }
        }

        public void RemoveDataLanguage<T>(int index) where T : BaseData
        {
            T[] list = GetDatas<T>();
            if (list != null)
            {
                foreach (T t in list)
                {
                    t.RemoveLanguage(index);
                }
            }

        }
        private void LoadLanguage()
        {
            string text = LoadFile("Language");
            LoadLanguage(text);
        }

        public void LoadLanguage(string text)
        {
            if (text != "")
            {
                languageIDs = DataUtils.getJsonArray<int>(text);
            }
        }

        public void SaveLanguage()
        {
            string text = JsonConvert.SerializeObject(languageIDs).ToString();
#if UNITY_2021_1_OR_NEWER
            string savePath = Application.dataPath + "/Game/Resources/Data/Language.txt";
            if (!File.Exists(savePath))
            {
                FileStream file = File.Open(savePath, FileMode.Create);
                file.Close();
            }

            //File.AppendAllText
            File.Delete(savePath);
            File.WriteAllText(savePath, text);
#endif
        }

        public string GetLanguageName(int id)
        {
            return LanguageName[languageIDs[id]];
        }

        public string[] GetLanguageNameList()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < languageIDs.Length; i++)
            {
                names.Add(LanguageName[languageIDs[i]]);
            }

            return names.ToArray();
        }

        #endregion


        string GetDataLanguage<T>() where T : BaseData
        {
            string text = "";

            T[] list = GetDatas<T>();

            if (list == null)
                return "";


            foreach (T t in list)
            {
                if (t.GetName(0) != "" && t.isTranslateName)
                {
                    //text += t.itemId + "_Name;";
                    for (int i = 0; i < languageIDs.Length; i++)
                    {
                        text += t.GetName(i);
                        if (i < list.Length - 1)
                        {
                            text += ";";
                        }
                    }
                    text += "\n";
                }

                if (t.GetDescription(0) != "" && t.isTranslateDescription)
                {
                    //text += t.itemId + "_Description;";
                    for (int i = 0; i < languageIDs.Length; i++)
                    {
                        text += t.GetDescription(i);
                        if (i < list.Length - 1)
                        {
                            text += ";";
                        }
                    }
                    text += "\n";
                }
            }

            return text;
        }

        public static string[] LanguageName = new string[] {"Afrikaans","Arabic","Armenian","Belarusian","Bulgarian","Catalan"," Chinese (Simplified)","Chinese (Traditional)","Croatian"
        ,"Czech","Danish","Dutch","English","Esperanto","Estonian","Filipino","Finnish","French","German","Greek","Hebrew","Hindi","Hungarian","Icelandic"
        ,"Indonesian","Italian","Japanese","Korean","Latvian","Lithuanian","Norwegian","Persian","Polish","Portuguese","Romanian","Russian","Serbian"
        ,"Slovak","Slovenian","Spanish","Swahili","Swedish","Thai","Turkish","Ukrainian","Vietnamese"};
        public static string[] LanguageCode = new string[] {"af","ar","hy","be","bg","ca","zh-CN","zh-TW","hr","cs","da","nl","en","eo","et","tl","fi","fr"
        ,"de","el","iw","hi","hu","is","id","it","ja","ko","lv","lt","no","fa","pl","pt","ro","ru","sr","sk","sl","es","sw","sv","th","tr","uk","vi"};


        public static string[] CountryName = new string[] {
            "SouthAfrica", // Afrikaans
            "SaudiArabia", // Arabic
            "Armenia", // Armenian
            "Belarus", // Belarusian
            "Bulgaria", // Bulgarian
            "Spain", // Catalan, specifically Catalonia
            "China", // Chinese (Simplified)
            "Taiwan", // Chinese (Traditional), also Hong Kong and Macau
            "Croatia", // Croatian
            "CzechRepublic", // Czech
            "Denmark", // Danish
            "Netherlands", // Dutch
            "UnitedKingdom", // English
            "Esperanto",
            "Estonia", // Estonian
            "Philippines", // Filipino
            "Finland", // Finnish
            "France", // French
            "Germany", // German
            "Greece", // Greek
            "Israel", // Hebrew
            "India", // Hindi
            "Hungary", // Hungarian
            "Iceland", // Icelandic
            "Indonesia", // Indonesian
            "Italy", // Italian
            "Japan", // Japanese
            "SouthKorea", // Korean
            "Latvia", // Latvian
            "Lithuania", // Lithuanian
            "Norway", // Norwegian
            "Iran", // Persian (Farsi)
            "Poland", // Polish
            "Portugal", // Portuguese
            "Romania", // Romanian
            "Russia", // Russian
            "Serbia", // Serbian
            "Slovakia", // Slovak
            "Slovenia", // Slovenian
            "Spain", // Spanish
            "Tanzania", // Swahili, also Kenya and other East African countries
            "Sweden", // Swedish
            "Thailand", // Thai
            "Turkey", // Turkish
            "Ukraine", // Ukrainian
            "Vietnam" // Vietnamese
        };

    }
}