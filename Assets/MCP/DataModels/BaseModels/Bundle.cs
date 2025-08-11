using MCP.DataModels.Attributes;

namespace MCP.DataModels.BaseModels
{
    [System.Serializable]
    public class Bundle : BaseItem
    {
        public ItemPack[] itemPacks = new ItemPack[0];
        public string endTime;
        public int duration = 0;
    }
}




