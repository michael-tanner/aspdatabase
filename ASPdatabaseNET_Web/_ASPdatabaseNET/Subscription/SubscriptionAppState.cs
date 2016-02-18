using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdb.UniversalADO;
using System.Security.Cryptography;
using System.Text;

namespace ASPdatabaseNET.Subscription
{
    //----------------------------------------------------------------------------------------------------////
    public class SubscriptionAppState
    {
        private static string AppKey = "ASPdb_SubAppState";

        //----------------------------------------------------------------------------------------------------
        public static bool ValidateActiveSubscribers()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                bool isFreeAccount = false;
                int subscribersCount = GetSubscribersCount();
                if (subscribersCount < 1)
                {
                    isFreeAccount = true;
                    subscribersCount = 1;
                }

                string sql = String.Format(@"
                    select [UserId], [Username], [Active], [IsAdmin] 
                    from [{0}].[ASPdb_Users] where [Active] = 1 order by [UserId]", Config.SystemProperties.AppSchema);
                var list = new List<Users.UserInfo>();
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        while(reader.Read())
                        {
                            list.Add(new Users.UserInfo()
                            {
                                UserId = reader.Get("UserId", -1),
                                Username = reader.Get("Username", ""),
                                Active = reader.Get("Active", false),
                                IsAdmin = reader.Get("IsAdmin", false)
                            });
                        }
                    }
                }
                string userIds_ToDeactivate = "";
                if (list.Count > subscribersCount)
                    for (int i = subscribersCount; i < list.Count; i++)
                        userIds_ToDeactivate += list[i].UserId + ", ";
                if (userIds_ToDeactivate != "")
                {
                    userIds_ToDeactivate = userIds_ToDeactivate.Substring(0, userIds_ToDeactivate.Length - 2);
                    sql = String.Format(@"update [{0}].[ASPdb_Users] set [Active] = 0 where [UserId] in ({1})", 
                        Config.SystemProperties.AppSchema, userIds_ToDeactivate);
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        command.ExecuteNonQuery();
                    }
                }


                if(isFreeAccount)
                {
                    var ids = new List<int>();
                    sql = String.Format("select [ConnectionId] from [{0}].[ASPdb_Connections] where [Active] = 1 order by [ConnectionId]", Config.SystemProperties.AppSchema);
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            while (reader.Read())
                                ids.Add(reader.Get("ConnectionId", -1));
                        }
                    }
                    if(ids.Count > 1)
                    {
                        string idsToDeactivate = "";
                        for (int i = 1; i < ids.Count; i++)
                        {
                            if (idsToDeactivate != "") idsToDeactivate += ", ";
                            idsToDeactivate += ids[i];
                        }
                        sql = String.Format(@"update [{0}].[ASPdb_Connections] set [Active] = 0 where [ConnectionId] in ({1})", Config.SystemProperties.AppSchema, idsToDeactivate);
                        using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }


                return true;
            }
            catch
            {
                return false;
            }
        }
        
        //----------------------------------------------------------------------------------------------------
        public static int GetSubscribersCount()
        {
            return GetSubscribersCount("Check");
        }
        //----------------------------------------------------------------------------------------------------
        public static int GetSubscribersCount(string actionType)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var appState = GetFromApplication();
            
            if (String.IsNullOrEmpty(appState.SubscriptionKey))
                return 0;

            if(appState.SubscribersCount > 0)
            {
                string str = 
                    appState.SubscriptionKey +
                    appState.SubscribersCount +
                    "ASPdatabase.NET 2014 " +
                    (appState.SubscribersCount * 1024);
                if (BCrypt.Net.BCrypt.Verify(GetHashString(str), appState.SubscriptionCode))
                {
                    return appState.SubscribersCount;
                }
                else
                {
                    appState.SubscribersCount = -1;
                    string temp = BCrypt.Net.BCrypt.HashString("invalid subscription", 18);
                }
            }


            try
            {
                var siteId = Subscription.Objs.SiteIdObj.GetNew();
                var response = (new AjaxService.ASPdatabaseService())
                    .SubscriptionService__CheckSubscription(siteId, appState.SubscriptionKey, actionType);
                appState.SubscribersCount = response.SubscriptionCount;
                appState.SubscriptionCode = response.SubscriptionCode;
                appState.HistoryCode = response.HistoryCode;
                appState.LastCheckTime = DateTime.Now;

                string json = (new ASPdb.Ajax.AjaxHelper()).ToJson(response); // update SubscriptionHistory
                Config.ASPdb_Values.Set("SubscriptionHistory", json);

                return appState.SubscribersCount;
            }
            catch (Exception exc)
            {
                ASPdb.Framework.Debug.RecordException(exc);
                // record SubscriptionCheckError (ToDo in later version)
            }


            // get last known value in SubscriptionHistory
            string json2 = Config.ASPdb_Values.GetFirstValue("SubscriptionHistory", "");
            if (json2.Length < 1)
                return 0;

            ASPdb.Framework.Debug.WriteLine("08 ... Get from History");

            var historyObj = (new ASPdb.Ajax.AjaxHelper()).FromJson<Subscription.Objs.CheckSubscriptionResponse>(json2);
            var lastItem = historyObj.HistoryItems[historyObj.HistoryItems.Length - 1];
            
            if (historyObj.SubscriptionCount != lastItem.SubscriptionCount)
                return 0;

                string str1 = 
                    appState.SubscriptionKey +
                    historyObj.SubscriptionCount + 
                    "ASPdatabase.NET 2014 " +
                    (appState.SubscribersCount * 1024);
                string str2 =
                    appState.SubscriptionKey +
                    lastItem.SubscriptionCount +
                    "ASPdatabase.NET 2014 " +
                    (lastItem.SubscriptionCount * 2048) +
                    lastItem.StartTime.GetValueOrDefault().ToString();

                if (BCrypt.Net.BCrypt.Verify(GetHashString(str1), historyObj.SubscriptionCode))
                    return 0;
                if (BCrypt.Net.BCrypt.Verify(GetHashString(str2), historyObj.HistoryCode))
                    return 0;

                return historyObj.SubscriptionCount;
        }
        //----------------------------------------------------------------------------------------------------
        public static string Get_LastCheck_MinutesLapsed()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var appState = GetFromApplication();

            try
            {
                if (appState.LastCheckTime.HasValue)
                {
                    string mins = (DateTime.Now - appState.LastCheckTime.Value).TotalMinutes.ToString();
                    mins = mins.Split(new char[] { '.' }).First();
                    return mins;
                }
            }
            catch { }

            return "";
        }


        //----------------------------------------------------------------------------------------------------
        public static string GetHashString(string inputString)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var sb = new StringBuilder();
            var algorithm = MD5.Create();
            var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        //----------------------------------------------------------------------------------------------------
        private static SubscriptionAppState GetFromApplication()
        {
            SubscriptionAppState rtn = null;
            try
            {
                rtn = (SubscriptionAppState)HttpContext.Current.Application[AppKey];
            }
            catch { }
            if(rtn == null)
            {
                rtn = new SubscriptionAppState();
                HttpContext.Current.Application[AppKey] = rtn;
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static void ClearAppState()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try { HttpContext.Current.Application[AppKey] = null; }
            catch { }
        }


        private int SubscribersCount;
        private string SubscriptionCode;
        private string SubscriptionKey;
        private string HistoryCode;
        private DateTime? LastCheckTime;

        //------------------------------------------------------------------------------------- Constructor --
        public SubscriptionAppState()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            this.SubscribersCount = -1;
            this.SubscriptionCode = "";
            this.SubscriptionKey = Config.ASPdb_Values.GetFirstValue("SubscriptionKey", "");
        }


    }
}