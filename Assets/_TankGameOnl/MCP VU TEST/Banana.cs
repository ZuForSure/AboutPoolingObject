using MCP.DataModels.BaseModels;

[System.Serializable]
public class Banana : BaseData
{
    public int numberOfBanana;
    public float idkWhatIsThisVar;
}

[System.Serializable]
public class Coconut : BaseItem
{
    public Coconut() : base()
    {
        itemClass = ItemClass.Coconut;
        isStackable = false;
        isTradable = true;
        isLimited = false;
        cost = new CurrencyData[0];
    }
}
