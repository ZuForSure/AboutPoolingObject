#if UNITY_EDITOR
using System;
using MCP.DataModels.BaseModels;

namespace MCP.Google {
	[Serializable]
	public class BaseGoogleResponse<TResult>  where TResult : BaseModel  {

		public int code  = 0;
		public int status = 0;
		public string error = "";
		public TResult data;

		public string timestamp = "";


		public BaseGoogleResponse() : base() {
		}

	}
}
#endif
