using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using MCP.DataModels.BaseModels;


#if UNITY_EDITOR && ENABLE_PLAYFABADMIN_API
using PlayFab.AdminModels;
#endif


namespace MCP.Editor
{
    public partial class ProjectWindow : EditorWindow
    {
        public int mWidth = 300;
        private int currentSection = 0;
        public bool isLogin = false;

        //Config all Tab here
        private Type[] types = new Type[] {
            typeof(LanguageTab),
            typeof(CurrencyTab),
            typeof(CardTab),
            typeof(BoxTab),
            typeof(BundleTab),
            typeof(DialogTab),
        };

        // tabs
        Hashtable tabs = new Hashtable();
        List<string> tabname = new List<string>();
#if USING_PLAYFAB
        PlayFab.ClientModels.LoginResult loginResult;
#endif
        public bool isLoading = false;

        [MenuItem("MCP/Game Editor", priority = 0)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ProjectWindow window = (ProjectWindow)EditorWindow.GetWindow(typeof(ProjectWindow), false, "Game Editor");
            window.Reload();
            window.Show();
            window.LoginPlayfab();
        }

        public void Reload()
        {
            DataHolder.Instance().Init();

            foreach (Type type in types)
            {
                if (!tabname.Contains(type.Name.Replace("Tab", "")))
                    tabname.Add(type.Name.Replace("Tab", ""));

                if (!tabs.ContainsKey(type.ToString()))
                {
                    object tab = Activator.CreateInstance(type, new object[] { this });
                    tabs.Add(type.ToString(), tab);
                }
                else
                {
                    ((BaseTab)tabs[type.ToString()]).Reload();
                }
            }
        }

        public void Save()
        {
            ((BaseTab)tabs[types[currentSection].ToString()]).SaveTab();
            AssetDatabase.Refresh();
            Debug.Log("Save Editor Data success");
        }

