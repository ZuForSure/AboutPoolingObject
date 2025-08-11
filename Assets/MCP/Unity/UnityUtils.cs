using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;
using System.Text;
using MCP.DataModel.Utils;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;
#if ENABLE_DOWNLOAD_CONFIG
using Unity.SharpZipLib.GZip;
#endif
namespace MCP.Unity
{
    public static class UnityUtils
    {
        /*
		 * get md5 string
		 */
        public static string GetAvatarFolder(string playfabId)
        {
            string buildPoint = "/Dev";
#if ENABLE_MC_PRODUCTION
            buildPoint = "/Product";
#endif
            string trimmed = GetApplicationName(true) + buildPoint;
            stringBuilder.Length = 0;
            stringBuilder.Append(trimmed);
            stringBuilder.Append("/Avatar");
            stringBuilder.Append("/");
            stringBuilder.Append(playfabId);
            return stringBuilder.ToString();
        }
        public static string GetGameConfigFolder(string version)
        {

            string trimmed = GetFileShareDirectory(true);
            stringBuilder.Length = 0;
            stringBuilder.Append(trimmed);
            stringBuilder.Append("/GameConfig");
            stringBuilder.Append("/V_");
            stringBuilder.Append(version);
            return stringBuilder.ToString();
        }
        public static string GetFileShareDirectory(bool removeSpace)
        {
            string buildPoint = "/Dev";
#if ENABLE_MC_PRODUCTION
            buildPoint = "/Product";
#endif
            return GetApplicationName(true) + buildPoint;
        }
        public static string GetApplicationName(bool removeSpace)
        {
            if (removeSpace) return String.Concat(Application.productName.Where(c => !Char.IsWhiteSpace(c)));
            else return Application.productName;
        }

        #region User
        public static string GetPlayerPrefsString(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetString(key);
            }
            else
            {
                return "";
            }
        }

