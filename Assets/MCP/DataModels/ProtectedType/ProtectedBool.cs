using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using System;

namespace MCP.DataModels.ProtectedType
{
    [Serializable]
    public class ProtectedBool : BaseModel
    {
        public bool value = false;

        public string v = "";

        public long i = 0;

        public string s = "";

        public ProtectedBool(bool t)
        {
            this.i = ProtectedTypeUtils.GetInstance().GetGlobalId();
            this.value = t;
            this.s = CheckSum();
            this.v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString());
        }

        public string CheckSum()
        {
            return DataUtils.Md5Sum(ProtectedTypeUtils.GetInstance().EncodeType(i.ToString()) + value.ToString()).Substring(8, 8); ;
        }

        public bool Set(bool t)
        {
            if (ProtectedTypeUtils.GetInstance().GetGlobalId() < 1000)
                this.i = ProtectedTypeUtils.GetInstance().GetGlobalId();
            this.value = t;
            this.s = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString());
            return this.value;
        }

        public bool Get()
        {

                if (s != CheckSum())
                {
                    try
                    {
                        if (v != "@@")
                        {
                            this.value = bool.Parse(ProtectedTypeUtils.GetInstance().DecodeType(v));
                        }
                    }
                    catch (Exception e)
                    {
                        this.value = false;
                    }
                    s = CheckSum();
                    v = ProtectedTypeUtils.GetInstance().EncodeType(this.value.ToString());
                    return this.value;
                }
            
            return this.value;
        }


    }
}
