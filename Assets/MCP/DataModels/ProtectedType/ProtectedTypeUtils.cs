using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP.DataModels.ProtectedType
{
    internal class ProtectedTypeUtils
    {
        private static ProtectedTypeUtils _instance;

        private const string _randomKey = "wgIl3ZjLdwYviMeTPo90QhVk1BHLA4YgWt5ES1avh8Lace8wqp5SfetYqRFJvg2xh6Kn1pIrHRyVBGCtgCSn3V8FbsVROZSosJEN5CcHQpX6Roj89TDk0sRYzxPKzbqjzPbjk7PxZhVYAO8vg6kFPF7px8Hwl5yDxElhwxRdlpvmU9La96qelkXyAuTK55JuYOvohN0zdEJp5MSlkfpYxAMcudeB7dxL973Y6B835RIKB8Yq7Usr0IaxvF8QostF";

        private int _globalId = 0;
        public ProtectedTypeUtils()
        {
            System.Random rand = new System.Random((int)DateTime.Now.Ticks);
            this._globalId = rand.Next(10000);
        }

        public int GetGlobalId()
        {
            Random rand = new System.Random();
            this._globalId = rand.Next(10000);
            /*
            this._globalId += 9887;

            this._globalId = (this._globalId > 10000 ? this._globalId - 10000 : this._globalId);*/
            return this._globalId;
        }

        public static ProtectedTypeUtils GetInstance()
        {
            if (null == _instance)
            {
                _instance = new ProtectedTypeUtils();
            }
            return _instance;
        }

        public string EncodeType(string text)
        {
            return encrypt(text, _randomKey);
        }

        public string EncodeType(string text, string customKey)
        {
            return encrypt(text, customKey);
        }

        public string EncodeType(string text, DateTime customKey)
        {
            return encrypt(text, ((int)customKey.Ticks % 100).ToString());
        }

        public string EncodeType(string text, int customKey)
        {
            return encrypt(text, ((int)customKey % 100).ToString());
        }

        public string DecodeType(string cipherText)
        {
            return decrypt(cipherText, _randomKey);
        }
        public string DecodeType(string cipherText, string customKey)
        {
            return decrypt(cipherText, customKey);
        }

        public string DecodeType(string cipherText, DateTime customKey)
        {
            return decrypt(cipherText, ((int)customKey.Ticks % 100).ToString());
        }

        public string DecodeType(string cipherText, int customKey)
        {
            return decrypt(cipherText, ((int)customKey % 100).ToString());
        }

        /*
		 * encrypt text based on secret key
		 */
        private string encrypt(string plainText, string key)
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
        private string decrypt(string plainTextString, string secretKey)
        {
            byte[] cipheredBytes = System.Convert.FromBase64String(plainTextString);
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            for (int i = 0, j = 0; i < cipheredBytes.Length; i++, j = (j + 1) % keyBytes.Length)
            {
                cipheredBytes[i] = (byte)(cipheredBytes[i] ^ keyBytes[j]);
            }
            return System.Text.Encoding.UTF8.GetString(cipheredBytes);

        }

        public string EncryptStringWithKey(string plainTextString, string secretKey)
        {
            return encrypt(plainTextString, secretKey);
        }

        public string DecryptStringWithKey(string cipherText, string secretKey)
        {
            return decrypt(cipherText, secretKey);
        }

    }
}
