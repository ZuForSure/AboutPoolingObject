
using System;
using UnityEditor;
using UnityEngine;

namespace MCP.Editor
{
	public class EditorTab
	{
		public static EditorTab instance;
		protected int selection = 0;
		protected Vector2 SP1 = new Vector2(0, 0);
		protected Vector2 SP2 = new Vector2(0, 0);
		protected Vector2 SP3 = new Vector2(0, 0);
		protected Vector2 SP4 = new Vector2(0, 0);
		protected bool fold1 = false;
		protected bool fold2 = false;
		protected bool fold3 = false;
		protected bool fold4 = false;
		protected bool fold5 = false;
		protected bool fold6 = false;
		protected bool fold7 = false;
		protected bool fold8 = false;
		protected bool fold9 = false;
		protected bool fold10 = false;
		protected bool fold11 = false;
		protected bool fold12 = false;
		protected bool fold13 = false;
		protected bool fold14 = false;
		protected bool fold15 = false;
		protected bool fold16 = false;
		protected bool fold17 = false;
		protected bool fold18 = false;

		public EditorTab()
		{
			instance = this;
		}

		public void Separate()
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
		}

		public static int EnumToolbar(string text, int index, System.Type e)
		{
			return EditorTab.EnumToolbar(text, index, e, 250);
		}

		public static int EnumToolbar(string text, int index, System.Type e, int width)
		{
			string[] names = System.Enum.GetNames(e);
			if (text != "")
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(text);
			}
			index = GUILayout.Toolbar(index, names, GUILayout.Width(width));
			if (text != "")
			{
				EditorGUILayout.EndHorizontal();
			}
			return index;
		}


		public static int ChanceCheck(int chance)
		{
			if (chance < 0) chance = 0;
			else if (chance > 100) chance = 100;
			return chance;
		}

		public static int MinMaxCheck(int check, int min, int max)
		{
			if (check < min) check = min;
			else if (check > max) check = max;
			return check;
		}



		public virtual void Reload()
		{

		}
	}
}
