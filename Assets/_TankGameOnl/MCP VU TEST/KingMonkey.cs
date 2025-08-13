using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MCP;
using MCP.DataModels.BaseModels;
using MCP.DataModel.Utils;

public class KingMonkey : MonoBehaviour
{
    public GameObject bananaForMonkey;
    public List<Currency> currencies;

    private void Awake()
    {
        DataHolder.Instance().Init();
    }

    private void Start()
    {
        Debug.Log(DataHolder.Instance().GetLanguageName(1));
        Debug.Log(DataHolder.Instance().LoadFile("Language"));
        Debug.Log(BaseItem.ReferenceEquals(this.bananaForMonkey, this));


    }
}
