#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using MCP.DataModels.BaseModels;

namespace MCP.Google
{
    [Serializable]
    public class GoogleTranslationResponse : BaseModel
    {
        public List<TranslationItem> translations = new List<TranslationItem>();

        public GoogleTranslationResponse() : base()
        {

        }

    }
}
#endif
