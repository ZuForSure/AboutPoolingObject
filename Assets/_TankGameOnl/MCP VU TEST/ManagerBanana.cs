using MCP.DataModels.BaseModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBanana : MonoBehaviour
{
    void Start()
    {
        this.TestBaseData();
        this.TestBaseItem();
    }

    void TestBaseData()
    {
        Banana banaa = new();

        banaa.SetName(0, "A " + banaa.numberOfBanana + " " + banaa.idkWhatIsThisVar);
        banaa.SetDescription(0, "I Love Banana.");
        banaa.SetName(1, "B");
        banaa.SetDescription(1, "UU U KHE KHE");

        Debug.Log($"Length: {banaa.languageItem.Length}");
        for (int i = 0; i < banaa.languageItem.Length; i++)
        {
            Debug.Log($"Language: {banaa.languageItem[i].Name}");
        }

        banaa.AddLanguageItem();
        Debug.Log($"Length: {banaa.languageItem.Length}");
        for (int i = 0; i < banaa.languageItem.Length; i++)
        {
            Debug.Log($"Language: {banaa.languageItem[i].Name}");
        }

        banaa.RemoveLanguage(0);
        Debug.Log($"Length: {banaa.languageItem.Length}");
        for (int i = 0; i < banaa.languageItem.Length; i++)
        {
            Debug.Log($"Language: {banaa.languageItem[i].Name}");
        }

        Debug.Log($"EN: {banaa.GetName(0)} - {banaa.GetDescription(0)}");
        Debug.Log($"Monkey Language: {banaa.GetName(1)} - {banaa.GetDescription(1)}");
        //Debug.Log("ID: " + banaa.id);
    }

    void TestBaseItem()
    {
        Coconut mamacoco = new()
        {
            cost = new CurrencyData[]
            {
                new(),
                new(),
                new()
            }
        };

        for (int i = 0; i < mamacoco.cost.Length; i++)
        {
            Debug.LogWarning($"Cost {i}: {mamacoco.cost[i].value} - {mamacoco.cost[i].currencyId}");
        }
    }
}
