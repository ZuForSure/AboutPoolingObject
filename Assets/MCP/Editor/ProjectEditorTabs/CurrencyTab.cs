
using MCP.DataModels.BaseModels;
using UnityEditor;
using UnityEngine;

namespace MCP.Editor
{
    public class CurrencyTab : BaseTab
    {
        public CurrencyTab(ProjectWindow pw) : base(pw)
        {
            this.Reload();
        }

        public override void SaveTab()
        {
            base.SaveTab();
            DataHolder.Instance().SaveData<Currency>();
        }

        public override void ShowTab()
        {
            base.ShowTab();
            AddToolBarEditor<Currency>(false);
            if (DataHolder.Instance().GetDataCount<Currency>(currentKey) > 0)
            {
                EditorGUI.BeginChangeCheck();
                Currency currency = DataHolder.Instance().GetData<Currency>(selection, currentKey);
                currency.Code = EditorGUILayout.TextField("Code", currency.Code, GUILayout.Width(pw.mWidth));
                currency.languageItem[0].Name = EditorGUILayout.TextField("Name", currency.languageItem[0].Name, GUILayout.Width(pw.mWidth));

                if (currency.Id() == 0)
                {
                    currency.Code = "RM";
                    currency.SetName(0, "Real Money");

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
