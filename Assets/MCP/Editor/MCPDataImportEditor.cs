using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System;
using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using System.Reflection;
using System.Linq;

namespace MCP.Editor
{
    public class GameDataExportEditor
    {
        [MenuItem("MCP/Export Card Data")]
        public static void ExportCardData()
        {
            string text = "";
            string fileName = "CardData";
            text += "Ids;Card;Description;Type\n";
            for (int i = 0; i < DataHolder.Instance().GetDataCount<Card>(); i++)
            {
                Card card = DataHolder.Instance().GetData<Card>(i);
                text += i.ToString() + ";";
                text += card.GetName(0) + ";";
                text += card.GetDescription(0) + ";";
                text += card.equipmentType.ToString() + ";";
                text += "\n";
            }

            string savePath = Application.dataPath + "/Game/ExportData/" + fileName + ".csv";
            if (!File.Exists(savePath))
            {
                FileStream file = File.Open(savePath, FileMode.Create);
                file.Close();
            }

            //File.AppendAllText
            File.Delete(savePath);
            File.WriteAllText(savePath, text);
        }
    }


#if ENABLE_PLAYFABADMIN_API
    public class GameDataImportEditor
    {
        public static TextAsset chestDataCsvFile;
        public static string[,] dataGrid;

        [MenuItem("MCP/Import Currency Data")]
        public static void ImportCurrencyDataFromExcel()
        {
            dataGrid = GetPath("Currency");
            DataHolder.Instance().RemoveAllData<Currency>();
            for (int y = 1; y < dataGrid.GetLength(1); y++)
            {
                Currency currency = DataHolder.Instance().AddData<Currency>();
                currency.SetName(0, dataGrid[1, y]);
                currency.Code = dataGrid[2, y];
                Debug.Log("Add Currency " + currency.GetName(0)); 
            };

            DataHolder.Instance().SaveData<Currency>();
            AssetDatabase.Refresh();

        }



        [MenuItem("MCP/Import Item Data")]
        public static void ImportItemDataFromExcel()
        {
            dataGrid = GetPath("Item");
            DataHolder.Instance().RemoveAllData<Item>();
            for (int y = 1; y < dataGrid.GetLength(1); y++)
            {
                Item item = DataHolder.Instance().AddData<Item>();
                item.isStackable = true;
                item.SetName(0, dataGrid[1, y]);
                item.itemType = Enum.Parse<ItemType>(dataGrid[2,y]);
                Debug.Log("Add Item " + item.GetName(0));
            };

            DataHolder.Instance().SaveData<Item>();
            AssetDatabase.Refresh();

        }

       


    #region Other funtion

        //[MenuItem("MCP/Other Function/Delete mana and type in card info")]

    #endregion
        static string[,] GetPath(string dataType)
        {
            string loadPath = Application.dataPath + "/Game/ImportDatas/";
            if (ES3.KeyExists(dataType + "Path"))
            {
                loadPath = ES3.Load<string>(dataType + "Path");
            }

            loadPath = EditorUtility.OpenFilePanel("Load import file", loadPath, "csv,txt");

            if (loadPath != "")
                ES3.Save(dataType + "Path", loadPath);

            string text = File.ReadAllText(loadPath);
            dataGrid = DataUtils.ReadFromCSVFile(text);
            for (int y = 0; y < dataGrid.GetLength(1); y++)
            {
                for (int x = 0; x < dataGrid.GetLength(0); x++)
                {
                    if(dataGrid[x, y] != null && dataGrid[x, y].Where(c => !char.IsControl(c)).ToArray() != null)
                        dataGrid[x,y] = new string(dataGrid[x, y].Where(c => !char.IsControl(c)).ToArray());
                }
            }
                

            return dataGrid;
        }

        static int GetIntFromString(string value, bool getZero = true)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (Int32.TryParse(value, out int result))
                {
                    return result;
                }
                else
                {
                    Debug.Log("Can't parse " + value + " to INT!");
                }
            }

            if (getZero)
                return 0;
            else
                return -1;
        }
        static float GetFloatFromString(string value, bool getZero = true)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (float.TryParse(value, out float result))
                {
                    return result;
                }
                else
                {
                    Debug.Log("Can't parse " + value + " to float!");
                }
            }

            if (getZero)
                return 0;
            else
                return -1;
        }
        static string[] GetStringList(string value)
        {
            string[] result = value.Split('@');
            return result;
        }
    }
#endif
}
