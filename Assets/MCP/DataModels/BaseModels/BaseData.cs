using System.Reflection;
using MCP.DataModel.Utils;

namespace MCP.DataModels.BaseModels
{
    [System.Serializable]
    public class BaseData : BaseModel
    {
        //Essential
        //public string itemId = "";
        public int id = -1;
        public int Id()
        {            
            return id;
        }
        public LanguageItem[] languageItem = new LanguageItem[0];
        public bool isTranslateName = false;
        public bool isTranslateDescription = false;
        public string iconUrl = "";
        public bool isAvailable = true;

        public BaseData()
        {
            MethodInfo method = typeof(BaseData).GetMethod("Init");
            MethodInfo generic = method.MakeGenericMethod(this.GetType());
            generic.Invoke(this, null);
        }

        public void Init<T>()
        {
            languageItem = new LanguageItem[DataHolder.Instance().languageIDs.Length];
            for (int i = 0; i < languageItem.Length; i++)
            {
                languageItem[i] = new LanguageItem();
            }

            if (DataHolder.Instance() != null)
                GenerateID<T>(DataHolder.Instance().GetDataCount<T>());

            if (languageItem.Length > 0)
                languageItem[0].Name = typeof(T).Name + " " + DataHolder.Instance().GetDataCount<T>();
        }

        
        public void GenerateID<T>(int id)
        {
            this.id = id;
            //itemId = DataHolder.Instance().gameConfig.bundleName.ToLower() + typeof(T).Name.ToLower() + "." + id;
        }

        public void AddLanguageItem()
        {
            this.languageItem = DataUtils.Add(new LanguageItem(), this.languageItem);
        }

        public void RemoveLanguage(int index)
        {
            this.languageItem = DataUtils.Remove(index, this.languageItem);
        }

        public string GetDescription(int languageID)
        {
            if (languageItem.Length > languageID)
                if (languageItem[languageID].Description != "")
                    return languageItem[languageID].Description;
                else
                    return languageItem[0].Description;
            else
                return languageItem[0].Description;
        }

        public void SetDescription(int languageID, string text)
        {
            if (this.languageItem.Length > languageID)
            {
                languageItem[languageID].Description = text;
            }
            else
            {
                LanguageItem item = new LanguageItem();
                item.Description = text;
                this.languageItem = DataUtils.Add(item, this.languageItem);
            }
        }

        public string GetName(int languageID)
        {
            if (languageItem.Length > languageID)
                if (languageItem[languageID].Name != "")
                    return languageItem[languageID].Name;
                else
                    return languageItem[0].Name;
            else
                return languageItem[0].Name;
        }

        public void SetName(int languageID, string text)
        {
            if (this.languageItem.Length > languageID)
            {
                languageItem[languageID].Name = text;
            }
            else
            {
                LanguageItem item = new LanguageItem();
                item.Name = text;
                this.languageItem = DataUtils.Add(item, this.languageItem);
            }
        }
    }


    [System.Serializable]
    public class LanguageItem
    {
        public string Name = "";
        public string Description = "";

    }
}


