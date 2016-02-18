using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.Users
{
    //----------------------------------------------------------------------------------------------------////
    [Serializable()]
    public class UserSessionServer
    {
        public string RSA_PublicKey_PEM;
        private string RSA_PrivateKey_XML;
        public Dictionary<string, ASPdb.Security.AESKeyInfo> AESKeysDict;

        public UserInfo UserInfo = null;
        public bool IsLoggedIn { get { return (this.UserInfo != null); } }

        public bool Impersonation_IsAllowed = false;
        public bool Impersonation_IsOn = false;
        public UserInfo Impersonation_ActualUser = null;

        //------------------------------------------------------------------------------------- Constructor --
        public UserSessionServer()
        {
            this.AESKeysDict = new Dictionary<string, ASPdb.Security.AESKeyInfo>();
        }

        //----------------------------------------------------------------------------------------------------
        public string Get__RSA_PrivateKey_XML()
        {
            return this.RSA_PrivateKey_XML;
        }
        //----------------------------------------------------------------------------------------------------
        public void Set__RSA_PrivateKey_XML(string xml)
        {
            this.RSA_PrivateKey_XML = xml;
        }

        //----------------------------------------------------------------------------------------------------
        public void CheckForDemoMode()
        {
            if(this.UserInfo == null)
                throw new Exception("UserInfo is null");

            var userInfo = this.UserInfo;
            if (this.Impersonation_ActualUser != null)
                userInfo = this.Impersonation_ActualUser;

            if (UserSessionLogic.IsInDemoMode)
                if (userInfo.Username.ToLower() == "demo")
                    throw new Exception("This demo user session is Read-Only.");

            
        }


        //----------------------------------------------------------------------------------------------------
        public void CloseSession()
        {

        }
    }
}