        public static int GetPlayerPrefsInt(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }
            else
            {
                return 0;
            }
        }

        public static float GetPlayerPrefsFloat(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key);
            }
            else
            {
                return 0f;
            }
        }
        #endregion

        #region UI
        public static void SetActiveUIGameObject(Behaviour uiBehaviour, bool isActive)
        {
            if (uiBehaviour != null)
                uiBehaviour.gameObject.SetActive(isActive);
        }

        public static void SetSelectableForImages(MonoBehaviour monoBehaviour, bool isSelectable)
        {
            if (monoBehaviour != null)
            {
                Image[] images = monoBehaviour.GetComponentsInChildren<Image>(true);
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].raycastTarget = isSelectable;
                }
            }
        }
        public static void SetSlider(Slider slider, float current, float require)
        {
            if (slider == null) return;
            slider.value = 1f * current / require;
        }
        public static void SetFill(Image image, float current, float require)
        {
            if (image == null) return;
            image.fillAmount = 1f * current / require;
        }
        #endregion

        #region GameObject

        /// <summary>
        /// Set Active Comopent if not null and active seft != isActive
        /// Return result
        /// </summary>
        public static bool SetActiveResult(Component gameObject, bool isActive)
        {
            if (gameObject != null && gameObject.gameObject.activeSelf != isActive)
            {
                gameObject.gameObject.SetActive(isActive);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Set Active GameObject if not null and active seft != isActive.
        /// Return result
        /// </summary>
        public static bool SetActiveResult(GameObject gameObject, bool isActive)
        {
            if (gameObject != null && gameObject.activeSelf != isActive)
            {
                gameObject.SetActive(isActive);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set Active Comopent if not null and active seft != isActive.
        /// </summary>
        public static void SetActive(Component component, bool isActive)
        {
            if (component != null && component.gameObject.activeSelf != isActive)
                component.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// Set Active GameObject if not null and active seft != isActive.
        /// </summary>
        public static void SetActive(GameObject gameObject, bool isActive)
        {
            if (gameObject != null && gameObject.activeSelf != isActive)
                gameObject.SetActive(isActive);
        }

        #endregion

        #region Transform

        public static Vector2 GetRandomPosition(Vector2 center, float radius, float radiusThickness)
        {
            if (radiusThickness >= radius)
            {
                return center;
            }

            float randomRadius = UnityEngine.Random.Range(radiusThickness, radius);

            float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2);

            float x = center.x + randomRadius * Mathf.Cos(randomAngle);
            float y = center.y + randomRadius * Mathf.Sin(randomAngle);

            return new Vector2(x, y);
        }


        public static void SetAndStretchToParentSize(RectTransform _mRect, RectTransform _parent)
        {
            if (_mRect == null || _parent == null)
                return;

            _mRect.anchoredPosition = _parent.position;
            _mRect.anchorMin = new Vector2(1, 0);
            _mRect.anchorMax = new Vector2(0, 1);
            _mRect.pivot = new Vector2(0.5f, 0.5f);
            _mRect.sizeDelta = _parent.rect.size;
            _mRect.transform.SetParent(_parent);

        }

        public static void SetStretchAnchorAll(RectTransform t)
        {
            if (t == null)
                return;

            t.pivot = Vector2.one * 0.5f;
            t.anchorMin = Vector2.zero;
            t.anchorMax = Vector2.one;
            t.anchoredPosition = Vector2.zero;
            t.sizeDelta = Vector2.zero;
        }

        public static void SetStretchAnchorLeft(RectTransform t)
        {
            if (t == null)
                return;

            SetStretchAnchorToSide(t, Vector2.up);
        }

        public static void SetStretchAnchorRight(RectTransform t)
        {
            if (t == null)
                return;

            SetStretchAnchorToFarSide(t, Vector2.right);
        }

        public static void SetStretchAnchorTop(RectTransform t)
        {
            if (t == null)
                return;

            SetStretchAnchorToFarSide(t, Vector2.up);
        }

        public static void SetStretchAnchorBottom(RectTransform t)
        {
            if (t == null)
                return;

            SetStretchAnchorToSide(t, Vector2.right);
        }

        public static void ForceRebuildLayoutCoroutine(RectTransform rect)
        {
            if (rect != null)
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        static void SetStretchAnchorToSide(RectTransform t, Vector2 stretch)
        {
            if (t == null)
                return;

            var old_size = t.rect.size;
            var perpendicular = Vector2.one - stretch;
            t.pivot = stretch * 0.5f;
            t.anchorMin = Vector2.zero;
            t.anchorMax = stretch;
            t.anchoredPosition = Vector2.zero;
            t.sizeDelta = Vector2.Scale(perpendicular, old_size);
        }

        static void SetStretchAnchorToFarSide(RectTransform t, Vector2 stretch)
        {
            if (t == null)
                return;

            var old_size = t.rect.size;
            t.pivot = (Vector2.one + stretch) * 0.5f;
            t.anchorMin = stretch;
            t.anchorMax = Vector2.one;
            t.anchoredPosition = Vector2.zero;
            t.sizeDelta = Vector2.Scale(stretch, old_size);
        }
        #endregion

        #region String 

        public static List<string> GetPlainStringVoiceList(string words)
        {
            string[] temp = GetPlainVoiceString(words).Split(' ');
            List<string> results = new List<string>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != "" && temp[i] != " ")
                    results.Add(temp[i]);
            }

            return results;
        }

        public static List<string> GetPlainStringTextList(string words)
        {
            string[] temp = Regex.Split(words, @"([^will,shall,can]n't)|('[^t][a-z]{0,1})|(\s)");
            List<string> results = new List<string>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != "" && temp[i] != " ")
                    results.Add(temp[i]);
            }

            return results;
        }
        public static string[] GetSentencseList(string text)
        {
            return Regex.Split(text, @"(?<=[\.!\?])[ \t]*");
        }


        public static List<string> GetStringList(string words)
        {
            string[] temp = words.Split(' ');
            List<string> results = new List<string>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != "" && temp[i] != " ")
                {
                    temp[i] = temp[i].Replace("<u>", "");
                    temp[i] = temp[i].Replace("</u>", "");
                    temp[i] = temp[i].Replace("<b>", "");
                    temp[i] = temp[i].Replace("</b>", "");
                    temp[i] = temp[i].Replace("<i>", "");
                    temp[i] = temp[i].Replace("</i>", "");
                    temp[i] = temp[i].Replace("</n>", "");

                    results.Add(temp[i]);
                }
            }

            return results;
        }


        public static string GetPlainVoiceString(string value)
        {
            string s = Regex.Replace(value.ToLower(), @"[^0-9a-zA-Z\']", " ");
            return s.Replace("'", "");
        }

        public static string GetPlainTextString(string value)
        {
            return Regex.Replace(value.ToLower(), @"[^0-9a-zA-Z]", "");
        }
        public static string RemoveSpaceTextString(string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }
        public static string TrimLineFeedTextString(string value)
        {
            return Regex.Replace(value, @"\n+", "\n");
        }
        public static string RegexInvalidCharacter(string value)
        {
            string s = Regex.Replace(value, @"[^0-9a-zA-Z]", "");
            return s;
        }
        public static string ReplaceString(string oldString, string value1 = null, string value2 = null, string value3 = null)
        {
            string s = Regex.Replace(oldString, "@@@studentName", value1);
            if (value2 != null)
                s = Regex.Replace(s, "@@@parentName", value2);
            if (value3 != null)
                s = Regex.Replace(s, "@@@days", value3);
            return s;
        }
        public static string ReplaceSpecialCharacter(string value, bool removeSpace, string replacement)
        {
            if (removeSpace)
                return Regex.Replace(value, @"[^0-9a-zA-Z\'’]", replacement);
            else
                return Regex.Replace(value, @"[^0-9a-zA-Z\s\'’]", replacement);
        }

        /// <summary>
        /// Replace white space character with another string
        /// </summary>
        /// <param name="value">Your original string</param>
        /// <param name="replacement">Your replacement string</param>
        /// <param name="allKindSpace">Will remove all whitespace characters (space, tabs, line breaks...)</param>
        /// <returns>Whitespace trimmed string</returns>
        public static string ReplaceWhiteSpace(string value, string replacement, bool allKindSpace = false)
        {
            return allKindSpace ? Regex.Replace(value, @"\s", replacement) : value.Replace(" ", replacement);
        }

        public static string GetLowerTextString(string value)
        {
            return Regex.Replace(value.ToLower(), @"[^0-9a-zA-Z\'’]", " ");
        }
        public static bool CompareVoiceText(string n1, string n2)
        {
            if (Regex.Replace(n1.ToLower(), @"[^0-9a-zA-Z]", "") == Regex.Replace(n2.ToLower(), @"[^0-9a-zA-Z]", ""))
                return true;
            else
                return false;
        }

        public static string GetValidateString(string words, int index)
        {
            string[] temp = words.Split('@');
            return temp[index];
        }

        #endregion

        #region Graphic

        /// <summary>
        /// Set Color if graphic not null
        /// </summary>
        public static void SetColor(Graphic graphic, Color color)
        {
            if (graphic)
            {
                graphic.color = color;
            }
        }

        /// <summary>
        /// SetActive raycastTarget for graphic array
        /// </summary>
        public static void SetRaycast(Graphic[] graphics, bool active)
        {
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].raycastTarget = active;
            }
        }
        public static void SetMaterial(Image[] graphic, Material material)
        {
            for (int i = 0; i < graphic.Length; i++)
            {
                graphic[i].material = material;
            }
        }
        public static void SetAvatar(Image image, string avatarURL, Sprite defaultImage = null)
        {
            if (!string.IsNullOrEmpty(avatarURL) && avatarURL.Contains("http"))
            {
                UnityUtils.DownloadImage(avatarURL, 
                    successCallback =>
                    { 
                        SetSpriteForImage(image, UnityUtils.CreateSprite(successCallback));
                        SetActive(image, true);
                    },
                    () => 
                    {
                        //SetSpriteForImage(image, defaultImage); 
                        SetActive(image, false);
                    });
            }
            //else SetSpriteForImage(image, defaultImage);
            else
            {
                SetActive(image, false);
            }
        }

        #endregion

        #region Sprite

        /// <summary>
        /// Set sprite if image not null
        /// </summary>
        ///
        public static Texture2D DeCompress(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        public static void SetSpriteForImage(Image image, Sprite sprite)
        {
            if (image != null)
                image.sprite = sprite;
        }

        public static Sprite CreateSprite(Texture2D sourceTex)
        {
            Sprite s = Sprite.Create(sourceTex, new Rect(new Vector2(0, 0), new Vector2(sourceTex.width, sourceTex.height)), new Vector2(0.5f, 0.5f));
            return s;
        }

        public static Texture2D Resize(float scale, Texture2D sourceTex)
        {
            int w = (int)(sourceTex.width * scale);
            int h = (int)(sourceTex.height * scale);
            Texture2D b = new Texture2D(w, h, TextureFormat.RGB24, false);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color c = sourceTex.GetPixel((int)(x / scale), (int)(y / scale));
                    b.SetPixel(x, y, c);
                }
            }
            b.Apply();
            Texture2D.Destroy(sourceTex);
            return b;
        }

        public static Texture2D Resize(int height, Texture2D sourceTex)
        {
            float scale = 1;
            if (sourceTex.width > sourceTex.height)
            {
                scale = height * 1.0f / sourceTex.height;
            }
            else
            {
                scale = height * 1.0f / sourceTex.width;
            }
            int w = (int)(sourceTex.width * scale);
            int h = (int)(sourceTex.height * scale);
            Texture2D b = new Texture2D(w, h, TextureFormat.ARGB32, false);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color c = sourceTex.GetPixel((int)(x / scale), (int)(y / scale));
                    b.SetPixel(x, y, c);
                }
            }
            b.Apply();
            Texture2D.Destroy(sourceTex);
            return b;
        }

        public static Texture2D CropSquare(Texture2D sourceTex)
        {
            Texture2D b;
            if (sourceTex.width > sourceTex.height)
            {
                int x1 = (sourceTex.width - sourceTex.height) / 2;
                int x2 = x1 + sourceTex.height;
                b = new Texture2D(sourceTex.height, sourceTex.height, TextureFormat.ARGB32, false);
                for (int x = x1; x < x2; x++)
                {
                    for (int y = 0; y < sourceTex.height; y++)
                    {
                        Color c = sourceTex.GetPixel(x, y);
                        b.SetPixel(x - x1, y, c);
                    }
                }
            }
            else
            {
                int y1 = (sourceTex.height - sourceTex.width) / 2;
                int y2 = y1 + sourceTex.width;
                b = new Texture2D(sourceTex.width, sourceTex.width, TextureFormat.ARGB32, false);
                for (int x = 0; x < sourceTex.width; x++)
                {
                    for (int y = y1; y < y2; y++)
                    {
                        Color c = sourceTex.GetPixel(x, y);
                        b.SetPixel(x, y - y1, c);
                    }
                }
            }

            b.Apply();
            Texture2D.Destroy(sourceTex);
            return b;
        }

        public static Texture2D ClearColor(Texture2D sourceTex, Color color)
        {
            Texture2D b;
            b = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.ARGB32, false);
            for (int x = 0; x < sourceTex.width; x++)
            {
                for (int y = 0; y < sourceTex.height; y++)
                {
                    Color c = sourceTex.GetPixel(x, y);
                    if (c == color)
                        b.SetPixel(x, y, new Color(0, 0, 0, 0));
                    else
                        b.SetPixel(x, y, c);
                }
            }

            b.Apply();
            Texture2D.Destroy(sourceTex);
            return b;
        }

        public static Texture2D CropSquare(Texture2D sourceTex, Vector2 pos, int size)
        {
            Texture2D b;
            b = new Texture2D(size, size, TextureFormat.ARGB32, false);
            int x1 = (int)pos.x - size / 2;
            int x2 = (int)pos.x + size / 2;
            int y1 = (int)pos.y - size / 2;
            int y2 = (int)pos.y + size / 2;
            for (int x = x1; x < x2; x++)
            {
                for (int y = y1; y < y2; y++)
                {
                    Color c = sourceTex.GetPixel(x, y);
                    b.SetPixel(x - x1, y - y1, c);
                }
            }
            b.Apply();
            Texture2D.Destroy(sourceTex);
            return b;
        }

        public static Texture2D CropCircle(Texture2D sourceTex)
        {
            float r = (sourceTex.width + sourceTex.height) / 4;
            float cx = sourceTex.width / 2;
            float cy = sourceTex.height / 2;
            Texture2D b = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.ARGB32, false);
            Sprite s;
            for (int x = 0; x < sourceTex.width; x++)
            {
                for (int y = 0; y < sourceTex.height; y++)
                {
                    Color c = sourceTex.GetPixel(x, y);
                    if (r * r >= (x - cx) * (x - cx) + (y - cy) * (y - cy))
                        b.SetPixel(x, y, c);
                    else
                        b.SetPixel(x, y, Color.clear);
                }
            }
            b.Apply();
            Texture2D.Destroy(sourceTex);
            return b;
        }

        public static Texture2D CropCircle(Texture2D sourceTex, Vector2 pos, int size)
        {
            Texture2D b;
            b = new Texture2D(size, size, TextureFormat.ARGB32, false);
            int x1 = (int)pos.x - size / 2;
            int x2 = (int)pos.x + size / 2;
            int y1 = (int)pos.y - size / 2;
            int y2 = (int)pos.y + size / 2;
            float r = size / 2.0f;

            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    Color c = sourceTex.GetPixel(x, y);
                    if (r * r >= (x - pos.x) * (x - pos.x) + (y - pos.y) * (y - pos.y))
                        b.SetPixel(x - x1, y - y1, c);
                    else
                        b.SetPixel(x - x1, y - y1, Color.clear);
                }
            }
            b.Apply();
            Texture2D.Destroy(sourceTex);
            return b;
        }
        public static async void DownloadImage(string url, Action<Texture2D> onSuccessCallback, Action onErrorCallback)
        {
            url = url.Replace("%", "");
            string saveFilePath = GetSaveImagePath(url);

#if !UNITY_SERVER
            if (ES3.FileExists(saveFilePath))
            {
                onSuccessCallback(ES3.LoadImage(saveFilePath));
            }
            else
            {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
                {
                    // begin request:
                    var asyncOp = www.SendWebRequest();

                    // await until it's done: 
                    while (asyncOp.isDone == false)
                        await Task.Delay(1000 / 30);//30 hertz

                    // read results:
                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"{www.error}, URL:{www.url}");
                        onErrorCallback();
                    }
                    else
                    {
                        // return valid results:
                        onSuccessCallback(DownloadHandlerTexture.GetContent(www));
                        ES3.SaveRaw(www.downloadHandler.data, saveFilePath);
                    }

                    www.Dispose();
                }
            }
