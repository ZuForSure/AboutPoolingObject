#if UNITY_EDITOR
using System;
using MCP.DataModels.BaseModels;

namespace MCP.Google
{
    [Serializable]
    public class GoogleTranslationRequest : BaseModel
    {
        public string model = "nmt";
        public string format = "text";
        public string source = "en";
        public string target = "";
        public string q = "";
        public GoogleTranslationRequest() : base()
        {

        }

        public GoogleTranslationRequest(string targetLanguage, string text) : base()
        {
            this.target = targetLanguage;
            this.q = text;
        }

        public GoogleTranslationRequest(string targetLanguage, string text, string sourceLanguage) : base()
        {
            this.target = targetLanguage;
            this.source = sourceLanguage;
            this.q = text;
        }

    }
}
#endif
