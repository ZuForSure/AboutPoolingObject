using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using MCP.Unity;
using System;

namespace MCP.DataModels.ProtectedType
{
    [Serializable]
    public class ProtectedInt : BaseModel
    {
        public int value = 0;

        public string v = "";

        public int i = 0;

        public string s = "";

        public ProtectedInt(int t, int i)
        {
            this.i = i;
            value = t;
            s = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(), i);
        }

        public string CheckSum()
        {
            return DataUtils.Md5Sum(ProtectedTypeUtils.GetInstance().EncodeType(i.ToString()) + value.ToString() + UnityUtils.GetDeviceID()).Substring(8, 8);
        }

        public int Set(int t, int i)
        {
            this.i = i;
            this.value = t;
            s = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(), i);
            return this.value;
        }


        public int Set(int t)
        {
            this.value = t;
            s = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(), i);
            return this.value;
        }

        public int Refresh(int oldI, int newI, Action hackDetected)
        {
            if (i != oldI)
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
                        this.value = int.Parse(ProtectedTypeUtils.GetInstance().DecodeType(v, i));
                    }
                }
                catch (Exception e)
                {
                    return this.Set(0, newI);
                }

                return this.Set(this.value, newI);
            }

            return this.Set(this.value, newI);
        }

        public int Add(int d, Action hackDetected)
        {
            if (s != CheckSum())
            {
                hackDetected();
                try
                {
                    if (this.v != "@@")
                    {
                        this.value = int.Parse(ProtectedTypeUtils.GetInstance().DecodeType(v, i));
                    }
                }
                catch (Exception e)
                {
                    return this.Set(d);
                }

                return this.Set(this.value + d);
            }

            return this.Set(this.value + d);
        }

        public int Get(Action hackDetected)
        {

            if (s != CheckSum())
            {
                hackDetected();
                try
                {
                    if (this.v != "@@")
                    {
                        this.value = int.Parse(ProtectedTypeUtils.GetInstance().DecodeType(v, i));
                    }
                }
                catch (Exception e)
                {
                    this.value = 0;
                }

                s = CheckSum();
                v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString(), i);
                return this.value;
            }
            
            return this.value;
        }


    }
}
