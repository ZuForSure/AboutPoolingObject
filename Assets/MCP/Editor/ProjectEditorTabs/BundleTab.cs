
using System.Collections.Generic;
using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using UnityEditor;
using UnityEngine;
namespace MCP.Editor
{
	public class BundleTab : BaseTab
	{
        public BundleTab(ProjectWindow pw) : base(pw)
		{
			this.Reload();
		}

        public override void SaveTab()
        {
            base.SaveTab();
            DataHolder.Instance().SaveData<Bundle>();
        }

        public override void ShowTab()
		{
			base.ShowTab();
			AddToolBarEditor<Bundle>();
			
			if (DataHolder.Instance().GetDataCount<Bundle>(currentKey) > 0)
			{
                EditorGUI.BeginChangeCheck();
                Bundle item = DataHolder.Instance().GetData<Bundle>(selection, currentKey);
                AddItemEditor(item);

                item.duration = EditorGUILayout.IntField("Duration", item.duration, GUILayout.Width(pw.mWidth));

                Separate();
                if (GUILayout.Button("Add Item", GUILayout.Width(pw.mWidth / 3)))
                {
                    ItemPack itemPack = new ItemPack();
                    item.itemPacks = DataUtils.Add<ItemPack>(itemPack, item.itemPacks);
                }

                for (int i = 0; i < item.itemPacks.Length; i++)
                {
                    Separate();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Item Pack " + i.ToString(), GUILayout.Width(pw.mWidth / 3));
                    if (GUILayout.Button("Remove", GUILayout.Width(pw.mWidth / 3)))
                    {
                        item.itemPacks = DataUtils.Remove<ItemPack>(i, item.itemPacks);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (item.itemPacks.Length > i)
                    {
                        item.itemPacks[i].itemClass = (ItemClass)EditorGUILayout.EnumPopup("Item Type", item.itemPacks[i].itemClass, GUILayout.Width(pw.mWidth));
                        item.itemPacks[i].itemId = EditorGUILayout.Popup(item.itemPacks[i].itemClass.ToString(), item.itemPacks[i].itemId, DataHolder.Instance().GetDataNameListByItemClass(item.itemPacks[i].itemClass), GUILayout.Width(pw.mWidth));
                        item.itemPacks[i].number = EditorGUILayout.IntField("Number", item.itemPacks[i].number, GUILayout.Width(pw.mWidth));
                        item.itemPacks[i].level = EditorGUILayout.IntField("Level", item.itemPacks[i].level, GUILayout.Width(pw.mWidth));
                        item.itemPacks[i].rareType = (RareType)EditorTab.EnumToolbar("Rare Type", (int)item.itemPacks[i].rareType, typeof(RareType), pw.mWidth);
                    }
                }


                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                    isChanged = true;
            }
			this.EndTab();
		}
	}
}
