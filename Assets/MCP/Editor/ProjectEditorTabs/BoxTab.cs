
using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using UnityEditor;
using UnityEngine;
namespace MCP.Editor
{
    public class BoxTab : BaseTab
    {

        public BoxTab(ProjectWindow pw) : base(pw)
        {
            this.Reload();
        }


        public override void SaveTab()
        {
            base.SaveTab();
            DataHolder.Instance().SaveData<Box>();
        }

        public override void ShowTab()
        {
            base.ShowTab();
            AddToolBarEditor<Box>();
            if (DataHolder.Instance().GetDataCount<Box>(currentKey) > 0)
            {
                Box item = DataHolder.Instance().GetData<Box>(selection, currentKey);
                AddItemEditor(item);

                EditorGUI.BeginChangeCheck();

                item.randomType = (RandomType)EditorTab.EnumToolbar("RandomType Type", (int)item.randomType, typeof(RandomType), pw.mWidth);
                EditorGUILayout.BeginVertical("box");

                if (GUILayout.Button("Add Reward", GUILayout.Width(150)))
                {
                    item.randomPacks = DataUtils.Add(new RandomPack(), item.randomPacks);
                }

                for (int i = 0; i < item.randomPacks.Length; i++)
                {
                    EditorGUILayout.LabelField("Random Pack " + i.ToString());
                    item.randomPacks[i].itemClass = (ItemClass)EditorGUILayout.EnumPopup("Item Class", item.randomPacks[i].itemClass, GUILayout.Width(pw.mWidth));

                    item.randomPacks[i].itemId = EditorGUILayout.Popup(item.randomPacks[i].itemClass.ToString(), item.randomPacks[i].itemId, DataHolder.Instance().GetDataNameListByItemClass(item.randomPacks[i].itemClass), GUILayout.Width(pw.mWidth));
                    item.randomPacks[i].rareType = (RareType)EditorTab.EnumToolbar("Rare Type", (int)item.randomPacks[i].rareType, typeof(RareType), pw.mWidth);

                    item.randomPacks[i].minNumber = EditorGUILayout.IntField("Min Number", item.randomPacks[i].minNumber, GUILayout.Width(pw.mWidth));
                    item.randomPacks[i].maxNumber = EditorGUILayout.IntField("Max Number", item.randomPacks[i].maxNumber, GUILayout.Width(pw.mWidth));
                    item.randomPacks[i].chance = EditorGUILayout.FloatField("Chance", item.randomPacks[i].chance, GUILayout.Width(pw.mWidth));

                    if (GUILayout.Button("Remove Reward", GUILayout.Width(200)))
                    {
                        item.randomPacks = DataUtils.Remove(i, item.randomPacks);
                    }
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();

                }

                EditorGUILayout.EndVertical();
                if (EditorGUI.EndChangeCheck())
                    isChanged = true;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            this.EndTab();
        }
    }
}
