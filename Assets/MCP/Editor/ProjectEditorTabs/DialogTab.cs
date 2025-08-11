
using MCP.DataModels.BaseModels;
using UnityEditor;
using UnityEngine;

namespace MCP.Editor
{
 
    public class DialogTab : BaseTab
    {
        private GameObject tmpPrefab;
        private Dialog[] tmpDialogs = new Dialog[0];
        public DialogTab(ProjectWindow pw) : base(pw)
        {
            this.Reload();
        }

        public override void SaveTab()
        {
            base.SaveTab();
            DataHolder.Instance().SaveData<Dialog>();
        }

        public override void ShowTab()
        {
            base.ShowTab();
            AddToolBarEditor<Dialog>();
            if (DataHolder.Instance().GetDialogCount(currentKey,dialogType) > 0)
            {
                EditorGUI.BeginChangeCheck();
                Dialog item = DataHolder.Instance().GetDialog(selection, currentKey,dialogType);
                item.dialogType = (DialogType)EditorGUILayout.EnumPopup("Dialog Type", item.dialogType, GUILayout.Width(pw.mWidth));
                if (EditorGUI.EndChangeCheck())
                    isChanged = true;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            this.EndTab();
        }
    }
}
