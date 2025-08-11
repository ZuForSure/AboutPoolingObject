using MCP.DataModels.Attributes;

namespace MCP.DataModels.BaseModels
{
	[System.Serializable]
	public class Dialog : BaseData
	{
		public DialogType dialogType;
		public Dialog() : base()
		{
			if(languageItem != null && languageItem.Length > 0)
				languageItem[0].Name = "New Dialog";
		}
	}
}
