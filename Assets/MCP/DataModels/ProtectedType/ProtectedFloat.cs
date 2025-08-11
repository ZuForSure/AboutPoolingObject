using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using MCP.Unity;
using System;
using UnityEngine;

namespace MCP.DataModels.ProtectedType
{
    [Serializable]
    public class ProtectedFloat : BaseModel
    {
        public float value = 0;

        public string v = "";

        public int i = 0;

        public string s = "";

        public ProtectedFloat(float t, int i)
        {
            this.i = i;
            this.value = t;
            this.s = CheckSum();
            this.v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(System.Globalization.CultureInfo.InvariantCulture), i);
        }

        public string CheckSum()
        {
            return DataUtils.Md5Sum(ProtectedTypeUtils.GetInstance().EncodeType(i.ToString()) + value.ToString(System.Globalization.CultureInfo.InvariantCulture) + UnityUtils.GetDeviceID()).Substring(8, 8);
        }

        public float Set(float t, int i)
        {
            this.i = i;
            this.value = t;
            this.s = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(System.Globalization.CultureInfo.InvariantCulture), i);
            return this.value;
        }

        public float Set(float t)
        {
            this.value = t;
            this.s = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(System.Globalization.CultureInfo.InvariantCulture), i);
            return this.value;
        }

        public float Refresh(int oldI, int newI, Action hackDetected)
        {
            if (i != 0 && i != oldI)
            {
                hackDetected();
                return this.Set(0, newI);
            }
            if (s != CheckSum())
            {
                hackDetected();
                try
                {
                    if (this.v != "@@")
                    {
                        this.value = float.Parse(ProtectedTypeUtils.GetInstance().DecodeType(v, i), System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception e)
                {
                    return this.Set(0, newI);
                }

                return this.Set(value, newI);
            }

            return this.Set(value, newI);
        }

        public float Add(float d, Action hackDetected)
        {
            if (s != CheckSum())
            {
                hackDetected();
                try
                {
                    if (this.v != "@@")
                    {
                        this.value = float.Parse(ProtectedTypeUtils.GetInstance().DecodeType(v, i), System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception e)
                {
                    return this.Set(d);
                }

                return this.Set(value +d);
            }

            return this.Set(value + d);
        }

        public float Get(Action hackDetected)
        {

            if (s != CheckSum())
            {
                hackDetected();
                try
                {
                    if (this.v != "@@")
                    {
                        this.value = float.Parse(ProtectedTypeUtils.GetInstance().DecodeType(v, i), System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Wrong Check sum " + e.StackTrace);
                    this.value = 0;
                }
                s = CheckSum();
                v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(System.Globalization.CultureInfo.InvariantCulture), i);
                return this.value;
            }

            return this.value;
        }


    }
}
