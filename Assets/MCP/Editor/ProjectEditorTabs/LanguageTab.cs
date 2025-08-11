
using MCP.DataModels.BaseModels;
using UnityEditor;
using UnityEngine;
namespace MCP.Editor
{
	public class LanguageTab : BaseTab
	{
		public LanguageTab(ProjectWindow pw) : base(pw)
		{
			this.Reload();
		}


        public override void SaveTab()
        {
            base.SaveTab();
            DataHolder.Instance().SaveLanguage();
        }

        public override void ShowTab()
		{
			EditorGUILayout.BeginVertical();

			// buttons
			EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Add Language", GUILayout.Width(pw.mWidth/2)))
			{
				DataHolder.Instance().AddLanguage();
				selection = DataHolder.Instance().languageIDs.Length - 1;
				GUI.FocusControl("ID");
				Debug.Log("Add New Language " + DataHolder.Instance().languageIDs.Length);
			}
			if (DataHolder.Instance().languageIDs.Length > 1)
			{
				if (GUILayout.Button("Remove Language", GUILayout.Width(pw.mWidth/2)))
                {
                    DataHolder.Instance().RemoveLanguage(selection);
                    selection = DataHolder.Instance().languageIDs.Length - 1;
				}
			}
            if (EditorGUI.EndChangeCheck())
                isChanged = true;
            EditorGUILayout.EndHorizontal();

			// status value list
			this.AddLanguageList(DataHolder.Instance().languageIDs);

			// value settings
			EditorGUILayout.BeginVertical();
			SP2 = EditorGUILayout.BeginScrollView(SP2);
			if (DataHolder.Instance().languageIDs.Length > 0)
			{
                EditorGUI.BeginChangeCheck();
                this.AddID("Language ID");
				DataHolder.Instance().languageIDs[selection] = EditorGUILayout.Popup("Language name", DataHolder.Instance().languageIDs[selection], DataHolder.LanguageName, GUILayout.Width(pw.mWidth * 2));
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Language Code ", GUILayout.Width(pw.mWidth * 0.5f));
				EditorGUILayout.LabelField(DataHolder.LanguageCode[DataHolder.Instance().languageIDs[selection]], GUILayout.Width(pw.mWidth * 0.5f));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
                if (EditorGUI.EndChangeCheck())
                    isChanged = true;
            }
			this.EndTab();
		}
	}
}
