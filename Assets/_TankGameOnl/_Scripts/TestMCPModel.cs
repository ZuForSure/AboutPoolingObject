using MCP.DataModels.BaseModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMCPModel : MonoBehaviour
{
    [Header("==============Languege==============")]
    public string nameLanguage;
    public string[] arrayNameLanguage;
    //public LanguageItem languegeCurrently;

    [Header("==============Currency==============")]
    public int currencyID;
    public int currencyIDByName;
    public Currency currencyCurrently;
    public Currency[] listCurrencies;

    [Header("==============Card==============")]
    public EquipmentType equipmentType;
    public RareType rareType;
    public List<int> cardIds;
    public List<Card> listCards;
    public Card cardCurrently;
    public Card[] arrayCard;

    [Header("==============Box==============")]
    public Box boxCurrently;
    public Box[] arrayBox;

    [Header("==============Bundle==============")]
    public Bundle bundleCurrently;

    [Header("==============dialog==============")]
    public Dialog dialogCurrently;



    private void Awake()
    {
        DataHolder.Instance().Init(); // phải khởi tạo DataHolder trước khi sử dụng
    }

    private void Start()
    {
        // =============Language=============
        nameLanguage = DataHolder.Instance().GetLanguageName(0); // lấy từng ngôn ngữ theo ID
        arrayNameLanguage = DataHolder.Instance().GetLanguageNameList(); // lấy tất cả ngôn ngữ

        // =============Currency=============
        currencyID = DataHolder.Instance().GetCurrencyId("VM"); // lấy ID của currency theo tên
        currencyIDByName = DataHolder.Instance().GetCurrencyIdByName("Vitual Money"); // lấy ID của currency theo tên

        // =============Card=============
        cardIds = DataHolder.Instance().GetCardIds(equipmentType); // lấy danh sách ID của card theo EquipmentType
        listCards = DataHolder.Instance().GetCards(rareType); // lấy danh sách Card theo RareType

        // Các hàm ở trên là để lấy trực tiếp dữ liệu từ DataHolder

        // -------------GetData------------

        // Các hàm dưới đây là để lấy dữ liệu từ DataHolder bằng class kế thừa từ BaseData
        currencyCurrently = DataHolder.Instance().GetData<Currency>(currencyID); // lấy dữ liệu Currency theo ID
        cardCurrently = DataHolder.Instance().GetData<Card>(0); // lấy dữ liệu Card theo ID
        boxCurrently = DataHolder.Instance().GetData<Box>(0); // lấy dữ liệu Box theo ID
        bundleCurrently = DataHolder.Instance().GetData<Bundle>(0); // lấy dữ liệu Bundle theo ID
        dialogCurrently = DataHolder.Instance().GetData<Dialog>(0); // lấy dữ liệu Dialog theo ID

        listCurrencies = DataHolder.Instance().GetDatas<Currency>("Real Money"); // lấy tất cả Currency có languega.name  là Real Money
        arrayCard = DataHolder.Instance().GetDatas<Card>("Cầu lửa"); // lấy tất cả Card có info ngôn ngữ 0 là "Cầu lửa"
        arrayBox = DataHolder.Instance().GetDatas<Box>(); // lấy tất cả Box

        //languegeCurrently = DataHolder.Instance().GetData<LanguageItem>(); // không lấy được vì class LanguageItem không kế thừa từ BaseData , cũng không có ID

    }
}
