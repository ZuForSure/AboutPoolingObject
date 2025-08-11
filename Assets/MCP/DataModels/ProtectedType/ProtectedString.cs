using MCP.DataModel.Utils;
using MCP.DataModels.BaseModels;
using System;

namespace MCP.DataModels.ProtectedType
{
    [Serializable]
    public class ProtectedString : BaseModel
    {
        public string value = "";

        public string v = "";

        public long i = 0;

        public string c = "";

        public ProtectedString(string s)
        {
            this.i = ProtectedTypeUtils.GetInstance().GetGlobalId();
            value = s;
            c = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value);
        }

        public string CheckSum()
        {
            return DataUtils.Md5Sum(ProtectedTypeUtils.GetInstance().EncodeType(i.ToString()) + value.ToString()).Substring(8, 8);
        }

        public string Set(string s)
        {
            if (ProtectedTypeUtils.GetInstance().GetGlobalId() < 1000)
                this.i = ProtectedTypeUtils.GetInstance().GetGlobalId();
            this.value = s;
            c = CheckSum();
            v = ProtectedTypeUtils.GetInstance().EncodeType(this.value);
            return this.value;
        }

        public string Get()
        {

                if (c != CheckSum())
                {
                    try
                    {
                        if (v != "@@")
                        {
                            this.value = ProtectedTypeUtils.GetInstance().DecodeType(v);
                        }
                    }
                    catch (Exception e)
                    {
                        this.value = "";
                    }

                    c = CheckSum();
                    v = ProtectedTypeUtils.GetInstance().EncodeType(this.value);
                    return this.value;
                }
            

            return this.value;
        }


    }
}
