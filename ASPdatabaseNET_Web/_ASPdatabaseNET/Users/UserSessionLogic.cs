using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using ASPdb.Security;
using ASPdb.Security.RSAHelpers;
using ASPdb.UniversalADO;

namespace ASPdatabaseNET.Users
{
    //----------------------------------------------------------------------------------------------------////
    public class UserSessionLogic
    {
        public static string UserSessionKey = "ASPdatabaseNET.Users.UserSessionInfo";
        //private static int BCrypt_Workfactor = 12;

        //----------------------------------------------------------------------------------------------------
        public static UserSessionServer GetUser()
        {
            UserSessionServer rtn = null;
            try
            {
                rtn = (UserSessionServer)HttpContext.Current.Session[UserSessionLogic.UserSessionKey];
            }
            catch { }
            if(rtn == null)
            {
                rtn = new UserSessionServer();
                HttpContext.Current.Session[UserSessionLogic.UserSessionKey] = rtn;
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static UserInfo Login(string username, string password)
        {
            if (!Subscription.SubscriptionAppState.ValidateActiveSubscribers())
                throw new Exception("Validation Error");

            string errorMsg = "Invalid Login Credentials";
            try
            {
                string sql = "select * from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users] where [Username] = @Username and [Active] = 1";
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@Username", username);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        if(reader.Read())
                        {
                            if (BCrypt.Net.BCrypt.Verify(password, reader.Get("Password", "")))
                            {
                                var userSession = GetUser();
                                userSession.UserInfo = new UserInfo()
                                {
                                    UserId = reader.Get("UserId", -1),
                                    Username = reader.Get("Username", ""),
                                    Email = reader.Get("Email", ""),
                                    FirstName = reader.Get("FirstName", ""),
                                    LastName = reader.Get("LastName", ""),
                                    Active = reader.Get("Active", false),
                                    IsAdmin = reader.Get("IsAdmin", false)
                                };
                                userSession.Impersonation_IsAllowed = userSession.UserInfo.IsAdmin;
                                SaveLastLoginTime(userSession.UserInfo.UserId);

                                userSession.UserInfo.AllPermissions = LoadAllPermissions(userSession.UserInfo.UserId, userSession.UserInfo.IsAdmin);


                                if (userSession.Impersonation_IsAllowed)
                                    userSession.Impersonation_ActualUser = userSession.UserInfo;
                                return userSession.UserInfo;
                            }
                        }
                    }
                }
            }
            catch (Exception exc) { errorMsg = "Error in Login"; ASPdb.Framework.Debug.RecordException(exc); }

            throw new Exception(errorMsg);
        }
        //----------------------------------------------------------------------------------------------------
        private static void SaveLastLoginTime(int userId)
        {
            string sql = String.Format("update [{0}].[ASPdb_Users] set [LastLoginTime] = @LastLoginTime where [UserId] = @UserId", Config.SystemProperties.AppSchema);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@LastLoginTime", DateTime.Now);
                command.AddParameter("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }


        //----------------------------------------------------------------------------------------------------
        public static void DoLogout()
        {
            UserSessionLogic.ClearSessionUser();
        }
        //----------------------------------------------------------------------------------------------------
        private static void ClearSessionUser()
        {
            try
            {
                ((UserSessionServer)HttpContext.Current.Session[UserSessionLogic.UserSessionKey]).CloseSession();
            }
            catch { }
            try
            {
                HttpContext.Current.Session[UserSessionLogic.UserSessionKey] = null;
            }
            catch { }
        }

        //----------------------------------------------------------------------------------------------------
        public static UserSessionClient GetUserSession_ForClient()
        {
            var userSessionInfo = GetUser();
            if (userSessionInfo == null)
                return null;
            var rtn = new UserSessionClient()
            {
                UserInfo = userSessionInfo.UserInfo,
                IsLoggedIn = userSessionInfo.IsLoggedIn,
                Impersonation_IsAllowed = userSessionInfo.Impersonation_IsAllowed,
                Impersonation_IsOn = userSessionInfo.Impersonation_IsOn,
                Impersonation_ActualUser = userSessionInfo.Impersonation_ActualUser,
                IsAdmin = userSessionInfo.UserInfo.IsAdmin
            };
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static bool ToggleImpersonation()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var userSession = GetUser();
            if(userSession != null && userSession.IsLoggedIn && userSession.Impersonation_IsAllowed)
            {
                userSession.Impersonation_IsOn = !userSession.Impersonation_IsOn;
                if (!userSession.Impersonation_IsOn)
                    userSession.UserInfo = userSession.Impersonation_ActualUser;

                return userSession.Impersonation_IsOn;
            }
            return false;
        }
        //----------------------------------------------------------------------------------------------------
        public static UserInfo[] GetImpersonationList()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new List<UserInfo>();
            var userSession = GetUser();
            if (userSession.Impersonation_ActualUser == null)
                return null;

            var actualUser = userSession.Impersonation_ActualUser;
            rtn.Add(new UserInfo()
            {
                UserId = actualUser.UserId,
                LastName = "You",
                FirstName = "",
                Username = actualUser.Username,
                Active = actualUser.Active,
                Email = actualUser.Email
            });

            
            string sql = "select * from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users] where [UserId] > -1 order by [Active] desc, [LastName], [FirstName], [Username]";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        var userInfo = new UserInfo()
                        {
                            UserId = reader.Get("UserId", -1),
                            Username = reader.Get("Username", ""),
                            FirstName = reader.Get("FirstName", ""),
                            LastName = reader.Get("LastName", ""),
                            Email = reader.Get("Email", ""),
                            Active = reader.Get("Active", false)
                        };
                        if (userInfo.UserId != actualUser.UserId)
                            rtn.Add(userInfo);
                    }
                }
            }

