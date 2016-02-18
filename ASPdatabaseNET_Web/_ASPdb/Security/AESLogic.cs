using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ASPdb.Security
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AESLogic : jQueryContext
    {
        //----------------------------------------------------------------------------------------------------
        public static AESKeyInfo CreateNewAES()
        {
            var rtn = new AESKeyInfo();
            rtn.A = RandomBase64(1, 23);
            rtn.B = RandomBase64(0, 11);
            rtn.C = RandomBase64(0, 11);
            rtn.D = RandomBase64(1, 23);

            rtn.Pass = RandomBase64(25, 30);

            var salt1 = AESLogic.CryptoJS_lib_WordArray_random(128 / 8);
            var key = AESLogic.CryptoJS_PBKDF2(rtn.Pass, salt1, 128 / 32, 200);
            rtn.Key = "" + AESLogic.CryptoJS_enc_Base64_stringify(key);

            var salt2 = AESLogic.CryptoJS_lib_WordArray_random(128 / 8);
            var iv = AESLogic.CryptoJS_PBKDF2(rtn.Pass, salt2, 128 / 32, 200);
            rtn.IV = "" + AESLogic.CryptoJS_enc_Base64_stringify(iv);
            
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public static string EncryptClient(AESKeyInfo aesKeyInfo, string message)
        {
            var key = ASPdb.Security.AESLogic.CryptoJS_enc_Base64_parse(aesKeyInfo.Key);
            var iv = ASPdb.Security.AESLogic.CryptoJS_enc_Base64_parse(aesKeyInfo.IV);
            var parsedText = AESLogic.CryptoJS_enc_Utf8_parse(message);
            var cipherText = AESLogic.CryptoJS_AES_encrypt(parsedText, key, 128 / 8, iv);
            return cipherText;
        }
        //----------------------------------------------------------------------------------------------------
        public static string DecryptClient(AESKeyInfo aesKeyInfo, string cipherText)
        {
            var key = ASPdb.Security.AESLogic.CryptoJS_enc_Base64_parse(aesKeyInfo.Key);
            var iv = ASPdb.Security.AESLogic.CryptoJS_enc_Base64_parse(aesKeyInfo.IV);
            string decrypted = AESLogic.CryptoJS_AES_decrypt(cipherText, key, 128 / 8, iv);
            string plainText = ASPdb.Security.AESLogic.ToString_UTF8(decrypted);
            return plainText;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export=false)]
        public static string EncryptServer(AESKeyInfo aesKeyInfo, string message)
        {
            var key_Bytes = Convert.FromBase64String(aesKeyInfo.Key);
            var iv_Bytes = Convert.FromBase64String(aesKeyInfo.IV);
            byte[] cipherBytes = EncryptStringToBytes(message, key_Bytes, iv_Bytes);
            var encrypted = Convert.ToBase64String(cipherBytes);
            return encrypted;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public static string DecryptServer(AESKeyInfo aesKeyInfo, string cipherText)
        {
            var key_Bytes = Convert.FromBase64String(aesKeyInfo.Key);
            var iv_Bytes = Convert.FromBase64String(aesKeyInfo.IV);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            var plainText = DecryptStringFromBytes(cipherBytes, key_Bytes, iv_Bytes);
            return plainText;
        }


        //----------------------------------------------------------------------------------------------------
        public static string RandomBase64(int min, int max)
        {
            int i = 32;
            eval("i = Math.floor(Math.random()*(max-min+1)+min);");
            var tmp = AESLogic.CryptoJS_lib_WordArray_random(i);
            return AESLogic.CryptoJS_enc_Base64_stringify(tmp);
        }

        //----------------------------------------------------------------------------------------------------
        //CryptoJS.lib.WordArray.random( )
        public static object CryptoJS_lib_WordArray_random(int i)
        {
            object rtn = null;
            var tmp = i;
            eval("rtn = CryptoJS.lib.WordArray.random(tmp);");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        // CryptoJS.PBKDF2(secretPassphrase, salt, { keySize: 128 / 32, iterations: 200 });
        public static string CryptoJS_PBKDF2(string secretPassphrase, object salt, int keySize, int iterations)
        {
            string rtn = null;
            keySize = 1 * keySize;
            iterations = 1 * iterations;

            // rtn = CryptoJS.PBKDF2(secretPassphrase, salt, { keySize: 128 / 32, iterations: 200 });
            eval("rtn = CryptoJS.PBKDF2(secretPassphrase, salt, { keySize: " + keySize + ", iterations: " + iterations + " });");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        // CryptoJS.enc.Base64.stringify( )
        public static string CryptoJS_enc_Base64_stringify(object input)
        {
            string rtn = null;
            eval("rtn = CryptoJS.enc.Base64.stringify(input);");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        // CryptoJS.enc.Base64.parse( )
        public static string CryptoJS_enc_Base64_parse(object input)
        {
            string rtn = null;
            eval("rtn = CryptoJS.enc.Base64.parse(input);");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        // CryptoJS.enc.Utf8.parse( )
        public static object CryptoJS_enc_Utf8_parse(object input)
        {
            object rtn = null;
            eval("rtn = CryptoJS.enc.Utf8.parse(input);");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        // CryptoJS.AES.encrypt ( )
        public static string CryptoJS_AES_encrypt(object inputText, string key, int keySize, string iv)
        {
            keySize = 1 * keySize;
            string encryptedCiphertext = "";
            eval(@"
                var encrypted = CryptoJS.AES.encrypt(inputText, key,
                {
                    keySize: " + keySize + @",
                    iv: iv,
                    mode: CryptoJS.mode.CBC,
                    padding: CryptoJS.pad.Pkcs7
                });
                encryptedCiphertext = encrypted.ciphertext;
            ");

            return ASPdb.Security.AESLogic.CryptoJS_enc_Base64_stringify(encryptedCiphertext);
        }
        //----------------------------------------------------------------------------------------------------
        // CryptoJS.AES.decrypt ( )
        public static string CryptoJS_AES_decrypt(object cipherText, string key, int keySize, string iv)
        {
            string rtn = null;
            keySize = 1 * keySize;
            eval(@"
                rtn = CryptoJS.AES.decrypt(cipherText, key,
                {
                    keySize: " + keySize + @",
                    iv: iv,
                    mode: CryptoJS.mode.CBC,
                    padding: CryptoJS.pad.Pkcs7
                });
            ");

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static string ToString_UTF8(object input)
        {
            string rtn = null;
            eval("rtn = input.toString(CryptoJS.enc.Utf8);");
            return rtn;
        }





        [JsMethod(Export = false)]
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        [JsMethod(Export = false)]
        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}