using System;
using System.Collections.Generic;
using System.Reflection;
using MCP.DataModel.Utils;
using MCP.DataModels.Attributes;

namespace MCP.DataModels.BaseModels
{
    [System.Serializable]
    public class BaseItem : BaseData
    {
        public ItemClass itemClass = ItemClass.Card;
        public bool isStackable = false;
        public bool isTradable = false;
        public bool isLimited = false;
        public CurrencyData[] cost = new CurrencyData[0];

        public BaseItem() : base()
        {
            itemClass = System.Enum.Parse<ItemClass>(this.GetType().Name);
        }
    }

    [System.Serializable]
    public class ItemPack : BaseModel
    {
        public ItemClass itemClass = ItemClass.Card;
        public int itemId = 0;
        public int number = 0;
        public RareType rareType = RareType.Common;
        public int level = 0;
    }
}




