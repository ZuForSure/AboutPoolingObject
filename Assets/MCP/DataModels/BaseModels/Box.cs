using MCP.DataModels.Attributes;

namespace MCP.DataModels.BaseModels
{
    [System.Serializable]
    public class Box : BaseItem
    {
        public RandomType randomType = RandomType.Option;
        public RandomPack[] randomPacks = new RandomPack[0];
    }

    [System.Serializable]
    public class RandomPack : BaseModel
    {
        public float chance = 0;
        public ItemClass itemClass = ItemClass.Card;
        public RareType rareType = RareType.Common;
        public ItemType itemType = ItemType.None;
        public int itemId = 0;
        public int minNumber = 0;
        public int maxNumber = 0;
    }
}




