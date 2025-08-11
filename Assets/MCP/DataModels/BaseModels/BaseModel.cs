using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.ComponentModel;
using Newtonsoft.Json;
using MCP.DataModel.Utils;

namespace MCP.DataModels.BaseModels
{
    [Serializable]
    public class BaseModel
    {

        //private Hashtable _valueTracker = new Hashtable();
        //private bool _valueTrackerLoaded = false;
        public BaseModel()
        {
            //_valueTracker = new Hashtable();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


        public string ToEncryptedJson(string key)
        {
            return DataUtils.EncryptStringWithKey(JsonConvert.SerializeObject(this), key);
        }


        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            Type objectType = this.GetType();
            FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                string fieldName = field.Name;
                object fieldValue = field.GetValue(this);
                dictionary[fieldName] = fieldValue;
            }

            return dictionary;
        }


        #region OldValueTracking
        /*
        public T GetMemberOldValue<T>(string key)
        {

            if (_valueTracker.Contains(key))
            {
                return (T)_valueTracker[key];
            }
            else
            {
                return default(T);
            }
        }


        public void SetMemberOldValue(string key, object obj)
        {
            if (_valueTracker.Contains(key))
            {
                _valueTracker[key] = obj;
            }
            else
            {
                _valueTracker.Add(key, obj);
            }
        }


        public string PrintValueTracker()
        {
            string result = "";
            foreach (DictionaryEntry info in _valueTracker)
            {
                result += (info.Key.ToString() + ": " + info.Value.ToString()) + "\r\n";
            }

            return result;
        }*/


        public void SaveAttribute(string attributeName, object value)
        {
            FieldInfo field = this.GetType().GetField(attributeName);

            if (field != null)
            {
                field.SetValue(this, value);
            }
        }

        #endregion

    }
}