#endif
        }
        public static async void DownloadFile(string url, Action<string> onSuccessCallback, Action onErrorCallback)
        {
            url = url.Replace("%", "");
            if (string.IsNullOrEmpty(url)) return;
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                // begin request:
                var asyncOp = www.SendWebRequest();

                // await until it's done: 
                while (asyncOp.isDone == false)
                    await Task.Delay(1000 / 30);//30 hertz

                // read results:
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{www.error}, URL:{www.url}");
                    onErrorCallback();
                }
                else
                {
                    if (www.downloadHandler.data != null)
                    {
                        string result = UnzipStreamToString(www.downloadHandler.data);
                        onSuccessCallback(result);
                    }
                    else
                    {
                        onErrorCallback();
                    }
                }
                www.Dispose();
            }
        }
        public static string UnzipStreamToString(byte[] inputStream)
        {
            MemoryStream ms = new MemoryStream(inputStream);
            MemoryStream outStream = new MemoryStream();
#if ENABLE_DOWNLOAD_CONFIG
            GZip.Decompress(ms, outStream, true);
#endif
            return Encoding.UTF8.GetString(outStream.ToArray(), 0, outStream.ToArray().Length);
        }
        public static byte[] ZipFileToStream(string filename)
        {
            MemoryStream ms = new MemoryStream();
#if ENABLE_DOWNLOAD_CONFIG
            GZip.Compress(File.OpenRead(filename), ms, true);
#endif
            byte[] result = ms.ToArray();
            return result;
        }
        public static string GetSaveImagePath(string url)
        {
            int queryIndex = url.IndexOf('?');
            if (queryIndex != -1)
            {
                url = url.Substring(0, queryIndex);
            }

            string[] keys = url.Split('/');
            string path = keys[keys.Length - 1];
            string extension = Path.GetExtension(path);
            string saveFilePath = DataUtils.Md5Sum(url) + extension;
            return saveFilePath;
        }
