#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using System;

namespace MCP.Google
{
    public class GoogleHelper : MonoBehaviour
    {

        public string TranslationApiUrl = "";
        [HideInInspector]
        public float TimeOut = 200;


        [HideInInspector]
        public static GoogleHelper instance;

        public void Awake()
        {
            if (instance == null)
                instance = this;
            else
                GameObject.Destroy(this.gameObject);

        }

        public void SendTranslationApi(GoogleTranslationRequest request, Action<GoogleTranslationResponse> callback, Action<int> errorCallback, Action timeoutCallback)
        {
            //prepare request
            var form = new WWWForm();

            var formData = System.Text.Encoding.UTF8.GetBytes(request.ToJson());
            var header = form.headers;
            header.Remove("Content-Type");
            header.Add("Content-Type", "application/json");

            //Debug.Log("API URL: " + TranslationApiUrl);
            var www = new WWW(TranslationApiUrl, formData, header);

            StartCoroutine(WaitForReceiveInfo(callback, errorCallback, timeoutCallback, www));
        }



        private IEnumerator WaitForReceiveInfo(Action<GoogleTranslationResponse> callback, Action<int> errorCallback, Action timeoutCallback, WWW www)
        {
            //this.loadingCircular.Show (true);
            float time = 0;
            bool isTimeout = false;
            while (!isTimeout)
            {
                if (time >= TimeOut)
                {
                    isTimeout = true;
                    break;
                }
                else
                {
                    time += Time.deltaTime;
                }
                if (www.isDone)
                {
                    isTimeout = false;
                    break;
                }
                yield return null;
            }

            //this.loadingCircular.Hide ();
            if (!isTimeout)
            {
                if (www.text != null)
                {
                    Debug.Log ("Response: " + www.text);
                    //File.AppendAllText (Application.dataPath + "/Images/result.txt", "\r\n" + www.text);
                    // handle error in response
                    BaseGoogleResponse< GoogleTranslationResponse> result = new BaseGoogleResponse<GoogleTranslationResponse>();
                    try
                    {
                        result = JsonUtility.FromJson<BaseGoogleResponse<GoogleTranslationResponse>>(www.text);
                    }
                    catch (Exception e)
                    {
                        //ApiUtils.Log("Invalid server response: " + www.text);
                        //errorCallback (-1);
                        result.status = -1;
                    }

                    if (result != null && result.data != null)
                    {

                        callback(result.data);


                    }
                    else
                    {

                        errorCallback(-1);

                    }


                }
                else
                {
                    Debug.Log("Error message: Response is null");
                    errorCallback(-1);
                }

            }
            else
            {
                Debug.LogError("\n www is null or have error: " + www.error + "\n" + www.url);
                timeoutCallback();
            }
            www.Dispose();
        }

        public void TranslateMeExample(string targetLanguage, string textTotranslate, Action<TranslationItem> onCompleteCallback)
        {
            GoogleTranslationRequest request = new GoogleTranslationRequest("vi", "Translate me to Vietnamese");

            //call to login api
            GoogleHelper.instance.SendTranslationApi(
                request,
                (result) =>
                {
                    Debug.Log("Get translation text successful");
                    Debug.Log("result: " + result.translations[0].ToJson());
                    //do all things like login
                    onCompleteCallback(result.translations[0]);
                },
                (errorStatus) =>
                {
                    Debug.Log("Error: " + errorStatus);
                    //do some other processing here
                },
                () =>
                {
                    //timeout handler here
                    Debug.Log("Api call is timeout");
                }
            );
        }

    }
}
#endif

