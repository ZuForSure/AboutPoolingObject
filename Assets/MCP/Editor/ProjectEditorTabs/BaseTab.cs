namespace MCP.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System.Collections.Generic;
    using MCP.DataModels.BaseModels;
    using MCP.DataModel.Utils;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public class BaseTab : EditorTab
	{
		protected ProjectWindow pw;
		protected Texture2D tmpSprites;
		protected int temcategory = -1;
		protected string currentKey="@";
		protected string[] tempSearchKeyArray;
		protected string tempKey = "";
        protected DialogType dialogType = DialogType.All;
        int currencyId = 0;
        public bool isChanged = false;
        public BaseTab(ProjectWindow pw) : base()
		{
			this.pw = pw;

		}

        public virtual void SaveTab()
        {
            isChanged = false;
        }

		public override void Reload()
		{
			selection = 0;
		}

        public virtual void ShowTab()
        {
        }
        
		public void AddItemList<T>() where T:BaseData
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.Width(pw.mWidth * 0.7f));

            EditorGUILayout.BeginHorizontal();
            tempKey = (string)EditorGUILayout.TextField("",tempKey, GUILayout.Width(pw.mWidth*0.7f));
            if (currentKey != tempKey)
            {
                currentKey = tempKey;
            }

            tempSearchKeyArray = DataHolder.Instance().GetDataNameSearchList<T>(currentKey);

            EditorGUILayout.EndHorizontal();

            if (typeof(T) == typeof(Dialog))
            {
                dialogType = (DialogType)EditorGUILayout.EnumPopup("Dialog Type", dialogType, GUILayout.Width(pw.mWidth * 0.7f));
                tempSearchKeyArray = DataHolder.Instance().GetDialogNameSearchList(currentKey, dialogType);
            }

            EditorGUILayout.BeginVertical("box");
			SP1 = EditorGUILayout.BeginScrollView(SP1, GUILayout.Width(pw.mWidth * 0.7f));

			if (DataHolder.Instance().GetDataCount<T>() > 0)
			{
                if (tempSearchKeyArray.Length - 1 < selection)
                    selection = 0;
                var prev = selection;
				selection = GUILayout.SelectionGrid(selection, tempSearchKeyArray, 1);
				if (prev != selection)
				{
					GUI.FocusControl("ID");
				}
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
		}

        public void AddLanguageList(int[] data)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(pw.mWidth * 0.7f));
            EditorGUILayout.BeginVertical("box");
            SP1 = EditorGUILayout.BeginScrollView(SP1, GUILayout.Width(pw.mWidth * 0.7f));

            if (data.Length > 0)
            {
                var prev = selection;
                selection = GUILayout.SelectionGrid(selection, DataHolder.Instance().GetLanguageNameList(), 1);
                if (prev != selection)
                {
                    GUI.FocusControl("ID");
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        public virtual void AddBaseItemEditor<T>(BaseData item) where T : BaseData
        {
			if (item == null)
				return;
            EditorGUILayout.LabelField("ID: " + item.id);
            EditorGUI.BeginChangeCheck();
            item.isAvailable = EditorGUILayout.Toggle("Available", item.isAvailable, GUILayout.Width(pw.mWidth));
            if (EditorGUI.EndChangeCheck())
                isChanged = true;
            EditorGUILayout.BeginVertical("box");
            fold1 = EditorGUILayout.Foldout(fold1, "Info");
            if (fold1)
            {
                EditorGUI.BeginChangeCheck();
                item.isTranslateName = EditorGUILayout.Toggle("Translate Name", item.isTranslateName, GUILayout.Width(pw.mWidth));
                item.isTranslateDescription = EditorGUILayout.Toggle("Translate Des", item.isTranslateDescription, GUILayout.Width(pw.mWidth));
                for (int i = 0; i < DataHolder.Instance().languageIDs.Length; i++)
                {
                    if (item != null && item.languageItem != null && (item.languageItem.Length <= i || item.languageItem[i] == null))
                        item.AddLanguageItem();

					EditorGUILayout.LabelField(DataHolder.Instance().GetLanguageName(i));
                    item.languageItem[i].Name = EditorGUILayout.TextField("Name", item.languageItem[i].Name, GUILayout.Width(pw.mWidth * 2), GUILayout.Height(80));
                    item.languageItem[i].Name = item.languageItem[i].Name.Replace("\\n", "\n");
                    item.languageItem[i].Description = EditorGUILayout.TextField("Description", item.languageItem[i].Description, GUILayout.Width(pw.mWidth * 2));
                    EditorGUILayout.Separator();
                }

                if (GUILayout.Button("Clear All", GUILayout.Width(pw.mWidth)))
                {
                    for (int i = 1; i < DataHolder.Instance().languageIDs.Length; i++)
                    {
                        item.languageItem[i].Name = "";
                        item.languageItem[i].Description = "";
                    }
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Icon", GUILayout.MaxWidth(110));
                if (item.iconUrl != null)
                {
                    this.tmpSprites = AssetDatabase.LoadAssetAtPath<Texture2D>(item.iconUrl);
                }
                this.tmpSprites = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, this.tmpSprites, typeof(Texture2D), false, GUILayout.MaxWidth(100));
                if (this.tmpSprites != null)
                {
                    item.iconUrl = AssetDatabase.GetAssetPath(this.tmpSprites);

                }
                EditorGUILayout.LabelField(item.iconUrl);
                EditorGUILayout.EndHorizontal();

                if (this.tmpSprites != null)
                {
                    if (GUILayout.Button("Clear Image", GUILayout.Width(100)))
                    {
                        item.iconUrl = "";
                        tmpSprites = null;
                    }
                }
                EditorGUILayout.Separator();
                if (EditorGUI.EndChangeCheck())
                    isChanged = true;
            }
            EditorGUILayout.EndVertical();
        }

        public void AddItemEditor(BaseItem item)
		{
            EditorGUILayout.BeginVertical("box");
            fold2 = EditorGUILayout.Foldout(fold2, "Item Settings");
            if (fold2)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Class Type: " + item.itemClass.ToString());
                item.isStackable = EditorGUILayout.Toggle("Stackable", item.isStackable, GUILayout.Width(pw.mWidth));
                if (item.isStackable)
                    item.isTradable = false;
                item.isTradable = EditorGUILayout.Toggle("Tradable", item.isTradable, GUILayout.Width(pw.mWidth));
                item.isLimited = EditorGUILayout.Toggle("Limited", item.isLimited, GUILayout.Width(pw.mWidth));

                /*
                if (GUILayout.Button("Add Tag", GUILayout.Width(pw.mWidth / 3)))
                {
                    item.tags = DataUtils.Add("", item.tags);
                }

                for (int i = 0; i < item.tags.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    item.tags[i] = EditorGUILayout.TextField("Tag " + i.ToString(), item.tags[i], GUILayout.Width(pw.mWidth));
                    if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                    {
                        item.tags = DataUtils.Remove(i, item.tags);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (item.itemConsumable == null && GUILayout.Button("Consumable", GUILayout.Width(pw.mWidth / 2)))
                {
                    item.itemConsumable = new ItemConsumable();
                }

                if (item.itemConsumable != null)
                {
                    if (GUILayout.Button("Remove Consumable", GUILayout.Width(pw.mWidth / 2)))
                    {
                        item.itemConsumable = null;
                    }
					if(item.itemConsumable != null)
					{
                        item.itemConsumable.consumableCount = (uint)EditorGUILayout.IntField("Consumable Count", (int)item.itemConsumable.consumableCount, GUILayout.Width(pw.mWidth));
                        item.itemConsumable.consumablePeriod = (uint)EditorGUILayout.IntField("Consumable Period", (int)item.itemConsumable.consumablePeriod, GUILayout.Width(pw.mWidth));
                        item.itemConsumable.consumablePeriodGroup = EditorGUILayout.TextField("Consumable Period Group", item.itemConsumable.consumablePeriodGroup, GUILayout.Width(pw.mWidth));
                    }
                }*/
                if (EditorGUI.EndChangeCheck())
                    isChanged = true;
            }
			EditorGUILayout.EndVertical();

            //Price
            EditorGUILayout.BeginVertical("box");
            fold3 = EditorGUILayout.Foldout(fold3, "Prices");
            if (fold3)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();               
                currencyId = (int)EditorGUILayout.Popup("Currency Type", currencyId, DataHolder.Instance().GetDataNameList<Currency>(), GUILayout.Width(pw.mWidth));
                if (GUILayout.Button("Add Currency", GUILayout.Width(pw.mWidth / 3)))
                {
                    CurrencyData currencyData = new CurrencyData();
                    currencyData.currencyId = currencyId;
                    item.cost = DataUtils.Add<CurrencyData>(currencyData, item.cost);
                }
                EditorGUILayout.EndHorizontal();

                for (int i = 0; i < item.cost.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    item.cost[i].value = EditorGUILayout.IntField("Value " + DataHolder.Instance().GetData<Currency>(item.cost[i].currencyId).Code, item.cost[i].value, GUILayout.Width(pw.mWidth));
                    if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                    {
                        item.cost = DataUtils.Remove<CurrencyData>(i, item.cost);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (EditorGUI.EndChangeCheck())
                    isChanged = true;
            }
            EditorGUILayout.EndVertical();
            Separate();
        }

		public void AddToolBarEditor<T>(bool showInfo = true) where T:BaseData
		{
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            if (selection < 0 && DataHolder.Instance().GetDataCount<T>() > 0)
                selection = 0;

            EditorGUI.BeginChangeCheck();
            this.ShowAddButton<T>();
            this.ShowCopyButton<T>();
            
            if (selection >= 0)
            {
                this.ShowRemButton<T>();
                this.ShowRemoveAllButton<T>();
            }
            
            if (EditorGUI.EndChangeCheck())
                isChanged = true;

            this.CheckSelection<T>();
            EditorGUILayout.EndHorizontal();

            // color list
            this.AddItemList<T>();
            EditorGUILayout.BeginVertical();
            SP2 = EditorGUILayout.BeginScrollView(SP2);

            if(typeof(T) != typeof(Dialog))
            {
                if (DataHolder.Instance().GetDataCount<T>(currentKey) > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    if (showInfo)
                    {
                        T item = DataHolder.Instance().GetData<T>(selection, currentKey);
                        AddBaseItemEditor<T>(item);
                    }
                }
            }
            else
            {
                if (DataHolder.Instance().GetDialogCount(currentKey,dialogType) > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();
                    if (showInfo)
                    {
                        Dialog item = DataHolder.Instance().GetDialog(selection, currentKey,dialogType);
                        AddBaseItemEditor<Dialog>(item);
                    }
                }
            }


        }

        public void AddID(string text)
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.Separator();
			GUI.SetNextControlName("ID");
			EditorGUILayout.LabelField(text, selection.ToString(), GUILayout.Width(pw.mWidth));
		}

		public bool ShowRemButton<T>()
		{
			GUI.SetNextControlName("Remove");
			bool press = GUILayout.Button("Remove", GUILayout.Width(pw.mWidth/4));
			if (press)
			{
				GUI.FocusControl("Remove");
				DataHolder.Instance().RemoveData<T>(selection);
			}
			return press;
		}

        public bool ShowRemoveAllButton<T>()
        {
            GUI.SetNextControlName("Remove All");
            bool press = GUILayout.Button("Remove All", GUILayout.Width(pw.mWidth / 4));
            if (press)
            {
                GUI.FocusControl("Remove");
                DataHolder.Instance().RemoveAllData<T>();
            }
            return press;
        }

        public bool ShowAddButton<T>()
        {
            bool press = GUILayout.Button("Add", GUILayout.Width(pw.mWidth /4));
            if(press)
            {
                T newData = DataHolder.Instance().AddData<T>();

                if (typeof(T) == typeof(Dialog))
                {
                    if (newData is Dialog dialog)
                    {
                        dialog.dialogType = dialogType;
                    }
                }

                selection = DataHolder.Instance().GetDataCount<T>() - 1;
                GUI.FocusControl("ID");
            }
            return press;
        }

		public bool ShowCopyButton<T>() where T:BaseData
		{
			GUI.SetNextControlName("Copy");
			bool press = GUILayout.Button("Copy", GUILayout.Width(pw.mWidth/4));
			if (press)
			{
				GUI.FocusControl("Copy");
				DataHolder.Instance().CopyData<T>(selection);
				selection = DataHolder.Instance().GetDataCount<T>() - 1;
                SaveTabAndRefresh();
            }

            return press;
		}

        private async void SaveTabAndRefresh()
        {
            await Task.Delay(100);
            // Call SaveTab
            SaveTab();
            AssetDatabase.Refresh();
            // Await a delay (e.g., 500 milliseconds)
            await Task.Delay(100);

            // Refresh the AssetDatabase after the delay
            DataHolder.Instance().Reload();
            //AssetDatabase.Refresh();
        }

        public void CheckSelection<T>()
        {
			if(selection >= DataHolder.Instance().GetDataCount<T>())
				selection = DataHolder.Instance().GetDataCount<T>() - 1;
        }


        public void EndTab()
		{
            EditorGUILayout.Separator();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

        public T DrawField<T>(string lable,object value)
        {
            Type type = typeof(T);
            if (type.IsEnum)
            {   
                //var index = (int)Enum.Parse(typeof(T), value.ToString());
                value = EditorTab.EnumToolbar("Rare Type", (int)value, typeof(T), pw.mWidth);
                return (T)value;
            }
            else if (type.IsArray)
            {
                return (T)value;
            }
            else
            {
                //Debug.Log(type.Name);
                switch (type.Name)
                {
                    case "Int32":
                        value = EditorGUILayout.IntField(lable, (int)value, GUILayout.Width(pw.mWidth));
                        break;
                    case "Single":
                        value = EditorGUILayout.FloatField(lable, (float)value, GUILayout.Width(pw.mWidth));
                        break;
                    case "Boolean":
                        value = EditorGUILayout.Toggle(lable, (bool)value, GUILayout.Width(pw.mWidth));
                        break;
                    case "String":
                        value = EditorGUILayout.TextField(lable, (string)value, GUILayout.Width(pw.mWidth));
                        break;
                    default:
                        break;
                }

                return (T)value;
            }
           
            
        }
    }
}