#endregion

#region Text
        static StringBuilder stringBuilder = new StringBuilder();

        public static void SetText(Text text, string s)
        {
            if (text != null)
                text.text = s;
        }


        public static void SetText(Text text, int s)
        {
            if (text != null)
                text.text = s.ToString();
        }

        /// <summary>
        /// Set TMP_Text.text if not null
        /// </summary>
        public static void SetText(TMP_Text tmp_Text, string s)
        {
            if (tmp_Text != null)
            {
                stringBuilder.Length = 0;
                stringBuilder.Append(s);
                tmp_Text.SetText(stringBuilder);
            }
        }
        public static void SetText(TMP_Text tmp_Text, List<string> str)
        {
            if (tmp_Text != null)
            {
                stringBuilder.Length = 0;
                foreach (string s in str)
                {
                    stringBuilder.Append(s);
                }

                tmp_Text.SetText(stringBuilder);
            }
        }
        public static string GetText(List<string> str)
        {
            stringBuilder.Length = 0;
            foreach (string s in str)
            {
                stringBuilder.Append(s);
            }
            return stringBuilder.ToString();
        }
        public static string AddLinkFormat(string text, int id)
        {
            return "<link=" + id.ToString() + ">" + text + "</link>";
        }

        public static string AddColorFormat(string text, string color)
        {
            return "<color=" + color + ">" + text + "</color>";
        }

        public static void AddColorWithoutChangingTextFormat(Text text, string color)
        {
            Color colorToChange;
            if (ColorUtility.TryParseHtmlString(color, out colorToChange))
                text.color = colorToChange;
        }

        public static void AddColorWithoutChangingTextMPFormat(TMP_Text text, string color)
        {
            Color colorToChange;
            if (ColorUtility.TryParseHtmlString(color, out colorToChange))
                text.color = colorToChange;
        }

        public static string AddUnderlineFormat(string text)
        {
            return "<u>" + text + "</u>";
        }


        public static string AddBoldFormat(string text)
        {
            return "<b>" + text + "</b>";
        }

        public static string GetStringNumber(float t, int f = -1)
        {
            if (Mathf.Abs(t) > 99 || f == 0)
                return t.ToString("F0");
            else if (Mathf.Abs(t) > 9 || f == 1)
                return t.ToString("F1");
            else if (Mathf.Abs(t) <= 9 || f == 2)
                return t.ToString("F2");
            return t.ToString("F2");
        }

        public static T GetRandomValue<T>(T[] array)
        {
            if (array == null || array.Length == 0)
                return default;

            int randomIndex = UnityEngine.Random.Range(0, array.Length);
            return array[randomIndex];
        }

        public static int GetRandomExceptOne(int exception, int[] arrayToRandom)
        {
            List<int> list = new List<int>(arrayToRandom) { };
            list.RemoveAll(x => x == exception);
            int random = list[UnityEngine.Random.Range(0, list.Count)];
            return random;
        }
        public static int GetRandomExceptThese(int[] exceptions, int[] arrayToRandom)
        {
            List<int> list = new List<int>(arrayToRandom) { };
            foreach (int exception in exceptions)
            {
                list.RemoveAll(x => x == exception);
            }
            int random = list[UnityEngine.Random.Range(0, list.Count)];
            return random;
        }
        public static int GetRandomExceptThese(int[] exceptions, int lengthToRandom)
        {
            List<int> list = new List<int>() { };
            for (int i = 0; i < lengthToRandom; i++)
                list.Add(i);

            foreach (int exception in exceptions)
            {
                list.RemoveAll(x => x == exception);
            }

            int random = list[UnityEngine.Random.Range(0, list.Count)];
            return random;
        }

        public static string GetStringWithSignNumber(float t)
        {
            if (t == 0)
                return "";
            else if (Mathf.Abs(t) > 99)
            {
                if (t > 0)
                    return "+" + t.ToString("F0");
                else
                    return t.ToString("F0");
            }
            else if (Mathf.Abs(t) > 9)
            {
                if (t > 0)
                    return "+" + t.ToString("F1");
                else
                    return t.ToString("F1");
            }
            else
            {
                if (t > 0)
                    return "+" + t.ToString("F2");
                else
                    return t.ToString("F2");
            }
        }



        public static class TextDetectCharacterHelper
        {
            /// <summary>
            /// Text tag example: localize...<a id=10>clickable Text</a>...
            /// will return 10 when click clickable text
            /// </summary>
            /// <param name="textTarget"></param>
            /// <param name="screenPoint"></param>
            /// <param name="cam"></param>
            /// <returns></returns>
            public static (bool, string) FindLinkInTouchText(Text textTarget, Vector2 screenPoint, Camera cam)
            {
                // Convert the touch/click position to local position of the Text element.
                RectTransform rectTransform = textTarget.rectTransform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, cam, out Vector2 localPoint);

                // Create a TextGenerator to process the text and find the character index.
                TextGenerator textGenerator = new TextGenerator();
                textGenerator.Populate(textTarget.text, textTarget.GetGenerationSettings(textTarget.rectTransform.rect.size));

                int characterIndex = -1;

                Debug.Log("textGenerator.characterCount " + textGenerator.characterCount);

                for (int i = 0; i < textGenerator.characterCount; i++)
                {
                    UICharInfo charInfo = textGenerator.characters[i];

                    // Calculate character bounds.
                    Rect charBounds = new Rect(
                        charInfo.cursorPos.x,
                        charInfo.cursorPos.y - textTarget.fontSize,
                        charInfo.charWidth,
                        textTarget.fontSize
                    );

                    // Check if the local point is within the character's bounding box.
                    if (charBounds.Contains(localPoint))
                    {
                        characterIndex = i;
                        break;
                    }
                }

                if (characterIndex == -1)
                {
                    Debug.Log("characterIndex: valid");
                    return (false, null);
                }

                var data = ExtractLinkTagIndices(textTarget.text);

                return IsIndexInsideLinkTag(characterIndex, data);
            }

            private static (bool, string) IsIndexInsideLinkTag(int index, List<LinkTagIndices> colorTagIndices)
            {
                foreach (LinkTagIndices tagIndex in colorTagIndices)
                {
                    if (index >= tagIndex.StartIndex && index <= tagIndex.EndIndex)
                    {
                        return (true, tagIndex.Context); // The index is inside a color tag.
                    }
                }
                return (false, null); // The index is not inside any color tag.
            }

            public static List<LinkTagIndices> ExtractLinkTagIndices(string input)
            {
                List<LinkTagIndices> tagIndices = new List<LinkTagIndices>();

                // Define the regular expression pattern to match color tags.
                // string pattern = "<color=.*?</color>";
                string pattern = "<a id=.*?</a>";

                // Find all matches of the pattern in the input text.
                MatchCollection matches = Regex.Matches(input, pattern);

                foreach (Match match in matches)
                {
                    LinkTagIndices tagIndex = new LinkTagIndices
                    {
                        StartIndex = match.Index,
                        EndIndex = match.Index + match.Length - 1, // Adjust for 0-based index
                        Context = ExtractContentBetweenLinkTags(match.Value)
                    };
                    tagIndices.Add(tagIndex);
                }

                return tagIndices;
            }

            private static string ExtractContentBetweenLinkTags(string input)
            {
                // Define the regular expression pattern to match color tags.
                // string pattern = "<color=.*?>(.*?)</color>";
                string pattern = "<a id=(.*?)>";

                // Use Regex.Match to find the first match.
                Match match = Regex.Match(input, pattern);

                // Check if a match was found.
                if (match.Success)
                {
                    // Get the captured content between the color tags.
                    string content = match.Groups[1].Value;

                    return content;
                }
                else
                {
                    // No matching color tags found, return the input as-is.
                    return input;
                }
            }
        }


        public struct LinkTagIndices
        {
            public int StartIndex { get; set; }

            public int EndIndex { get; set; }

            public string Context { get; set; }
        }

