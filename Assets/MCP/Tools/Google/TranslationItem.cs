#if UNITY_EDITOR
using System;
using MCP.DataModels.BaseModels;

namespace MCP.Google
{
    [Serializable]
    public class TranslationItem : BaseModel
    {
        public string translatedText = "";
        public string model = "";

        public TranslationItem() : base()
        {

        }

    }
}
#endif
