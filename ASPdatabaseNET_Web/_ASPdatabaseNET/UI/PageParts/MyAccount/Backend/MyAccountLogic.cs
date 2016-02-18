using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.MyAccount.Objs;
using ASPdb.UniversalADO;

namespace ASPdatabaseNET.UI.PageParts.MyAccount.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class MyAccountLogic
    {
        //----------------------------------------------------------------------------------------------------
        public static MyAccountInfo GetInfo()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var user = ASPdatabaseNET.Users.UserSessionLogic.GetUserSession_ForClient().UserInfo;

            var rtn = new MyAccountInfo()
            {
                UserId = user.UserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return rtn;
        }
        
        //----------------------------------------------------------------------------------------------------
        public static void Save(MyAccountInfo myAccountInfo)
        {
            var user = ASPdatabaseNET.Users.UserSessionLogic.GetUserSession_ForClient().UserInfo;
            if (user.UserId != myAccountInfo.UserId)
                throw new Exception("Invalid user info.");

            if (String.IsNullOrEmpty(myAccountInfo.FirstName))
                throw new Exception("Please enter your first name.");
            if (String.IsNullOrEmpty(myAccountInfo.LastName))
                throw new Exception("Please enter your last name.");
            if (String.IsNullOrEmpty(myAccountInfo.Email) && myAccountInfo.Email.Length < 5 && !myAccountInfo.Email.Contains("@"))
                throw new Exception("Please enter your email address.");

            try
            {
                string sql = String.Format(@"
                    update [{0}].[ASPdb_Users]
                    set  [FirstName] = @FirstName
                        ,[LastName] = @LastName
                        ,[Email] = @Email
                    where [UserId] = @UserId
                ", Config.SystemProperties.AppSchema);

                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@FirstName", myAccountInfo.FirstName);
                    command.AddParameter("@LastName", myAccountInfo.LastName);
                    command.AddParameter("@Email", myAccountInfo.Email);
                    command.AddParameter("@UserId", myAccountInfo.UserId);

                    command.ExecuteNonQuery();
                }

                sql = String.Format("select * from [{0}].[ASPdb_Users] where [UserId] = @UserId", Config.SystemProperties.AppSchema);
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@UserId", myAccountInfo.UserId);
                    using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        if(reader.Read())
                        {
                            user.FirstName = reader.Get("FirstName", "");
                            user.LastName = reader.Get("LastName", "");
                            user.Email = reader.Get("Email", "");
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                ASPdb.Framework.Debug.RecordException(exc);
                throw new Exception("Error while trying to save account info.");
            }
        }

        //----------------------------------------------------------------------------------------------------
        public static void ResetPassword(MyAccountInfo myAccountInfo, string passwordOld, string password1, string password2)
        {
            var user = ASPdatabaseNET.Users.UserSessionLogic.GetUserSession_ForClient().UserInfo;
            if (user.UserId != myAccountInfo.UserId)
                throw new Exception("Invalid Request");

            UI.PageParts.Users.Backend.UsersLogic.ValidatePassword(password1);
            if(password1 != password2)
                throw new Exception("'New Password' and 'Repeat New Password' do not match.");


            UI.PageParts.Users.Backend.UsersLogic.SavePassword(user.UserId, passwordOld, password1);
        }

    }
}