#endregion

#region Animation

        /// <summary>
        /// SetTrigger if animator not null
        /// </summary>
        public static void SetTrigger(Animator animator, int hash)
        {
            if (animator)
            {
                animator.SetTrigger(hash);
            }
        }

        /// <summary>
        /// SetTrigger if animator not null
        /// </summary>
        public static void SetTrigger(Animator animator, string name)
        {
            if (animator)
            {
                animator.SetTrigger(name);
            }
        }

        /// <summary>
        /// Play if animator not null
        /// </summary>
        public static void Play(Animator animator, int hash)
        {
            if (animator)
            {
                animator.Play(hash);
            }
        }

        /// <summary>
        /// SetBool if animator not null
        /// </summary>
        public static void SetBool(Animator animator, int hash, bool boolean)
        {
            if (animator)
            {
                animator.SetBool(hash, boolean);
            }
        }
        public static readonly int HASH_SOLO_CHOOSE = Animator.StringToHash("Solo_Choose");
        public static readonly int HASH_SOLO_CLOSE = Animator.StringToHash("Solo_Close");
        public static readonly int HASH_SOLO_OPEN = Animator.StringToHash("Solo_Open");
        public static readonly int HASH_SOLO_DONE = Animator.StringToHash("Solo_Done");

        public static readonly int HASH_GROUP_CHOOSE = Animator.StringToHash("Group_Choose");
        public static readonly int HASH_GROUP_CLOSE = Animator.StringToHash("Group_Close");
        public static readonly int HASH_GROUP_OPEN = Animator.StringToHash("Group_Open");
        public static readonly int HASH_GROUP_DONE = Animator.StringToHash("Group_Done");

        public static readonly int HASH_CORRUPTED_CHOOSE = Animator.StringToHash("Corrupted_Choose");
        public static readonly int HASH_CORRUPTED_CLOSE = Animator.StringToHash("Corrupted_Close");
        public static readonly int HASH_CORRUPTED_OPEN = Animator.StringToHash("Corrupted_Open");
        public static readonly int HASH_CORRUPTED_DONE = Animator.StringToHash("Corrupted_Done");

        public static readonly int HASH_LEVELUP_ACTIVE = Animator.StringToHash("Active");
        public static readonly int HASH_LEVELUP_IDLE = Animator.StringToHash("Idle");
