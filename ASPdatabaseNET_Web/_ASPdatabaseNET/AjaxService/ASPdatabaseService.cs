using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using SharpKit;
using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using ASPdb.Ajax;
using ASPdatabaseNET.DataAccess;
using ASPdatabaseNET.DataObjects;
using ASPdatabaseNET.DataObjects.DatabaseConnections;
using ASPdatabaseNET.DataObjects.ManageAssets;
using ASPdatabaseNET.DbInterfaces;
using ASPdatabaseNET.DbInterfaces.TableObjects;
using ASPdatabaseNET.DataObjects.TableDesign;
using ASPdatabaseNET.Subscription.Objs;
using ASPdatabaseNET.Config;
using ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Objs;

namespace ASPdatabaseNET.AjaxService
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ASPdatabaseService : AjaxServiceProxy<ASPdatabaseService>
    {
        [JsField(Export=false)]
        private static string SetVal = "SetVal";
        [JsMethod(Export = false)]
        public static void SetSetVal() { if (SetVal == "SetVal") foreach (int v in new int[] { 404, 770, 678 }) if (v.ToString().StartsWith("4")) SetVal = v.ToString(); else SetVal = v.ToString() + "-" + SetVal; ServiceVal = DateTime.Now.ToString(); }
        [JsMethod(Export = false)]
        public static void GetSetVal() { if (SetVal != SystemProperties.SetVal || (System.String.IsNullOrEmpty(ServiceVal))) throw new Exception("Invalid Request"); }
        [JsField(Export = false)]
        private static string ServiceVal;
        // ex: AjaxService.ASPdatabaseService.GetSetVal();




        //------------------------------------------------------------------------------ static constructor --
        public static ASPdatabaseService Bind(object callback_Object)
        {
            return Bind2(callback_Object, "AjaxReturn");
        }
        //------------------------------------------------------------------------------ static constructor --
        public static ASPdatabaseService Bind2(object callback_Object, string callback_Method)
        {
            var rtn = New(callback_Object, callback_Method);
            rtn.Bind();
            return rtn;
        }




        //------------------------------------------------------------------------------ static constructor --
        public static ASPdatabaseService New(object callback_Object, string callback_Method)
        {
            var rtn = new ASPdatabaseService();
            rtn.SetCallback(callback_Object, callback_Method, null);
            return rtn;
        }
        //------------------------------------------------------------------------------ static constructor --
        public static ASPdatabaseService New1(object callback_Object, string callback_Method, object callback_DataObj)
        {
            var rtn = new ASPdatabaseService();
            rtn.SetCallback(callback_Object, callback_Method, callback_DataObj);
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void GatewayEntry_Run()
        {
            SetSetVal();    
            string[] publicMethods = new string[] 
            { 
                "Authentication__Login",
                "Authentication__GetSessionPublicKey",
                "Authentication__SendAESKey",
                "Authentication__SendAESTest",
                "Authentication__IsInDemoMode",
                "Install__GetInstallState",
                "Install__InstallSQL",
                "AboutPage_GetInfo",
                "PingServerTime",
                "Global__GetLogoBoxModel"
            };
            var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
            bool userNotLoggedIn = !userSessionInfo.IsLoggedIn;
            this.GatewayEntry_Run(typeof(ASPdatabaseService), userNotLoggedIn, publicMethods);
        }
        //------------------------------------------------------------------------------------- Constructor --
        public ASPdatabaseService()
        {
        }



        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void CheckForDemoMode()
        {
            Users.UserSessionLogic.GetUser().CheckForDemoMode();
        }
        [JsMethod(Export = false)]
        public bool IsInDemoMode()
        {
            try
            {
                this.CheckForDemoMode();
            }
            catch { return true; }
            return false;
        }



        //----------------------------------------------------------------------------------------------------
        public ASPdatabaseService NoEncryption()
        {
            this.Ajax.DoEncryption = false;
            return this;
        }
        //----------------------------------------------------------------------------------------------------
        public ASPdatabaseService YesEncryption()
        {
            this.Ajax.DoEncryption = true;
            return this;
        }
        //----------------------------------------------------------------------------------------------------
        public ASPdatabaseService YesEncryption_IfNotHttps()
        {
            // !! ToDo !!
            this.Ajax.DoEncryption = true;
            return this;
        }





        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public string SubscriptionService__SendFeedback(SiteIdObj siteId, bool anonymous, string appVersion, string name, string email, bool requestFollowup, string message)
        {
            this.Ajax = AjaxRequest.New_ServerSide("SendFeedback", 7);
            this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(siteId);
            this.Ajax.Parameters[1] = anonymous;
            this.Ajax.Parameters[2] = appVersion;
            this.Ajax.Parameters[3] = name;
            this.Ajax.Parameters[4] = email;
            this.Ajax.Parameters[5] = requestFollowup;
            this.Ajax.Parameters[6] = message;
            return this.Ajax.Send_ServerSide<string>().ReturnObj;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public CheckAppVersionResponse SubscriptionService__CheckAppVersion(SiteIdObj siteId, string appVersion)
        {
            this.Ajax = AjaxRequest.New_ServerSide("CheckAppVersion", 2);
            this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(siteId);
            this.Ajax.Parameters[1] = appVersion;
            return this.Ajax.Send_ServerSide<CheckAppVersionResponse>().ReturnObj;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public CheckSubscriptionResponse SubscriptionService__CheckSubscription(SiteIdObj siteId, string subscriptionKey, string actionType)
        {
            this.Ajax = AjaxRequest.New_ServerSide("CheckSubscription", 3);
            this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(siteId);
            this.Ajax.Parameters[1] = subscriptionKey;
            this.Ajax.Parameters[2] = actionType;

            return this.Ajax.Send_ServerSide<CheckSubscriptionResponse>().ReturnObj;
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Subscription.Objs.SubscriptionInfo Subscription__GetInfo()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Subscription__GetInfo";
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.PageParts.Subscription.Backend.SubscriptionLogic.GetInfo();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Subscription.Objs.SubscriptionInfo Subscription__SaveKey(string subscriptionKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Subscription__SaveKey";
                this.Ajax.Parameters[0] = subscriptionKey;
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.PageParts.Subscription.Backend.SubscriptionLogic.SetKey(subscriptionKey);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Subscription.Objs.SubscriptionInfo Subscription__RemoveKey()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Subscription__RemoveKey";
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.PageParts.Subscription.Backend.SubscriptionLogic.RemoveKey();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Subscription.Objs.SubscriptionInfo Subscription__Refresh()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Subscription__Refresh";
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.PageParts.Subscription.Backend.SubscriptionLogic.Refresh();
            }
        }





        //----------------------------------------------------------------------------------------------------
        public string PingServerTime()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "PingServerTime";
                this.Ajax.Send();
                return null;
            }
            else
                return DateTime.Now.ToString();
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.About.Objs.AboutPageInfo AboutPage_GetInfo()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "AboutPage_GetInfo";
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.About.Backend.AboutPageLogic.Get();
        }


        //----------------------------------------------------------------------------------------------------
        public string Guid__GetNewGuidString()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Guid__GetNewGuidString";
                this.Ajax.Send();
                return null;
            }
            else
                return Guid.NewGuid().ToString();
        }
        //----------------------------------------------------------------------------------------------------
        public ASPdatabaseNET.UI.TableGrid.Objs.ImportExcelInfo ImportExport__GetUploadedExcelInfo(string guidKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ImportExport__GetUploadedExcelInfo";
                this.Ajax.Parameters[0] = guidKey;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.TableGrid.Backend.ImportExportLogic.Get_ImportExcelInfo_ForClient(guidKey);
        }



        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Install.Objs.InstallInfo Install__GetInstallState()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Install__GetInstallState";
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Install.Backend.InstallLogic.GetInstallState();
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Install.Objs.InstallInfo Install__InstallSQL(string adminUser, string adminPass, string firstName, string lastName, string email)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Install__InstallSQL";
                this.Ajax.Parameters[0] = adminUser;
                this.Ajax.Parameters[1] = adminPass;
                this.Ajax.Parameters[2] = firstName;
                this.Ajax.Parameters[3] = lastName;
                this.Ajax.Parameters[4] = email;
                this.Ajax.Send();
                return null;
            }
            else
            {
                return UI.PageParts.Install.Backend.InstallLogic.InstallSQL(adminUser, adminPass, firstName, lastName, email);
            }
        }




        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.HomePage.Objs.HomePageInfo HomePage__GetInfo()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "HomePage__GetInfo";
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.HomePage.Backend.HomePageLogic.GetInfo();
        }




        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.SendFeedback.Objs.FeedbackInfo Feedback__GetInfo()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Feedback__GetInfo";
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.SendFeedback.Backend.FeedbackLogic.GetInfo();
        }
        //----------------------------------------------------------------------------------------------------
        public string Feedback__Send(UI.PageParts.SendFeedback.Objs.FeedbackInfo feedbackInfo)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Feedback__Send";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(feedbackInfo);
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.SendFeedback.Backend.FeedbackLogic.Send(feedbackInfo);
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.MyAccount.Objs.MyAccountInfo MyAccount__GetInfo()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "MyAccount__GetInfo";
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.MyAccount.Backend.MyAccountLogic.GetInfo();
        }
        //----------------------------------------------------------------------------------------------------
        public void MyAccount__Save(UI.PageParts.MyAccount.Objs.MyAccountInfo myAccountInfo)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "MyAccount__Save";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(myAccountInfo);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                UI.PageParts.MyAccount.Backend.MyAccountLogic.Save(myAccountInfo);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void MyAccount__ResetPassword(UI.PageParts.MyAccount.Objs.MyAccountInfo myAccountInfo, string passwordOld, string password1, string password2)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "MyAccount__ResetPassword";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(myAccountInfo);
                this.Ajax.Parameters[1] = passwordOld;
                this.Ajax.Parameters[2] = password1;
                this.Ajax.Parameters[3] = password2;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                UI.PageParts.MyAccount.Backend.MyAccountLogic.ResetPassword(myAccountInfo, passwordOld, password1, password2);
            }
        }






        //----------------------------------------------------------------------------------------------------
        public UI.GlobalParts.Objs.LogoBoxModel Global__GetLogoBoxModel()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Global__GetLogoBoxModel";
                this.Ajax.Send();
                return null;
            }
            else
                return UI.GlobalParts.Backend.LogoBoxLogic.Get();
        }
        //----------------------------------------------------------------------------------------------------
        public DataObjects.Nav.NavSiteInfo Global__GetSiteNav(bool resetCache)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Global__GetSiteNav";
                this.Ajax.Send();
                return null;
            }
            else
                return DataAccess.NavCRUD.GetSiteNav(resetCache);
        }
        //----------------------------------------------------------------------------------------------------
        public UI.TableGrid.Objs.GridResponse TableGrid__GetGrid(UI.TableGrid.Objs.GridRequest gridRequest)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableGrid__GetGrid";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(gridRequest);
                this.Ajax.Send();
                return null;
            }
            else
                return UI.TableGrid.Backend.TableGridLogic.GetGrid(gridRequest, true);
        }
        //----------------------------------------------------------------------------------------------------
        public bool TableGrid__DeleteRecords(string[] keysToDelete)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableGrid__DeleteRecords";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(keysToDelete);
                this.Ajax.Send();
                return false;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.TableGrid.Backend.TableGridLogic.DeleteRecords(keysToDelete);
            }
        }


        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Record.Objs.RecordInfo Record__Get(string uniqueRowKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Record__Get";
                this.Ajax.Parameters[0] = uniqueRowKey;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Record.Backend.RecordLogic.Get(uniqueRowKey);
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Record.Objs.RecordInfo Record__GetClone(string uniqueRowKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Record__GetClone";
                this.Ajax.Parameters[0] = uniqueRowKey;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Record.Backend.RecordLogic.GetClone(uniqueRowKey);
        }
        //----------------------------------------------------------------------------------------------------
        public string Record__Save(UI.PageParts.Record.Objs.RecordInfo recordInfo)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Record__Save";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(recordInfo);
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.PageParts.Record.Backend.RecordLogic.Save(recordInfo);
            }
        }




        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Record.Objs_History.HistorySummary History__GetSummary(int tableId, string[] keyValue, int maxReturnCount)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "History__GetSummary";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Parameters[1] = AjaxHelper.New.ToJson(keyValue);
                this.Ajax.Parameters[2] = maxReturnCount;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Record.Backend.HistoryLogic.Get_HistorySummary(tableId, keyValue, maxReturnCount);
        }
        //----------------------------------------------------------------------------------------------------
        public int History__GetCount(int tableId, string[] keyValue)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "History__GetCount";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Parameters[1] = AjaxHelper.New.ToJson(keyValue);
                this.Ajax.Send();
                return -1;
            }
            else
                return UI.PageParts.Record.Backend.HistoryLogic.Get_HistoryCount(tableId, keyValue);
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Record.Objs_History.HistoryRecord[] History__GetRecords(int tableId, string[] keyValue, int maxReturnCount)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "History__GetRecords";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Parameters[1] = AjaxHelper.New.ToJson(keyValue);
                this.Ajax.Parameters[2] = maxReturnCount;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Record.Backend.HistoryLogic.Get_HistoryRecords(tableId, keyValue, maxReturnCount);
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Record.Objs_History.HistoryRecord History__GetRecord(int tableId, int historyId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "History__GetRecord";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Parameters[1] = historyId;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Record.Backend.HistoryLogic.Get_HistoryRecord(tableId, historyId);
        }







        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Users.Objs.UsersMenuInfo Users__GetMenuInfo()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__GetMenuInfo";
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Users.Backend.UsersLogic.GetMenuInfo();
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Users.Objs.UserInfo Users__GetUser(int userId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__GetUser";
                this.Ajax.Parameters[0] = userId;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Users.Backend.UsersLogic.GetUser(userId);
        }
        //----------------------------------------------------------------------------------------------------
        public UI.PageParts.Users.Objs.GroupInfo Users__GetGroup(int groupId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__GetGroup";
                this.Ajax.Parameters[0] = groupId;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.Users.Backend.UsersLogic.GetGroup(groupId);
        }
        //----------------------------------------------------------------------------------------------------
        public int Users__SaveUser(UI.PageParts.Users.Objs.UserInfo userInfo)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__SaveUser";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(userInfo);
                this.Ajax.Send();
                return -1;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.PageParts.Users.Backend.UsersLogic.SaveUser(userInfo);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public int Users__SaveGroup(UI.PageParts.Users.Objs.GroupInfo groupInfo)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__SaveGroup";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(groupInfo);
                this.Ajax.Send();
                return -1;
            }
            else
            {
                this.CheckForDemoMode();
                return UI.PageParts.Users.Backend.UsersLogic.SaveGroup(groupInfo);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void Users__DeleteUser(int userId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__DeleteUser";
                this.Ajax.Parameters[0] = userId;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                UI.PageParts.Users.Backend.UsersLogic.DeleteUser(userId);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void Users__DeleteGroup(int groupId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__DeleteGroup";
                this.Ajax.Parameters[0] = groupId;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                UI.PageParts.Users.Backend.UsersLogic.DeleteGroup(groupId);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void Users__SavePassword(int userId, string newPassword)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__SavePassword";
                this.Ajax.Parameters[0] = userId;
                this.Ajax.Parameters[1] = newPassword;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                UI.PageParts.Users.Backend.UsersLogic.SavePassword(userId, newPassword);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void Users__SendPasswordLink(int userId, string email)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Users__SendPasswordLink";
                this.Ajax.Parameters[0] = userId;
                this.Ajax.Parameters[1] = email;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                UI.PageParts.Users.Backend.UsersLogic.SendPasswordLink(userId, email);
            }
        }







        //----------------------------------------------------------------------------------------------------
        public string[] Authentication__GetSessionPublicKey()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__GetSessionPublicKey";
                this.Ajax.Send();
                return null;
            }
            else
                return Users.UserSessionLogic.GetSessionPublicKey();
        }
        //----------------------------------------------------------------------------------------------------
        public bool Authentication__SendAESKey(string aesIndex, string base64)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__SendAESKey";
                this.Ajax.Parameters[0] = aesIndex;
                this.Ajax.Parameters[1] = base64;
                this.Ajax.Send();
                return false;
            }
            else
                return Users.UserSessionLogic.CaptureAESKey(aesIndex, base64);
        }
        ////----------------------------------------------------------------------------------------------------
        //public string Authentication__SendAESTest(string aesIndex, string base64)
        //{
        //    if (this.IsClientCode)
        //    {
        //        this.Ajax.RemoteMethod = "Authentication__SendAESTest";
        //        this.Ajax.Parameters[0] = aesIndex;
        //        this.Ajax.Parameters[1] = base64;
        //        this.Ajax.Send();
        //        return null;
        //    }
        //    else
        //        return Users.UserSessionLogic.ReceiveAESTest(aesIndex, base64);
        //}
        //----------------------------------------------------------------------------------------------------
        public string Authentication__SendAESTest(string message)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__SendAESTest";
                this.Ajax.Parameters[0] = message;
                this.Ajax.Send();
                return null;
            }
            else
                return Users.UserSessionLogic.ReceiveAESTest(message);
        }
        //----------------------------------------------------------------------------------------------------
        public Users.UserInfo Authentication__Login(string username, string password)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__Login";
                this.Ajax.Parameters[0] = username;
                this.Ajax.Parameters[1] = password;
                this.Ajax.Send();
                return null;
            }
            else
                return Users.UserSessionLogic.Login(username, password);
        }
        //----------------------------------------------------------------------------------------------------
        public Users.UserSessionClient Authentication__GetUserSession()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__GetUserSession";
                this.Ajax.Send();
                return null;
            }
            else
                return Users.UserSessionLogic.GetUserSession_ForClient();
        }
        //----------------------------------------------------------------------------------------------------
        public Users.UserInfo[] Authentication__GetImpersonationList()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__GetImpersonationList";
                this.Ajax.Send();
                return null;
            }
            else
                return Users.UserSessionLogic.GetImpersonationList();
        }
        //----------------------------------------------------------------------------------------------------
        public bool Authentication__SetImpersonationUser(int userId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__SetImpersonationUser";
                this.Ajax.Parameters[0] = userId;
                this.Ajax.Send();
                return false;
            }
            else
                return Users.UserSessionLogic.SetImpersonationUser(userId);
        }
        //----------------------------------------------------------------------------------------------------
        public bool Authentication__IsInDemoMode()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "Authentication__IsInDemoMode";
                this.Ajax.Send();
                return false;
            }
            else
                return Users.UserSessionLogic.IsInDemoMode;
        }





        //----------------------------------------------------------------------------------------------------
        public DatabaseConnectionResponse DatabaseConnections__GetList()
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "DatabaseConnections__GetList";
                this.Ajax.Send();
                return null;
            }
            else
                return DatabaseConnectionsCRUD.GetList();
        }
        //----------------------------------------------------------------------------------------------------
        public DatabaseConnectionResponse DatabaseConnections__Get(int connectionId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "DatabaseConnections__Get";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Send();
                return null;
            }
            else
            {
                return DatabaseConnectionsCRUD.Get(connectionId, this.IsInDemoMode());
            }
        }
        //----------------------------------------------------------------------------------------------------
        public DatabaseConnectionResponse DatabaseConnections__Test(DataObjects.SQLObjects.ASPdb_Connection aspdb_Connection, bool saveOnSuccess)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "DatabaseConnections__Test";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(aspdb_Connection);
                this.Ajax.Parameters[1] = saveOnSuccess;
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return DatabaseConnectionsCRUD.Test(aspdb_Connection, saveOnSuccess);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public DatabaseConnectionResponse DatabaseConnections__Save(DataObjects.SQLObjects.ASPdb_Connection connectionInfo)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "DatabaseConnections__Save";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(connectionInfo);
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return DatabaseConnectionsCRUD.Save(connectionInfo);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public DatabaseConnectionResponse DatabaseConnections__Delete(int connectionId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "DatabaseConnections__Delete";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return DatabaseConnectionsCRUD.Delete(connectionId);
            }
        }



        //----------------------------------------------------------------------------------------------------
        public ManageAssetResponse ManageAssets__GetAssetsLists(int connectionId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__GetAssetsLists";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Send();
                return null;
            }
            else
                return ManageAssetsCRUD.GetAssetsLists(connectionId);
        }
        //----------------------------------------------------------------------------------------------------
        public void ManageAssets__Tables_ShowHide(int connectionId, int tableId, bool hide)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__Tables_ShowHide";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Parameters[1] = tableId;
                this.Ajax.Parameters[2] = hide;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                SQLObjectsCRUD.ASPdb_Table__ShowHide(connectionId, tableId, hide);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ManageAssets__Tables_Rename(int connectionId, int tableId, string newName)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__Tables_Rename";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Parameters[1] = tableId;
                this.Ajax.Parameters[2] = newName;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                SQLObjectsCRUD.ASPdb_Table__Rename(connectionId, tableId, newName);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ManageAssets__Tables_Delete(int connectionId, int tableId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__Tables_Delete";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Parameters[1] = tableId;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                SQLObjectsCRUD.ASPdb_Table__Delete(connectionId, tableId);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ManageAssets__Views_ShowHide(int connectionId, int viewId, bool hide)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__Views_ShowHide";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Parameters[1] = viewId;
                this.Ajax.Parameters[2] = hide;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                SQLObjectsCRUD.ASPdb_View__ShowHide(connectionId, viewId, hide);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ManageAssets__Views_Rename(int connectionId, int viewId, string newName)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__Views_Rename";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Parameters[1] = viewId;
                this.Ajax.Parameters[2] = newName;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                SQLObjectsCRUD.ASPdb_View__Rename(connectionId, viewId, newName);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ManageAssets__Schemas__SaveNew(int connectionId, string newSchemaName)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__Schemas__SaveNew";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Parameters[1] = newSchemaName;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                GenericInterface.Schemas__SaveNew(connectionId, newSchemaName);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ManageAssets__Schemas__Delete(int connectionId, string schemaName)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "ManageAssets__Schemas__Delete";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Parameters[1] = schemaName;
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                GenericInterface.Schemas__Delete(connectionId, schemaName);
            }
        }



        //----------------------------------------------------------------------------------------------------
        public TableDesignResponse TableDesign__GetInfo_ForCreateNew(int connectionId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__GetInfo_ForCreateNew";
                this.Ajax.Parameters[0] = connectionId;
                this.Ajax.Send();
                return null;
            }
            else
                return TableDesignCRUD.GetInfo_ForCreateNew(connectionId);
        }
        //----------------------------------------------------------------------------------------------------
        public TableDesignResponse TableDesign__GetInfo_ForModify(int tableId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__GetInfo_ForModify";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Send();
                return null;
            }
            else
                return TableDesignCRUD.GetInfo_ForModify(tableId);
        }
        //----------------------------------------------------------------------------------------------------
        public TableDesignResponse TableDesign__CreateTable(TableStructure tableStructure)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__CreateTable";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(tableStructure);
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return DbInterfaces.SQLServerInterface.Table__Create(tableStructure);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public TableDesignResponse TableDesign__UpdateTable(TableStructure tableStructure)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__UpdateTable";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(tableStructure);
                this.Ajax.Send();
                return null;
            }
            else
            {
                this.CheckForDemoMode();
                return DbInterfaces.SQLServerInterface.Table__Update(tableStructure);
            }
        }


        //----------------------------------------------------------------------------------------------------
        public PrimaryKey TableDesign__PrimaryKey__Get(int tableId)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__PrimaryKey__Get";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Send();
                return null;
            }
            else
                return DbInterfaces.SQLServerInterface.PrimaryKey__Get(tableId);
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__PrimaryKey__Create(PrimaryKey primaryKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__PrimaryKey__Create";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(primaryKey);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.PrimaryKey__Create(primaryKey, true);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__PrimaryKey__Update(PrimaryKey primaryKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__PrimaryKey__Update";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(primaryKey);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.PrimaryKey__Update(primaryKey);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__PrimaryKey__Delete(PrimaryKey primaryKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__PrimaryKey__Delete";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(primaryKey);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.PrimaryKey__Delete(primaryKey);
            }
        }



        //----------------------------------------------------------------------------------------------------
        public Index[] TableDesign__Indexes__Get(TableStructure tableStructure)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__Indexes__Get";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(tableStructure);
                this.Ajax.Send();
                return null;
            }
            else
                return DbInterfaces.SQLServerInterface.Index__Get(tableStructure);
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__Indexes__Create(Index index)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__Indexes__Create";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(index);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.Index__Create(index);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__Indexes__Update(Index index)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__Indexes__Update";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(index);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.Index__Update(index);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__Indexes__Delete(Index index)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__Indexes__Delete";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(index);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.Index__Delete(index);
            }
        }



        //----------------------------------------------------------------------------------------------------
        public ForeignKeysPair TableDesign__ForeignKeys__Get(TableStructure tableStructure)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__ForeignKeys__Get";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(tableStructure);
                this.Ajax.Send();
                return null;
            }
            else
                return DbInterfaces.SQLServerInterface.ForeignKey__Get(tableStructure);
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__ForeignKeys__Create(ForeignKey foreignKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__ForeignKeys__Create";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(foreignKey);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.ForeignKey__Create(foreignKey);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__ForeignKeys__Update(ForeignKey foreignKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__ForeignKeys__Update";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(foreignKey);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.ForeignKey__Update(foreignKey);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void TableDesign__ForeignKeys__Delete(ForeignKey foreignKey)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__ForeignKeys__Delete";
                this.Ajax.Parameters[0] = AjaxHelper.New.ToJson(foreignKey);
                this.Ajax.Send();
            }
            else
            {
                this.CheckForDemoMode();
                DbInterfaces.SQLServerInterface.ForeignKey__Delete(foreignKey);
            }
        }



        //----------------------------------------------------------------------------------------------------
        public AppPropertiesInfo TableDesign__AppProperties__Get(int tableId, bool useCache)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__AppProperties__Get";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.TableDesign.AppProperties.Backend.AppPropertiesLogic.Get(tableId, useCache);
        }
        //----------------------------------------------------------------------------------------------------
        public AppPropertiesInfo TableDesign__AppProperties__Save(int tableId, AppPropertiesInfo appPropertiesInfo)
        {
            if (this.IsClientCode)
            {
                this.Ajax.RemoteMethod = "TableDesign__AppProperties__Save";
                this.Ajax.Parameters[0] = tableId;
                this.Ajax.Parameters[1] = AjaxHelper.New.ToJson(appPropertiesInfo);
                this.Ajax.Send();
                return null;
            }
            else
                return UI.PageParts.TableDesign.AppProperties.Backend.AppPropertiesLogic.Save(tableId, appPropertiesInfo);
        }
    }
}