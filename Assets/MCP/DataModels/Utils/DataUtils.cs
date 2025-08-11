using MCP.DataModels.BaseModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MCP.DataModel.Utils
{
    public static class DataUtils
    {
        /*
        public static List<T> GetList<T>(List<BaseItem> items) where T : BaseItem
        {
            List<T> list = new List<T>();

            foreach (BaseItem item in items)
            {
                if (typeof(T).Name.Contains(item.itemClass.ToString()))
                {
                    T newItem = (T)Activator.CreateInstance(typeof(T));
                    CopyObject(newItem, item);
                    AttributeHelper.SetValueCustomeData<T>(newItem, newItem.customData);
                    list.Add(newItem);
                }
            }

            return list;
        }

        public static T GetItem<T>(List<BaseItem> items, int index) where T : BaseItem
        {
            int count = 0;
            foreach (BaseItem item in items)
            {
                if (typeof(T).Name.Contains(item.itemClass.ToString()))
                {
                    if (count == index)
                    {
                        T newItem = (T)Activator.CreateInstance(typeof(T));
                        CopyObject(newItem, item);
                        AttributeHelper.SetValueCustomeData<T>(newItem, newItem.customData);
                        return newItem;
                    }
                    count++;
                }
            }
            return null;
        }

        public static T GetItem<T>(BaseItem item) where T : BaseItem
        {
            T newItem = (T)Activator.CreateInstance(typeof(T));
            CopyObject(newItem, item);
            AttributeHelper.SetValueCustomeData<T>(newItem, newItem.customData);
            return newItem;
        }*/

        public static Dictionary<string, object> ToDictionary(object obj)
        {
            Dictionary<string, object> extractFields = new Dictionary<string, object>();

            if (obj != null)
            {
                // Get the type of obj.
                Type myType = obj.GetType();

                FieldInfo[] myFields = myType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                // Display the attributes for each of the members of obj.
                for (int i = 0; i < myFields.Length; i++)
                {
                    if (!myFields[i].IsNotSerialized)
                    {
                        object child = obj.GetType().InvokeMember(myFields[i].Name, BindingFlags.GetField, null, obj, null);
                        FieldInfo[] childFields = child.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                        if (childFields.Length > 0)
                        {
                            extractFields.Add(myFields[i].Name, DataUtils.ToDictionary(child));
                        }
                        else
                        {
                            if (IsGenericList(child))
                            {
                                Type[] genericTypes = child.GetType().GetGenericArguments();
                                if (genericTypes.Length == 1)
                                {


                                    if (typeof(BaseModel).IsAssignableFrom(genericTypes[0]))
                                    {
                                        Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                        IList res = (IList)Activator.CreateInstance(t, new object[] { child });

                                        List<Dictionary<string, object>> tmpDictionaryList = new List<Dictionary<string, object>>();
                                        foreach (var item in res)
                                        {
                                            Dictionary<string, object> tmpElement = DataUtils.ToDictionary(item);
                                            tmpDictionaryList.Add(tmpElement);
                                        }
                                        extractFields.Add(myFields[i].Name, tmpDictionaryList);
                                    }
                                    else
                                    {
                                        Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                        IList res = (IList)Activator.CreateInstance(t, new object[] { child });
                                        Dictionary<string, object> tmpDictionaryList = new Dictionary<string, object>();
                                        int index = 0;
                                        foreach (var item in res)
                                        {
                                            tmpDictionaryList.Add(index.ToString(), item);
                                            index++;
                                        }
                                        extractFields.Add(myFields[i].Name, tmpDictionaryList);
                                    }
                                }
                            }
                            else
                            {
                                extractFields.Add(myFields[i].Name, child);
                            }
                        }
                    }
                }
            }
            return extractFields;
        }

        public static Dictionary<string, string> ToDictionaryString(object obj)
        {
            Dictionary<string, string> extractFields = new Dictionary<string, string>();

            if (obj != null)
            {
                // Get the type of obj.
                Type myType = obj.GetType();

                FieldInfo[] myFields = myType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                // Display the attributes for each of the members of obj.
                for (int i = 0; i < myFields.Length; i++)
                {
                    object child = obj.GetType().InvokeMember(myFields[i].Name, BindingFlags.GetField, null, obj, null);
                    extractFields.Add(myFields[i].Name, JsonConvert.SerializeObject(child));
                }
            }
            return extractFields;
        }

        public static void FromDictionary<T>(T obj, Dictionary<string, object> input) where T : BaseModel
        {
            foreach (KeyValuePair<string, object> pair in input)
            {
                try
                {
                    FieldInfo field = typeof(T).GetField(pair.Key);
                    //Debug.Log("Field " + field.Name);

                    if (field != null)
                    {
                        if ((field.FieldType.IsGenericType && (field.FieldType.GetGenericTypeDefinition() == typeof(List<>))))
                        {
                            Type[] genericTypes = field.FieldType.GetGenericArguments();
                            if (genericTypes.Length == 1)
                            {
                                if (genericTypes[0].IsAssignableFrom(typeof(BaseModel)))
                                {
                                    Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                    IList res = (IList)Activator.CreateInstance(t);

                                    MethodInfo method = typeof(DataUtils).GetMethod("FromDictionary");
                                    MethodInfo generic = method.MakeGenericMethod(genericTypes[0]);
                                    //scan through all item in list
                                    foreach (Dictionary<string, object> item in (List<Dictionary<string, object>>)pair.Value)
                                    {
                                        res.Add(generic.Invoke(null, new object[] { item }));
                                    }
                                    field.SetValue(obj, res);
                                }
                                else
                                {
                                    Type t = typeof(List<>).MakeGenericType(genericTypes[0]);
                                    IList res = (IList)Activator.CreateInstance(t);

                                    foreach (KeyValuePair<string, object> item in (Dictionary<string, object>)pair.Value)
                                    {
                                        res.Add(item.Value);
                                    }

                                    field.SetValue(obj, res);
                                }
                            }
                        }
                        else
                        {
                            if (pair.Value.GetType() == typeof(Dictionary<string, object>))
                            {
                                MethodInfo method = typeof(DataUtils).GetMethod("FromDictionary");
                                MethodInfo generic = method.MakeGenericMethod(field.FieldType);
                                field.SetValue(obj, generic.Invoke(null, new object[] { pair.Value }));
                            }
                            else if (pair.Value.GetType().IsArray)
                            {
                                field.SetValue(obj, pair.Value);
                            }
                            else if (field.FieldType.IsEnum)
                            {
                                field.SetValue(obj, Enum.Parse(field.FieldType, pair.Value.ToString()));
                            }
                            else if (field.FieldType.IsClass)
                            {
                                if (field.FieldType.ToString() == "System.String")
                                {
                                    field.SetValue(obj, pair.Value.ToString());
                                }
                                else
                                {
                                    MethodInfo method = typeof(DataUtils).GetMethod("CreateFromJSON", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                                    MethodInfo generic = method.MakeGenericMethod(field.FieldType);
                                    field.SetValue(obj, generic.Invoke(null, new object[] { pair.Value }));
                                }
                            }
                            else
                            {
                                switch (field.FieldType.ToString())
                                {
                                    case "System.Int32":
                                        field.SetValue(obj, (int)Convert.ChangeType(pair.Value, typeof(int)));
                                        break;
                                    case "System.Int64":
                                        field.SetValue(obj, (long)Convert.ChangeType(pair.Value, typeof(long)));
                                        break;
                                    case "System.Single":
                                        field.SetValue(obj, (float)Convert.ChangeType(pair.Value, typeof(float)));
                                        break;
                                    case "System.Double":
                                        double.TryParse(pair.Value.ToString(), out double x);
                                        field.SetValue(obj, (double)Convert.ChangeType(pair.Value, typeof(double)));
                                        break;
                                    case "System.DateTime":
                                        field.SetValue(obj, (DateTime)Convert.ChangeType(pair.Value, typeof(System.DateTime)));
                                        break;
                                    case "System.Boolean":
                                        field.SetValue(obj, (bool)Convert.ChangeType(pair.Value, typeof(System.Boolean)));
                                        break;
                                    case "System.String":
                                        field.SetValue(obj, (string)Convert.ChangeType(pair.Value, typeof(System.String)));
                                        break;
                                    default:
                                        field.SetValue(obj, pair.Value.ToString());
                                        break;
                                }
                                //Debug.Log("Get Value " + field.GetValue(obj));
                            }

                        }

                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        public static void FromDictionary<T>(T obj, Dictionary<string, string> input) where T : BaseModel
        {
            foreach (KeyValuePair<string, string> pair in input)
            {
                try
                {
                    FieldInfo field = typeof(T).GetField(pair.Key);
                    //Debug.Log("Field " + field.Name);

                    if (field != null)
                    {

                        if (pair.Value.GetType() == typeof(Dictionary<string, object>))
                        {
                            MethodInfo method = typeof(DataUtils).GetMethod("FromDictionary");
                            MethodInfo generic = method.MakeGenericMethod(field.FieldType);
                            field.SetValue(obj, generic.Invoke(null, new object[] { pair.Value }));
                        }
                        else if (pair.Value.GetType().IsArray)
                        {
                            field.SetValue(obj, pair.Value);
                        }
                        else if (field.FieldType.IsEnum)
                        {
                            field.SetValue(obj, Enum.Parse(field.FieldType, pair.Value.ToString()));
                        }
                        else if (field.FieldType.IsClass)
                        {
                            if (field.FieldType.ToString() == "System.String")
                            {
                                field.SetValue(obj, pair.Value.ToString());
                            }
                            else
                            {
                                MethodInfo method = typeof(DataUtils).GetMethod("CreateFromJSON", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                                MethodInfo generic = method.MakeGenericMethod(field.FieldType);
                                field.SetValue(obj, generic.Invoke(null, new object[] { pair.Value }));
                            }
                        }
                        else
                        {
                            switch (field.FieldType.ToString())
                            {
                                case "System.Int32":
                                    field.SetValue(obj, (int)Convert.ChangeType(pair.Value, typeof(int)));
                                    break;
                                case "System.Int64":
                                    field.SetValue(obj, (long)Convert.ChangeType(pair.Value, typeof(long)));
                                    break;
                                case "System.Single":
                                    field.SetValue(obj, (float)Convert.ChangeType(pair.Value, typeof(float)));
                                    break;
                                case "System.Double":
                                    double.TryParse(pair.Value.ToString(), out double x);
                                    field.SetValue(obj, (double)Convert.ChangeType(pair.Value, typeof(double)));
                                    break;
                                case "System.DateTime":
                                    field.SetValue(obj, (DateTime)Convert.ChangeType(pair.Value, typeof(System.DateTime)));
                                    break;
                                case "System.Boolean":
                                    field.SetValue(obj, (bool)Convert.ChangeType(pair.Value, typeof(System.Boolean)));
                                    break;
                                case "System.String":
                                    field.SetValue(obj, (string)Convert.ChangeType(pair.Value, typeof(System.String)));
                                    break;
                                default:
                                    field.SetValue(obj, pair.Value.ToString());
                                    break;
                            }
                            //Debug.Log("Get Value " + field.GetValue(obj));
                        }

                    }


                }
                catch (Exception e)
                {
                }
            }
        }


        public static bool IsGenericList(object o)
        {
            var oType = o.GetType();
            return (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)));
        }
        public static object GetMemberValueObject(object src, string memberName)
        {
            string[] words = memberName.Split(' ');
            return src.GetType().InvokeMember(words[0], BindingFlags.GetField, null, src, null);
        }

        public static T CreateFromJSON<T>(string jsonString) where T : BaseModel
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static string GetJsonString(dynamic dynamicObject)
        {
            return JsonConvert.SerializeObject(dynamicObject);
        }

        public static T CreatFromEncryptJson<T>(string encryptedString, string key) where T : BaseModel
        {
            try
            {
                string jsonString = DecryptStringWithKey(encryptedString, key);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static T Clone<T>(T obj) where T : BaseModel
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(obj.ToJson());
            }
            catch (Exception e)
            {
                return default(T);
            }

        }

        public static string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        public static bool CompareFirstCharacters(int n,string str1, string str2)
        {
            // Ensure both strings have at least n characters
            if (str1.Length >= n && str2.Length >= n)
            {
                // Extract the first 6 characters from both strings
                string first6Str1 = str1.Substring(0, n);
                string first6Str2 = str2.Substring(0, n);

                // Compare the first 6 characters
                return first6Str1 == first6Str2;
            }
            else
            {
                // If either string is shorter than 6 characters, return false
                return false;
            }
        }

        /*
		 * encrypt text based on secret key
		 */
        private static string encrypt(string plainText, string key)
        {
            byte[] plainTextbytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            for (int i = 0, j = 0; i < plainTextbytes.Length; i++, j = (j + 1) % keyBytes.Length)
            {
                plainTextbytes[i] = (byte)(plainTextbytes[i] ^ keyBytes[j]);
            }
            return System.Convert.ToBase64String(plainTextbytes);
        }
        

        /*
		 * decrypt text based on secret key
		 */
        private static string decrypt(string plainTextString, string secretKey)
        {
            byte[] cipheredBytes = System.Convert.FromBase64String(plainTextString);
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            for (int i = 0, j = 0; i < cipheredBytes.Length; i++, j = (j + 1) % keyBytes.Length)
            {
                cipheredBytes[i] = (byte)(cipheredBytes[i] ^ keyBytes[j]);
            }
            return System.Text.Encoding.UTF8.GetString(cipheredBytes);

        }

        public static string EncryptStringWithKey(string plainTextString, string secretKey)
        {
            return encrypt(plainTextString, secretKey);
        }

        public static string DecryptStringWithKey(string cipherText, string secretKey)
        {
            return decrypt(cipherText, secretKey);
        }

        public static float RoundUpToDecimalPlace(float number, int decimalPlaces =1)
        {
            float multiplier = MathF.Pow(10, decimalPlaces);
            return MathF.Ceiling(number * multiplier) / multiplier;
        }

        public static decimal StringToNum(string str)
        {
            decimal result = 0;
            if (decimal.TryParse(str.Replace(",", "").Replace(".", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out result)) return result;
            else return 0;
        }
        /*
		 * Util function to generate guid
		 */
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        #region Time
        static StringBuilder timeStringBuilder = new StringBuilder(200);
        public static DateTime StringToTime(string strDate)
        {
            //Log("Convert date: " + strDate);
            try
            {
                return System.DateTime.ParseExact(strDate, "yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception e1)
            {
                try
                {
                    return System.DateTime.ParseExact(strDate, "yyyy-MM-ddTHH.mm.ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception e2)
                {
                    try
                    {
                        return System.DateTime.Parse(strDate);
                    }
                    catch (Exception e3)
                    {
                        try
                        {
                            return DateTime.Parse(strDate + "+07:00");
                        }
                        catch (Exception e4)
                        {
                            return DateTime.Now;
                        }
                    }
                }
            }
        }

        public static string TimeToString(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static int TimeToRunningNumber(DateTime date)
        {
            //date.ToUniversalTime().ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            return Int32.Parse(date.ToUniversalTime().ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
        }

        public static DateTime RunningNumberToDate(int runningNumber)
        {
            return System.DateTime.ParseExact(runningNumber.ToString() + "000000", "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string GetTimeFormatFromTotalSeconds(double seconds, bool getDaysAndHours = false, bool getHours = false)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            timeStringBuilder.Length = 0;
            if (getDaysAndHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else if (getHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            return timeStringBuilder.ToString();

        }
        public static string GetMaxTimeFormatFromTotalSeconds(double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            timeStringBuilder.Length = 0;
            if (timeSpan.Days > 0)
                timeStringBuilder.AppendFormat("{0:D1} Day {1:D1}h", timeSpan.Days , timeSpan.Hours);
            else if (timeSpan.Hours > 0)
                timeStringBuilder.AppendFormat("{0:D1} Hour", timeSpan.Hours);
            else if (timeSpan.Minutes > 0)
                timeStringBuilder.AppendFormat("{0:D1} Min", timeSpan.Minutes);
            else if (timeSpan.Seconds > 0)
                timeStringBuilder.AppendFormat("{0:D1} s", timeSpan.Seconds);
            return timeStringBuilder.ToString();
        }
        public static string GetTimeFormatHasPassedTillNow(System.DateTime specificTime, bool getDaysAndHours = false, bool getHours = false)
        {
            System.TimeSpan timeSpan = System.DateTime.Now - specificTime;
            timeStringBuilder.Length = 0;
            if (getDaysAndHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else if (getHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            return timeStringBuilder.ToString();

        }
        public static string GetTimeFormatHasPassedTillNow(TimeSpan timeSpan, bool getDaysAndHours = false, bool getHours = false)
        {
            timeStringBuilder.Length = 0;
            if (getDaysAndHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else if (getHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            return timeStringBuilder.ToString();

        }
        public static TimeSpan GetTimespanHasPassedTillNow(System.DateTime specificTime)
        {
            System.TimeSpan timeSpan = System.DateTime.Now - specificTime;
            return timeSpan;
        }
        public static float GetTimeSecondsHasPassedTillNow(System.DateTime specificTime)
        {
            System.TimeSpan timeSpan = System.DateTime.Now - specificTime;
            return (float)timeSpan.TotalSeconds;
        }
        public static string GetTimeFormatHasPassedTillNow(string specificTime, bool getDaysAndHours = false, bool getHours = false)
        {
            System.TimeSpan timeSpan = System.DateTime.Now - StringToTime(specificTime);
            timeStringBuilder.Length = 0;
            if (getDaysAndHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else if (getHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            return timeStringBuilder.ToString();
        }
        public static float GetTimeSecondsHasPassedTillNow(string specificTime)
        {
            System.TimeSpan timeSpan = System.DateTime.Now - StringToTime(specificTime);
            return (float)timeSpan.TotalSeconds;
        }
        public static string GetTimeFormatRemainFromNow(System.DateTime timeCalculation, bool getDaysAndHours = false, bool getHours = false)
        {
            System.TimeSpan timeSpan = timeCalculation - System.DateTime.Now;
            if (timeSpan.TotalSeconds > 0)
            {
                if (getDaysAndHours)
                    timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                else if (getHours)
                    timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                else
                    timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            }
            else timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", 0, 0);
            return timeStringBuilder.ToString();
        }
        public static string GetStringTimespanFormatRemainFromNow(System.DateTime timeCalculation, string format, bool dayAndHours = false, bool hourAndMinute = false)
        {
            System.TimeSpan timeSpan = timeCalculation - System.DateTime.Now;
            timeStringBuilder.Length = 0;
            if (timeSpan.TotalSeconds > 0)
            {
                if (dayAndHours)
                    timeStringBuilder.AppendFormat(format, timeSpan.Days, timeSpan.Hours);
                else if (hourAndMinute)
                    timeStringBuilder.AppendFormat(format, timeSpan.Hours, timeSpan.Minutes);
                else
                    timeStringBuilder.AppendFormat(format, timeSpan.Minutes, timeSpan.Seconds);
            }
            else timeStringBuilder.AppendFormat(format, 0, 0);
            return timeStringBuilder.ToString();
        }

        public static TimeSpan GetTimespanFormatRemainFromNow(System.DateTime timeCalculation)
        {
            System.TimeSpan timeSpan = timeCalculation - System.DateTime.Now;
            return timeSpan;
        }
        public static float GetSecondsTimeRemainFromNow(System.DateTime timeCalculation)
        {
            System.TimeSpan timeSpan = timeCalculation - System.DateTime.Now;
            return (float)timeSpan.TotalSeconds;
        }

        public static string GetTimeFormatRemainFromNow(string timeCalculation, bool getDaysAndHours = false, bool getHours = false)
        {
            System.TimeSpan timeSpan = StringToTime(timeCalculation) - System.DateTime.Now;
            timeStringBuilder.Length = 0;
            if (getDaysAndHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            else if (getHours)
            {
                if (timeSpan.Hours > 0) timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                else timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            }

            else
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            return timeStringBuilder.ToString();
        }
        public static float GetSecondsTimeRemainFromNow(string timeCalculation)
        {
            System.TimeSpan timeSpan = StringToTime(timeCalculation) - System.DateTime.Now;
            return (float)timeSpan.TotalSeconds;
        }

        public static string GetTimeDuration(int time)
        {
            timeStringBuilder.Length = 0;
            if (time < 60)
            {
                return timeStringBuilder.AppendFormat("00:" + time.ToString("00")).ToString();
            }
            else if (time < 3600)
            {
                return timeStringBuilder.AppendFormat((time / 60).ToString("00") + ":" + (time % 60).ToString("00")).ToString();
            }
            else
            {
                return timeStringBuilder.AppendFormat((time / 3600).ToString("00") + ":" + ((time % 3600) / 60).ToString("00") + ":" + (time % 60).ToString("00")).ToString();
            }
        }

        public static DateTime GetSpecificDateTime(int dayOfWeek, int fireMinute = 0)
        {
            System.DayOfWeek day = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(DateTime.Now);
            int dayTogo = 0;
            if (day > System.DayOfWeek.Thursday)
                dayTogo = dayOfWeek + (6 - (int)day);
            else
                dayTogo = dayOfWeek - 1 - (int)day;
            System.DateTime date = System.DateTime.Now.Date.AddDays(dayTogo).AddMinutes(fireMinute);
            return date;
        }
        public static int CompareTime(DateTime time1, DateTime time2)
        {
            if (time1 > time2)
                return 1;
            if (time1 == time2)
                return 0;
            return -1;
        }

        public static int CompareTime(string timeString1, string timeString2)
        {
            DateTime time1 = StringToTime(timeString1);
            DateTime time2 = StringToTime(timeString2);
            if (time1 > time2)
                return 1;
            if (time1 < time2)
                return -1;
            if (time1 == time2)
                return 0;
            return 2;
        }
        public static TimeSpan GetTimespanBetweenTwoTimes(DateTime specificTime1, DateTime specificTime2)
        {
            return (specificTime1 - specificTime2).Duration();
        }

        public static TimeSpan GetTimespanBetweenTwoTimes(string specificTime1, string specificTime2)
        {
            return (StringToTime(specificTime1) - StringToTime(specificTime2)).Duration();
        }

        public static string GetTimespanFormatBetweenTwoTimes(string specificTime1, string specificTime2, bool getINTValue = false, bool getDaysAndHours = false, bool getHours = false)
        {
            TimeSpan timeSpan = (StringToTime(specificTime1) - StringToTime(specificTime2)).Duration();
            int timeSpanSeconds = 0;
            if (getINTValue)
            {
                timeSpanSeconds = timeSpan.Seconds;
            }
            timeStringBuilder.Length = 0;
            if (getDaysAndHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpanSeconds);
            else if (getHours)
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpanSeconds);
            else
                timeStringBuilder.AppendFormat("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpanSeconds);
            return timeStringBuilder.ToString();
        }
        #endregion

        public static bool IsArrayField(object obj, string field)
        {
            try
            {
                Type t = obj.GetType();
                MemberInfo[] myMembers = t.GetMember("data");
                if (myMembers.Length > 0)
                {
                    string[] words = myMembers[0].ToString().Split(' ');
                    if (words[0] != null)
                    {
#if UNITY_WEBGL
                        if (words[0].Contains("System.Collections.Generic.List"))
                            return true;

#endif
                        Type memberType = Type.GetType(words[0]);
                        if (memberType != null)
                        {
                            //UnityUtils.Log("Member info: " + words[0] + " is array: " + (memberType.GetInterface(nameof(IEnumerable)) != null));
                            return (memberType.GetInterface(nameof(IEnumerable)) != null);
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            //Debug.Log(n);
            while (n > 1)
            {
                n--;
                int k;
                Random RNG = new Random();
                k = RNG.Next(0, n);

                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(List<T> list, int seed)
        {
            int n = list.Count;
            //Debug.Log(n);
            while (n > 1)
            {
                n--;
                int k;
                Random RNG = new Random(seed);
                k = RNG.Next(0, n);

                T value = list[k];
                list[k] = list[n];
                list[n] = value;
                seed++;
            }
        }

        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k;
                Random RNG = new Random();
                k = RNG.Next(0, n);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }


        public static void Shuffle<T>(T[] array, int seed)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k;
                Random RNG = new Random(seed);
                k = RNG.Next(0, n);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
                seed++;
            }
        }

        public static T[] Add<T>(T n, T[] list)
        {
            ArrayList tmp = new ArrayList();

            if (list == null)
                list = new T[0];

            if (list.Length > 0)
            {
                foreach (T str in list) tmp.Add(str);
                tmp.Add(n);
            }
            else
                tmp.Add(n);

            return tmp.ToArray(typeof(T)) as T[];
        }

        public static T[] Remove<T>(int index, T[] list)
        {
            ArrayList tmp = new ArrayList();
            foreach (T str in list) tmp.Add(str);
            if (tmp.Count > index)
                tmp.RemoveAt(index);
            return tmp.ToArray(typeof(T)) as T[];
        }

        public static void CopyObject(this System.Object dst, object src)
        {
            var srcT = src.GetType();
            var dstT = dst.GetType();
            foreach (var f in srcT.GetFields())
            {
                var dstF = dstT.GetField(f.Name);
                if (dstF == null || dstF.IsLiteral)
                    continue;
                dstF.SetValue(dst, f.GetValue(src));
            }

            foreach (var f in srcT.GetProperties())
            {
                var dstF = dstT.GetProperty(f.Name);
                if (dstF == null)
                    continue;

                if (dstF.PropertyType.IsValueType)
                    dstF.SetValue(dst, f.GetValue(src, null), null);
                else
                {
                    System.Object subDestination = null;
                    CopyObject(subDestination, f.GetValue(src, null));
                    f.SetValue(f, subDestination);
                }
            }
        }

        public static System.Object ExtractObject(object src, List<string> fields)
        {
            var srcT = src.GetType();
            var dst = Activator.CreateInstance(srcT);
            foreach (var field in fields)
            {
                FieldInfo f = srcT.GetField(field);
                var dstF = srcT.GetField(f.Name);
                if (dstF == null || dstF.IsLiteral)
                    continue;
                dstF.SetValue(dst, f.GetValue(src));
            }

            return dst;
        }

        public static System.Object CreateFieldFromJson(Type type, string fieldName, string fieldValue)
        {
            try
            {
                FieldInfo field = type.GetField(fieldName);
                //Debug.Log("Field " + field.Name);

                if (field != null)
                {
                    if (field.FieldType.IsEnum)
                    {
                        return Enum.Parse(field.FieldType, fieldValue.ToString());
                    }
                    else if (field.FieldType.IsClass)
                    {
                        if (field.FieldType.ToString() == "System.String")
                        {
                            return fieldValue;
                        }
                        else
                        {
                            MethodInfo method = typeof(DataUtils).GetMethod("CreateFromJSON", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                            MethodInfo generic = method.MakeGenericMethod(field.FieldType);
                            return generic.Invoke(null, new object[] { fieldValue });
                        }
                    }
                    else
                    {
                        switch (field.FieldType.ToString())
                        {
                            case "System.Int32":
                                return (int)Convert.ChangeType(fieldValue, typeof(int));
                                break;
                            case "System.Int64":
                                return (long)Convert.ChangeType(fieldValue, typeof(long));
                                break;
                            case "System.Single":
                                return (float)Convert.ChangeType(fieldValue, typeof(float));
                                break;
                            case "System.Double":
                                double.TryParse(fieldValue, out double x);
                                return (double)Convert.ChangeType(fieldValue, typeof(double));
                                break;
                            case "System.DateTime":
                                return (DateTime)Convert.ChangeType(fieldValue, typeof(System.DateTime));
                                break;
                            case "System.Boolean":
                                return (bool)Convert.ChangeType(fieldValue, typeof(System.Boolean));
                                break;
                            case "System.String":
                                return (string)Convert.ChangeType(fieldValue, typeof(System.String));
                                break;
                            default:
                                return fieldValue.ToString();
                                break;
                        }
                        //Debug.Log("Get Value " + field.GetValue(obj));
                    }

                }

                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool IsValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        public static bool IsValidFacebookId(string inputId)
        {
            string strRegex = @"^([0-9]{12,})$";

            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputId))
                return (true);
            else
                return (false);
        }

        public static bool IsValidAppleToken(string inputToken)
        {
            string strRegex = @"^([0-9]{6}\.[0-9a-f]{32}\.[0-9]{4})$";

            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputToken))
                return (true);
            else
                return (false);
        }

        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonConvert.DeserializeObject<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }

        public static string[,] ReadFromCSVFile(string text)
        {
            string[] lines = text.Split("\n"[0]);
            int totalColumns = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCsvLine(lines[i]);
                totalColumns = System.Math.Max(totalColumns, row.Length);
            }

            string[,] outputGrid = new string[totalColumns, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCsvLine(lines[y]);
                for (int x = 0; x < row.Length; x++)
                {
                    outputGrid[x, y] = row[x];
                }
            }
            return outputGrid;
        }

        public static string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
                @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
                System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }
        public static bool IsRunningLocally()
        {
            return Environment.GetEnvironmentVariable("IS_RUNNING_LOCALLY") == "true";
        }

        public static bool IsThirdSaturdayOfMonth(DateTime date)
        {
            // Find the first Saturday of the month
            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            // Check how many days until the first Saturday
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
            DateTime firstSaturday = firstDayOfMonth.AddDays(daysUntilSaturday);

            // Find the third Saturday by adding 14 days (2 weeks) to the first Saturday
            DateTime thirdSaturday = firstSaturday.AddDays(14);

            // Check if the current date is the third Saturday
            return date.Date == thirdSaturday.Date;
        }

        public static bool IsLastSaturdayOfMonth(DateTime date)
        {
            // Check if today is a Saturday
            if (date.DayOfWeek != DayOfWeek.Saturday)
            {
                return false;
            }

            // Find the last day of the current month
            DateTime lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            // Calculate how many days back the last Saturday is from the last day of the month
            int daysUntilLastSaturday = ((int)lastDayOfMonth.DayOfWeek - (int)DayOfWeek.Saturday + 7) % 7;
            DateTime lastSaturday = lastDayOfMonth.AddDays(-daysUntilLastSaturday);

            // Check if the current date is the last Saturday
            return date.Date == lastSaturday.Date;
        }

        public static bool IsLastDayOfMonth(DateTime date)
        {
            // Find the last day of the current month
            DateTime lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            // Check if today's date is the same as the last day of the month
            return date.Date == lastDayOfMonth.Date;
        }

#if !UNITY_2017_1_OR_NEWER
        public static bool DoesPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }

        public static string GetLocalRootPath()
        {
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.GetFullPath(Path.Combine(binDirectory, ".."));
        }
#endif

        public static void Log(string s)
        {
#if ENABLE_MC_DEBUG
            UnityEngine.Debug.Log("[LMS Debug:]: " + s);
#endif
        }

        public static void LogError(string s)
        {
#if ENABLE_MC_DEBUG
            UnityEngine.Debug.LogError("[LMS Debug:]: " + s);
#endif
        }

        #region Bot Name
        public static string GetRandomBotName()
        {
            string botName = "";
            Random RNG = new Random();
            int i = RNG.Next(0, BotNames.Length);
            botName = BotNames[i];
            RNG = new Random();
            int n = RNG.Next(0, 100);
            /*
            if (n > 50)
            {
                int r = RNG.Next(0, randomSpecialChars.Length);
                botName += randomSpecialChars[r];
            }
            else
            {
                botName += RNG.Next(0, 10000).ToString();
            }*/
            botName += RNG.Next(0, 1000000000).ToString();
            return botName;
        }

        public static string[] BotNames = new string[] {
                                            "Marianna",
                                            "Steven",
                                            "Jacoby",
                                            "Mike",
                                            "Drew",
                                            "Savanah",
                                            "Sonny",
                                            "Dereon",
                                            "Haylee",
                                            "Leonel",
                                            "Ezra",
                                            "Khalil",
                                            "Cora",
                                            "Amina",
                                            "Denise",
                                            "Clarence",
                                            "Jovan",
                                            "Sydnee",
                                            "Marissa",
                                            "Zackary",
                                            "Leonidas",
                                            "Evelyn",
                                            "Nathanael",
                                            "Diana",
                                            "Kailey",
                                            "Kiana",
                                            "Eugene",
                                            "Giada",
                                            "Danielle",
                                            "Monique",
                                            "Carsen",
                                            "Deshawn",
                                            "Liberty",
                                            "Myah",
                                            "Karlie",
                                            "Cadence",
                                            "Case",
                                            "Dayton",
                                            "Abril",
                                            "Bailey",
                                            "Lana",
                                            "Ralph",
                                            "Angel",
                                            "Annabelle",
                                            "Dane",
                                            "Cameron",
                                            "Koen",
                                            "Daphne",
                                            "Landon",
                                            "Philip",
                                            "Xander",
                                            "Abel",
                                            "Maurice",
                                            "Azaria",
                                            "Karly",
                                            "Jayda",
                                            "Christopher",
                                            "Ciara",
                                            "Emerson",
                                            "Kaleigh",
                                            "Osvaldo",
                                            "Rylie",
                                            "Lillianna",
                                            "Camryn",
                                            "Audrina",
                                            "Phillip",
                                            "Lana",
                                            "Sawyer",
                                            "Aliza",
                                            "Jaelynn",
                                            "Felipe",
                                            "Daphne",
                                            "Barrett",
                                            "Justus",
                                            "Angelique",
                                            "Konner",
                                            "Alonso",
                                            "Haylee",
                                            "Gabriella",
                                            "Adriel",
                                            "Isaac",
                                            "Kristopher",
                                            "Zaria",
                                            "Karlee",
                                            "Thaddeus",
                                            "Adrienne",
                                            "Fernando",
                                            "Jillian",
                                            "Finn",
                                            "Tyshawn",
                                            "Royce",
                                            "Daisy",
                                            "Valentina",
                                            "Leland",
                                            "Isis",
                                            "Brock",
                                            "Jason",
                                            "Kaya",
                                            "Marley",
                                            "Gunnar","Louis",
                                            "Justin",
                                            "Elein",
                                            "Lee",
                                            "Tristan",
                                            "Grant",
                                            "Drew",
                                            "Finn",
                                            "Damien",
                                            "Chase",
                                            "Matilda",
                                            "Marcella",
                                            "Eli",
                                            "Tobias",
                                            "Justice",
                                            "Leo",
                                            "Benjamin",
                                            "Raine",
                                            "Annora",
                                            "Henry",
                                            "Everett",
                                            "Dezi",
                                            "Ricardo",
                                            "Lashon",
                                            "Monteen",
                                            "Syllable",
                                            "Robin",
                                            "Jackson",
                                            "Harriet",
                                            "Emerson",
                                            "Xavier",
                                            "Juan",
                                            "Gregory",
                                            "Beck",
                                            "Julina",
                                            "Javan",
                                            "Ryder",
                                            "Brighton",
                                            "Felix",
                                            "Aaron",
                                            "Riley",
                                            "Lawrence",
                                            "Jared",
                                            "Coralie",
                                            "Zane",
                                            "Caylen",
                                            "Will",
                                            "Amity",
                                            "Kylie",
                                            "Lane",
                                            "Porter",
                                            "Vanessa",
                                            "Naomi",
                                            "Denver",
                                            "Lillian",
                                            "Julian",
                                            "Ellice",
                                            "Apollo",
                                            "William",
                                            "Allison",
                                            "Ellison",
                                            "Oscar",
                                            "Randall",
                                            "Tavian",
                                            "Jolee",
                                            "Malachi",
                                            "Jude",
                                            "Ray",
                                            "Rayleen",
                                            "Jane",
                                            "Raven",
                                            "Blaise",
                                            "Hugh",
                                            "Coralie",
                                            "Brandt",
                                            "Cody",
                                            "Timothy",
                                            "Wade",
                                            "Heath",
                                            "Myron",
                                            "Carelyn",
                                            "Levi",
                                            "Caleb",
                                            "Cerise",
                                            "Jordon",
                                            "Aiden",
                                            "Dominick",
                                            "Rose",
                                            "Debree",
                                            "Carmden",
                                            "Julian",
                                            "Quintin",
                                            "Sutton",
                                            "Janetta",
                                            "Clark",
                                            "Korin",
                                            "Hope",
                                            "Adelaide",
                                            "Paul",
                                            "Marcellus","Moss",
                                             "Orozco",
                                             "Goodman",
                                             "Decker",
                                             "Mccoy",
                                             "Manning",
                                             "Watson",
                                             "Todd",
                                             "Peck",
                                             "Fowler",
                                             "Bryan",
                                             "Avila",
                                             "Archer",
                                             "Sanders",
                                             "Hines",
                                             "Frazier",
                                             "Cochran",
                                             "Berry",
                                             "Burton",
                                             "Horn",
                                             "Wallace",
                                             "Patterson",
                                             "Clayton",
                                             "Cline",
                                             "Buckley",
                                             "Blair",
                                             "Glenn",
                                             "Rodgers",
                                             "Beck",
                                             "Armstrong",
                                             "Cowan",
                                             "Sparks",
                                             "Bonilla",
                                             "Pratt",
                                             "Walton",
                                             "Haley",
                                             "Gordon",
                                             "Heath",
                                             "Goodwin",
                                             "Mayo",
                                             "Lara",
                                             "Moyer",
                                             "Odonnell",
                                             "Holmes",
                                             "Gray",
                                             "Garza",
                                             "Lowe",
                                             "Ochoa",
                                             "Franklin",
                                             "Reeves",
                                             "Ashley",
                                             "Ward",
                                             "Luna",
                                             "Arias",
                                             "Trujillo",
                                             "Bolton",
                                             "Harper",
                                             "Foley",
                                             "Burch",
                                             "Cochran",
                                             "Roach",
                                             "Wheeler",
                                             "Bishop",
                                             "Sandoval",
                                             "Brennan",
                                             "Rich",
                                             "Hall",
                                             "Mason",
                                             "Jennings",
                                             "Lucero",
                                             "Rose",
                                             "Lucas",
                                             "Garcia",
                                             "Weaver",
                                             "Weber",
                                             "Chung",
                                             "Kramer",
                                             "Heath",
                                             "Barry",
                                             "Krause",
                                             "Montoya",
                                             "Nielsen",
                                             "Kirby",
                                             "Johns",
                                             "Castillo",
                                             "Pearson",
                                             "Bentley",
                                             "Russell",
                                             "Hurst",
                                             "Floyd",
                                             "Gross",
                                             "King",
                                             "Gray",
                                             "Camacho",
                                             "Thomas",
                                             "Beck",
                                             "Stout",
                                             "Johns",
                                             "Christian",
                                             "Melton"
        };

        public static string[] randomSpecialChars = new string[]
        {
                "~",
                "!",
                "@",
                "#",
                "$",
                "%",
                "^",
                "&",
                "*",
                "'",
                "|",
                "`",
                "_",
                "-",
        };
        #endregion
    }


    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}