#endregion

        public static string GetOSFamily()
        {
            return RemoveInvalidCharacter(UnityEngine.SystemInfo.operatingSystemFamily.ToString());
        }

        public static string RemoveInvalidCharacter(string value)
        {
            string s = Regex.Replace(value, @"[^0-9a-zA-Z]", "");
            return s;
        }

        public static string GetDeviceID()
        {
#if UNITY_IOS && ENABLE_IOS_KEYCHAIN
            string applicationKey = "";
            applicationKey = FSG.iOSKeychain.Keychain.GetValue("deviceId");
            if (applicationKey == "")
            {
                //Debug.Log("not Exists");
                FSG.iOSKeychain.Keychain.SetValue("deviceId", UnityEngine.SystemInfo.deviceUniqueIdentifier);
                return UnityEngine.SystemInfo.deviceUniqueIdentifier;
            }
            else
            {
                //Debug.Log("Exists");
                return applicationKey;
            }
#else
            return UnityEngine.SystemInfo.deviceUniqueIdentifier;
#endif
        }

        public static string GetDeviceOS()
        {
            return Application.platform.ToString();
        }

        public static string GetDeviveLanguageCode()
        {
            string id = "en";
            return Application.systemLanguage switch
            {
                SystemLanguage.Afrikaans => "af",
                SystemLanguage.Arabic => "ar",
                SystemLanguage.Basque => id,
                SystemLanguage.Belarusian => "be",
                SystemLanguage.Bulgarian => "bg",
                SystemLanguage.Catalan => "ca",
                SystemLanguage.Chinese => "zh-CN",
                SystemLanguage.Czech => "cs",
                SystemLanguage.Danish => "da",
                SystemLanguage.Dutch => "nl",
                SystemLanguage.English => id,
                SystemLanguage.Estonian => "et",
                SystemLanguage.Faroese => id,
                SystemLanguage.Finnish => "fi",
                SystemLanguage.French => "fr",
                SystemLanguage.German => "de",
                SystemLanguage.Greek => "el",
                SystemLanguage.Hebrew => "iw",
                SystemLanguage.Hungarian => "hu",
                SystemLanguage.Icelandic => "is",
                SystemLanguage.Indonesian => "id",
                SystemLanguage.Italian => "it",
                SystemLanguage.Japanese => "ja",
                SystemLanguage.Korean => "ko",
                SystemLanguage.Latvian => "lv",
                SystemLanguage.Lithuanian => "lt",
                SystemLanguage.Norwegian => "no",
                SystemLanguage.Polish => "pl",
                SystemLanguage.Portuguese => "pt",
                SystemLanguage.Romanian => "ro",
                SystemLanguage.Russian => "ru",
                SystemLanguage.SerboCroatian => "sr",
                SystemLanguage.Slovak => "sk",
                SystemLanguage.Slovenian => "sl",
                SystemLanguage.Spanish => "es",
                SystemLanguage.Swedish => "sv",
                SystemLanguage.Thai => "th",
                SystemLanguage.Turkish => "tr",
                SystemLanguage.Ukrainian => "uk",
                SystemLanguage.Vietnamese => "vi",
                SystemLanguage.ChineseSimplified => "zh-CN",
                SystemLanguage.ChineseTraditional => "zh-TW",
                SystemLanguage.Unknown => id,
                _ => id
            };
        }

        public static bool IsValidVietNamPhoneNumber(string phoneNum)
        {
            if (string.IsNullOrEmpty(phoneNum))
                return false;
            string sMailPattern = @"^((09(\d){8})|
        |(032(\d){7})|(033(\d){7})|(034(\d){7})|(035(\d){7})|(036(\d){7})|(037(\d){7})|(038(\d){7})|(039(\d){7})|
        |(056(\d){7})|(058(\d){7})|(059(\d){7})|
        |(070(\d){7})|(076(\d){7})|(077(\d){7})|(078(\d){7})|(079(\d){7})|
        |(081(\d){7})|(082(\d){7})|(083(\d){7})|(084(\d){7})|(085(\d){7})|(086(\d){7})|(088(\d){7})|(089(\d){7})|
        |(01(\d){9}))$";
            return Regex.IsMatch(phoneNum.Trim(), sMailPattern);
        }


        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public static Vector2 SnapTo(ScrollRect scroller, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();

            var contentPos = (Vector2)scroller.transform.InverseTransformPoint(scroller.content.position);
            var childPos = (Vector2)scroller.transform.InverseTransformPoint(child.position);
            var endPos = contentPos - childPos;
            // If no horizontal scroll, then don't change contentPos.x
            /*
            if (!scroller.horizontal) 
            {
                endPos.x = contentPos.x;
            } 
            // If no vertical scroll, then don't change contentPos.y
            if (!scroller.vertical) endPos.y = contentPos.y;
            */
            return new Vector2(endPos.x, endPos.y);
        }
        public static Vector2 SnapToNew(ScrollRect scroller, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();

            var contentPos = (Vector2)scroller.transform.InverseTransformPoint(scroller.content.position);
            var childPos = (Vector2)scroller.transform.InverseTransformPoint(child.position);
            var endPos = contentPos - childPos;
            // If no horizontal scroll, then don't change contentPos.x

            if (!scroller.horizontal)
            {
                endPos.x = contentPos.x;
            }
            // If no vertical scroll, then don't change contentPos.y
            if (!scroller.vertical) endPos.y = contentPos.y;

            return new Vector2(endPos.x, endPos.y);
        }

    }



}



