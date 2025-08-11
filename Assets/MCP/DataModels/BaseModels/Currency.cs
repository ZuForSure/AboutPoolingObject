using System;
namespace MCP.DataModels.BaseModels
{
    [Serializable]
    public class Currency : BaseData
    {
        public string Code;
    }

    [Serializable]
    public class CurrencyData
    {
        public int value;
        public int currencyId;
    }
}