        void OnGUI()
        {
            if (tabs[typeof(LanguageTab).ToString()] == null)
                Init();

            EditorGUI.BeginDisabledGroup(isLoading);

            GUI.SetNextControlName("Toolbar");
            var prevSection = currentSection;

            string[] names = new string[tabname.Count];
            for (int i = 0; i < tabname.Count; i++)
            {
                names[i] = tabname[i];
                if (((BaseTab)tabs[types[i].ToString()]).isChanged)
                    names[i] += "*";
            }

            currentSection = GUILayout.SelectionGrid(currentSection, names, 7);
            GUILayout.Box(" ", GUILayout.ExpandWidth(true));


            if (tabs.ContainsKey(types[currentSection].ToString()))
            {
                ((BaseTab)tabs[types[currentSection].ToString()]).ShowTab();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh", GUILayout.Width(mWidth / 4)))
            {
                DataHolder.Instance().Reload();
            }

            if (((BaseTab)tabs[types[currentSection].ToString()]).isChanged)
            {
                if (GUILayout.Button("Save", GUILayout.Width(mWidth / 4)))
                {
                    this.Save();
                }
            }

#if UNITY_EDITOR && ENABLE_PLAYFABADMIN_API
            if (types[currentSection] == typeof(CurrencyTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                UploadCurrencyToPlayfab();
            }

            if (types[currentSection] == typeof(ItemTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                UploadItemsToPlayfab<Item>();
            }

            if (types[currentSection] == typeof(CardTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                UploadItemsToPlayfab<Card>();
            }

            if (types[currentSection] == typeof(BoxTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                UploadItemsToPlayfab<Box>();
            }

            if (types[currentSection] == typeof(EmoteTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                UploadItemsToPlayfab<Emote>();
            }
            if (types[currentSection] == typeof(BundleTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                UploadItemsToPlayfab<Bundle>();
            }

#endif

            if (types[currentSection] == typeof(LanguageTab) && GUILayout.Button("Export Language", GUILayout.Width(mWidth / 2)))
            {
                //Export
                ExportLanguage();
            }

            if (types[currentSection] == typeof(LanguageTab) && GUILayout.Button("Import Language", GUILayout.Width(mWidth / 2)))
            {
                ImportLanguage();
            }


#if UNITY_EDITOR && ENABLE_PLAYFABADMIN_API

            if (types[currentSection] == typeof(NotificationTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                string value = "{\"Notification\":" + DataHolder.Instance().LoadFile("Notification") + "}";
                SaveGameDataConfiguration("LocalNotification", value);
            }
            if (types[currentSection] == typeof(GameEventTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                string value = "{\"GameEvent\":" + DataHolder.Instance().LoadFile("GameEvent") + "}";
                SaveGameDataConfiguration("GameEventData", value);
            }
            if (types[currentSection] == typeof(GameVersionTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                string value = DataHolder.Instance().LoadFile("GameVersion");
                SaveGameDataConfiguration("GameVersion", value);
            }

            if (types[currentSection] == typeof(MailTab) && GUILayout.Button("upload", GUILayout.Width(mWidth / 4)))
            {
                UploadNews();
            }
#endif

#if ENABLE_UPLOAD_DATA_SERVER
            if (types[currentSection] == typeof(LanguageTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Language");
            }
            if (types[currentSection] == typeof(CurrencyTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Currency");
            }
            if (types[currentSection] == typeof(ItemTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Item");
            }
            if (types[currentSection] == typeof(CardTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Card");
            }
            if (types[currentSection] == typeof(BoxTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Box");
            }
            if (types[currentSection] == typeof(FragmentTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Fragment");
            }
            if (types[currentSection] == typeof(ChestTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Chest");
            }
            if (types[currentSection] == typeof(EmoteTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Emote");
            }
            if (types[currentSection] == typeof(BundleTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Bundle");
            }
            if (types[currentSection] == typeof(QuestTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Quest");
            }
            if (types[currentSection] == typeof(StatTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Stat");
            }
            if (types[currentSection] == typeof(LevelStatTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("LevelStat");
            }
            if (types[currentSection] == typeof(ChestCycleTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("ChestCycle");
            }
            if (types[currentSection] == typeof(NewPlayerTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("NewPlayer");
            }
            if (types[currentSection] == typeof(GameplayConfigTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("GameplayConfig");
            }
            if (types[currentSection] == typeof(DialogTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("Dialog");
            }
            if (types[currentSection] == typeof(GameEventTab) && GUILayout.Button("upload Server", GUILayout.Width(mWidth / 3)))
            {
                UploadToServer("GameEvent");
            }
#endif

            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }


        public void LoginPlayfab()
        {
#if USING_PLAYFAB
            PlayFabSharedSettings playFabSharedSettings = Resources.Load<PlayFabSharedSettings>("PlayFabSharedSettings");
            PlayFab.PlayFabSettings.TitleId = playFabSharedSettings.TitleId;
            PlayFab.PlayFabSettings.DeveloperSecretKey = playFabSharedSettings.DeveloperSecretKey;
            APIManager.GetInstance().LoginWithDeviceID((data) => {
                Debug.Log("Login Sucess " + data.PlayFabId);
                loginResult = data;
                isLogin = true;
                isLoading = false;
            },
            (error) => {
                Debug.Log("error " + error.ErrorMessage);
            });
#endif
        }

#if UNITY_EDITOR && ENABLE_PLAYFABADMIN_API
        void UploadCurrencyToPlayfab()
        {
            if (!EditorUtility.DisplayDialog("Warning", "Do you want to upload currency to playfab?", "ok", "cancel"))
            {
                Debug.Log("Cancel upload");
                return;
            }

            isLoading = true;
            MCP.DataModels.BaseModels.Currency[] currencies = DataHolder.Instance().GetDatas<MCP.DataModels.BaseModels.Currency>();
            List<VirtualCurrencyData> virtualCurrencyDatas = new List<VirtualCurrencyData>();
            for (int i = 1; i < currencies.Length; i++)
            {
                VirtualCurrencyData virtualCurrencyData = new VirtualCurrencyData();
                virtualCurrencyData.CurrencyCode = currencies[i].Code;
                virtualCurrencyData.DisplayName = currencies[i].GetName(0);
                virtualCurrencyDatas.Add(virtualCurrencyData);
            }
            AdminAPIManager.CreateCurrency(virtualCurrencyDatas, () => {
                Debug.Log("Success Editor Upload Items");
                //Upload Currency
                isLoading = false;
            }, (error) => {
                Debug.Log("error " + error);
                isLoading = false;
            });
        }
        void UploadToServer(string fileName)
        {
            if (!EditorUtility.DisplayDialog("Warning", "Do you want to upload to server?", "ok", "cancel"))
            {
                Debug.Log("Cancel upload");
                return;
            }

            if (string.IsNullOrEmpty(DataHolder.Instance().gameVersion.dataVersion)
                || DataUtils.StringToNum(DataHolder.Instance().gameVersion.dataVersion) < DataUtils.StringToNum(Application.version))
            {
                if (EditorUtility.DisplayDialog("Warning", "Data version can not empty or less than Application version! ", "ok"))
                    return;
            }

            isLoading = true;
            string directoryName = UnityUtils.GetGameConfigFolder(DataHolder.Instance().gameVersion.dataVersion);

            TextAsset textFile = Resources.Load("Data/" + fileName) as TextAsset;
            string path = "Assets/Game/Resources/Data/" + fileName + ".txt";
            if (textFile != null)
            {
                APIManager.Instance.UploadFile(loginResult, UnityUtils.ZipFileToStream(path), fileName, directoryName, true, result =>
                {
                    isLoading = false;
                    Debug.Log("Success Editor Upload Items");
                }, error =>
                {
                    Debug.Log("error " + error);
                    isLoading = false;
                });
            }

        }
        void UploadItemsToPlayfab<T>() where T : BaseItem
        {
            if (!EditorUtility.DisplayDialog("Warning", "Do you want to upload " + typeof(T).Name + " to playfab?", "ok", "cancel"))
            {
                Debug.Log("Cancel upload " + typeof(T).Name);
                return;
            }

            if (isLogin)
            {
                isLoading = true;

                //Upload all Items
                List<CatalogItem> catalogItems = GetCatalogItems<T>(DataHolder.Instance().GetDatas<T>().ToList());

                AdminAPIManager.CreateItems(DataHolder.Instance().gameConfig.catalogVersion, catalogItems, () =>
                {
                    Debug.Log("Success Upload " + typeof(T).Name + "Count: " + catalogItems.Count);
                    isLoading = false;
                }, (error) =>
                {
                    Debug.Log("Upload error " + error);
                    isLoading = false;
                });
            }
            else
            {
                EditorUtility.DisplayDialog("Warning", "Login required", "ok");
            }
        }

        public List<CatalogItem> GetCatalogItems<T>(List<T> items) where T : BaseItem
        {
            List<CatalogItem> catalogItems = new List<CatalogItem>();

            foreach (T item in items)
            {
                if (item.isAvailable)
                {
                    PlayFab.AdminModels.CatalogItem catalogItem = new PlayFab.AdminModels.CatalogItem()
                    {
                        ItemId = item.itemId,
                        ItemClass = item.itemClass.ToString(),
                        Tags = item.tags.ToList(),
                        DisplayName = item.GetName(0),
                        IsTradable = item.isTradable,
                        IsStackable = item.isStackable,
                        CustomData = AttributeHelper.GetCustomDataFromValue<T>(item)
                    };


                    catalogItem.VirtualCurrencyPrices = new Dictionary<string, uint>();
                    for (int i = 0; i < item.cost.Length; i++)
                    {
                        DataModels.BaseModels.Currency currency = DataHolder.Instance().GetData<DataModels.BaseModels.Currency>(item.cost[i].currencyId);
                        if (currency != null)
                            catalogItem.VirtualCurrencyPrices.Add(currency.Code, (uint)item.cost[i].value);
                    }

                    if (item.itemConsumable != null)
                    {
                        CatalogItemConsumableInfo catalogItemConsumableInfo = new CatalogItemConsumableInfo();
                        if (item.itemConsumable.consumableCount > 0)
                            catalogItemConsumableInfo.UsageCount = item.itemConsumable.consumableCount;
                        if (item.itemConsumable.consumablePeriod > 0)
                            catalogItemConsumableInfo.UsagePeriod = item.itemConsumable.consumablePeriod;
                        if (item.itemConsumable.consumablePeriodGroup != "")
                            catalogItemConsumableInfo.UsagePeriodGroup = item.itemConsumable.consumablePeriodGroup;
                        catalogItem.Consumable = catalogItemConsumableInfo;
                    }


                    catalogItems.Add(catalogItem);
                }

            }
            return catalogItems;
        }

        public void SaveGameDataConfiguration(string saveKey, string saveData)
        {
            AdminAPIManager.UploadGameDataConfiguration(saveKey, saveData, () =>
            {
                Debug.Log("Success Upload GameDataConfiguration " + saveKey);
            }, error =>
            {
                Debug.Log("Upload error GameDataConfiguration " + error);
            });
        }

        public async void UploadNews()
        {
            for (int i = 0; i < DataHolder.Instance().GetDataCount<Message>(); i++)
            {
                Message message = DataHolder.Instance().GetData<Message>(i);
                if(message.messageType == DataModels.BaseModels.MessageType.News && message.isAvailable)
                {
                    AdminAPIManager.UploadNews(message.GetName(0), message.ToJson(), () =>
                    {
                        Debug.Log("Success Upload Message " + message.GetName(0));
                    }, error =>
                    {
                        Debug.Log("Upload error message " + error);
                    });
                    await System.Threading.Tasks.Task.Delay(3000);
                }
            }
        }
#endif

        public void ExportLanguage()
        {
            DataHolder.Instance().ExportLanguage();
            Debug.Log("Export Successful");
            AssetDatabase.Refresh();
        }

        public void ImportLanguage()
        {
            //Import
            string loadPath = Application.dataPath + "/Game/ImportDatas/";
            /*
            if (ES3.KeyExists("ImportLanguagePath"))
            {
                loadPath = ES3.Load<string>("ImportLanguagePath");
            }

            loadPath = EditorUtility.OpenFilePanel("Load import file", loadPath, "csv,txt");

            if (loadPath != "")
                ES3.Save("ImportLanguagePath", loadPath);
            */

            string text = File.ReadAllText(loadPath);

            DataHolder.Instance().ImportLanguage(text);
            for(int i = 0; i < types.Length; i++)
            {
                ((BaseTab)tabs[types[i].ToString()]).SaveTab();
            }
                
            Debug.Log("Import " + loadPath + "Successful");
            AssetDatabase.Refresh();
        }
    }
}