            return rtn.ToArray();
        }
        //----------------------------------------------------------------------------------------------------
        public static bool SetImpersonationUser(int userId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var userSession = GetUser();
            if (userSession != null && userSession.IsLoggedIn && userSession.Impersonation_IsAllowed && userSession.Impersonation_IsOn)
            {
                string sql = "select * from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users] where [UserId] = @UserId";
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@UserId", userId);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        if (reader.Read())
                        {
                            userSession.UserInfo = new UserInfo()
                            {
                                UserId = reader.Get("UserId", -1),
                                Username = reader.Get("Username", ""),
                                Email = reader.Get("Email", ""),
                                FirstName = reader.Get("FirstName", ""),
                                LastName = reader.Get("LastName", ""),
                                IsAdmin = reader.Get("IsAdmin", false)
                            };
                            userSession.UserInfo.AllPermissions = LoadAllPermissions(userSession.UserInfo.UserId, userSession.UserInfo.IsAdmin);
                        }
                    }
                }
                return true;
            }
            return false;
        }
        //----------------------------------------------------------------------------------------------------
        public static AllPermissionsInfo LoadAllPermissions(int userId, bool isAdmin)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new AllPermissionsInfo() { UserId = userId, IsAdmin = isAdmin };
            if (isAdmin)
                return rtn;
            rtn.PermissionValuesDict = new Dictionary<string, PermissionValues>();

            string sql = String.Format(@"
                select 
                    T2.[GroupId], T2.[GroupName],
                    T3.[PermissionType], T3.[ConnectionId], T3.[Schema], T3.[TableId], T3.[View], T3.[Edit], T3.[Insert], T3.[Delete]
                from 
                    [{0}].[ASPdb_UsersToGroups] as T1
                inner join 
                    [{0}].[ASPdb_UserGroups] as T2 on T1.[GroupId] = T2.[GroupId]
                inner join
                    [{0}].[ASPdb_Permissions] as T3 on T1.[GroupId] = T3.[GroupId]
                where 
                    T1.[UserId] = @UserId and T2.[Active] = 1
                order by 
                    T3.[ConnectionId], T3.[Schema], T3.[TableId]
                ", Config.SystemProperties.AppSchema);
            using(DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@UserId", userId);
                using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while(reader.Read())
                    {
                        string permissionType = reader.Get("PermissionType", "");
                        int connectionId = reader.Get("ConnectionId", -1);
                        string schema = reader.Get("Schema", "");
                        int tableId = reader.Get("TableId", -1);
                        var permissionValues = new PermissionValues()
                        {
                            View = reader.Get("View", false),
                            Edit = reader.Get("Edit", false),
                            Insert = reader.Get("Insert", false),
                            Delete = reader.Get("Delete", false)
                        };
                        string key = "";
                        switch(permissionType)
                        {
                            case "C": key = "C_" + connectionId; break;
                            case "S": key = "S_" + connectionId + "_" + schema.ToLower(); break;
                            case "T": key = "T_" + connectionId + "_" + schema.ToLower() + "_" + tableId; break;
                        }
                        if (!rtn.PermissionValuesDict.ContainsKey(key))
                            rtn.PermissionValuesDict.Add(key, permissionValues);
                    }
                }
            }
            return rtn;
        }



        //----------------------------------------------------------------------------------------------------
        public static string[] GetSessionPublicKey()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new string[3];

            var user = UserSessionLogic.GetUser();
            if (String.IsNullOrEmpty(user.RSA_PublicKey_PEM))
                GenerateRSAKeys(user);

            rtn[0] = Guid.NewGuid().ToString();
            rtn[1] = user.RSA_PublicKey_PEM;
            rtn[2] = Guid.NewGuid().ToString();

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private static void GenerateRSAKeys(UserSessionServer userInfo)
        {
            var rsa = RSALogic.GetNew_RSAProvider();
            userInfo.RSA_PublicKey_PEM = RSALogic.Get_PublicPEM(rsa);
            userInfo.Set__RSA_PrivateKey_XML(RSALogic.Get_PrivateXML(rsa));
        }


        //----------------------------------------------------------------------------------------------------
        public static bool CaptureAESKey(string aesIndex, string base64)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var user = UserSessionLogic.GetUser();

            string aes_JSON = RSALogic.Decrypt(user.Get__RSA_PrivateKey_XML(), base64);
            var aesKeyInfo = (new JavaScriptSerializer()).Deserialize<AESKeyInfo>(aes_JSON);

            if (!user.AESKeysDict.ContainsKey(aesIndex))
                user.AESKeysDict.Add(aesIndex, aesKeyInfo);

            return true;
        }

        //----------------------------------------------------------------------------------------------------
        public static string EncryptAES(string aesIndex, string message)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var user = UserSessionLogic.GetUser();
            string rtn = "!";
            if (user.AESKeysDict.ContainsKey(aesIndex))
            {
                var aesKeyInfo = user.AESKeysDict[aesIndex];
                rtn = AESLogic.EncryptServer(aesKeyInfo, message);
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static string DecryptAES(string aesIndex, string base64)
        {
            var user = UserSessionLogic.GetUser();
            string rtn = "!";
            if (user.AESKeysDict.ContainsKey(aesIndex))
            {
                var aesKeyInfo = user.AESKeysDict[aesIndex];
                rtn = AESLogic.DecryptServer(aesKeyInfo, base64);
            }
            return rtn;
        }




        ////----------------------------------------------------------------------------------------------------
        //public static string ReceiveAESTest(string aesIndex, string base64)
        //{
        //    var user = UserSessionLogic.GetUser();
        //    string rtn = "!";
        //    if (user.AESKeysDict.ContainsKey(aesIndex))
        //    {
        //        var aesKeyInfo = user.AESKeysDict[aesIndex];
        //        string message = AESLogic.DecryptServer(aesKeyInfo, base64);
        //        if (message == "What color is the sky?")
        //            rtn = AESLogic.EncryptServer(aesKeyInfo, "Blue");
        //    }
        //    return rtn;
        //}
        //----------------------------------------------------------------------------------------------------
        public static string ReceiveAESTest(string message)
        {
            return "Your message: " + message + " ... The time is " + DateTime.Now.ToString();
        }







        //----------------------------------------------------------------------------------------------------
        private static bool? _isInDemoMode;
        public static bool IsInDemoMode
        {
            get
            {
                if (!_isInDemoMode.HasValue)
                {
                    _isInDemoMode = false;
                    try
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings["DemoMode"].ToLower().Trim() == "true")
                            _isInDemoMode = true;
                    }
                    catch { }
                }
                return _isInDemoMode.GetValueOrDefault(false);
            }
        }
    }
}