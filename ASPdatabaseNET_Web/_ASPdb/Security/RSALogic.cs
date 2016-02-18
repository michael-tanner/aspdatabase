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
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using ASPdatabaseNET.Security.RsaKeyConverter;

namespace ASPdb.Security
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class RSALogic : jQueryContext
    {
        //----------------------------------------------------------------------------------------------------
        public static RSACryptoServiceProvider GetNew_RSAProvider()
        {
            return new RSACryptoServiceProvider(3048);
        }
        //----------------------------------------------------------------------------------------------------
        public static string Get_PublicPEM(RSACryptoServiceProvider rsa)
        {
            return ASPdb.Security.RSAHelpers.RsaKeyConverter.XmlToPem(rsa.ToXmlString(false));
        }
        //----------------------------------------------------------------------------------------------------
        public static string Get_PrivateXML(RSACryptoServiceProvider rsa)
        {
            return rsa.ToXmlString(true);
        }
        //----------------------------------------------------------------------------------------------------
        public static RSACryptoServiceProvider Get_RSA(string privateXML)
        {
            var rtn = new RSACryptoServiceProvider();
            rtn.FromXmlString(privateXML);
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public static string Encrypt(string public_PEM, string plainText)
        {
            string xml = RsaKeyConverter.PemToXml(public_PEM);
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xml);
                byte[] input = System.Text.Encoding.UTF8.GetBytes(plainText);
                byte[] output = rsa.Encrypt(input, false);
                return Convert.ToBase64String(output);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static string Decrypt(string private_XML, string cipherText)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(private_XML);
                byte[] input = Convert.FromBase64String(cipherText);
                byte[] output = rsa.Decrypt(input, false);
                return System.Text.Encoding.Default.GetString(output);
            }
        }
    }
}

namespace ASPdb.Security.RSAHelpers
{
    //----------------------------------------------------------------------------------------------------////
    public static class BytesExtensions
    {
        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }

    //----------------------------------------------------------------------------------------------------////
    public static class RsaExtensions
    {
        public static AsymmetricCipherKeyPair GetKeyPair(this RSA rsa)
        {
            try
            {
                return DotNetUtilities.GetRsaKeyPair(rsa);
            }
            catch
            {
                return null;
            }
        }

        public static RsaKeyParameters GetPublicKey(this RSA rsa)
        {
            try
            {
                return DotNetUtilities.GetRsaPublicKey(rsa);
            }
            catch
            {
                return null;
            }
        }
    }

    //----------------------------------------------------------------------------------------------------////
    public static class RsaKeyConverter
    {
        //----------------------------------------------------------------------------------------------------
        public static string XmlToPem(string xml)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(xml);

                AsymmetricCipherKeyPair keyPair = rsa.GetKeyPair(); // try get private and public key pair
                if (keyPair != null) // if XML RSA key contains private key
                {
                    PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
                    return FormatPem(privateKeyInfo.GetEncoded().ToBase64(), "RSA PRIVATE KEY");
                }

                RsaKeyParameters publicKey = rsa.GetPublicKey(); // try get public key
                if (publicKey != null) // if XML RSA key contains public key
                {
                    SubjectPublicKeyInfo publicKeyInfo =
                        SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
                    return FormatPem(publicKeyInfo.GetEncoded().ToBase64(), "PUBLIC KEY");
                }
            }

            throw new InvalidKeyException("Invalid RSA Xml Key");
        }

        //----------------------------------------------------------------------------------------------------
        private static string FormatPem(string pem, string keyType)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("-----BEGIN {0}-----\n", keyType);

            int line = 1, width = 64;

            while ((line - 1) * width < pem.Length)
            {
                int startIndex = (line - 1) * width;
                int len = line * width > pem.Length
                              ? pem.Length - startIndex
                              : width;
                sb.AppendFormat("{0}\n", pem.Substring(startIndex, len));
                line++;
            }

            sb.AppendFormat("-----END {0}-----\n", keyType);
            return sb.ToString();
        }

        //----------------------------------------------------------------------------------------------------
        public static string PemToXml(string pem)
        {
            if (pem.StartsWith("-----BEGIN RSA PRIVATE KEY-----")
                || pem.StartsWith("-----BEGIN PRIVATE KEY-----"))
            {
                return GetXmlRsaKey(pem, obj =>
                {
                    if ((obj as RsaPrivateCrtKeyParameters) != null)
                        return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)obj);
                    var keyPair = (AsymmetricCipherKeyPair)obj;
                    return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)keyPair.Private);
                }, rsa => rsa.ToXmlString(true));
            }

            if (pem.StartsWith("-----BEGIN PUBLIC KEY-----"))
            {
                return GetXmlRsaKey(pem, obj =>
                {
                    var publicKey = (RsaKeyParameters)obj;
                    return DotNetUtilities.ToRSA(publicKey);
                }, rsa => rsa.ToXmlString(false));
            }

            throw new InvalidKeyException("Unsupported PEM format...");
        }

        //----------------------------------------------------------------------------------------------------
        private static string GetXmlRsaKey(string pem, Func<object, RSA> getRsa, Func<RSA, string> getKey)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var sr = new StreamReader(ms))
            {
                sw.Write(pem);
                sw.Flush();
                ms.Position = 0;
                var pr = new PemReader(sr);
                object keyPair = pr.ReadObject();
                using (RSA rsa = getRsa(keyPair))
                {
                    var xml = getKey(rsa);
                    return xml;
                }
            }
        }


    }
}