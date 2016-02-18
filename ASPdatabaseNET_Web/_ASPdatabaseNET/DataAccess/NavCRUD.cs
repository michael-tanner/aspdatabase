using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.DataAccess;
using ASPdatabaseNET.DataObjects.Nav;

namespace ASPdatabaseNET.DataAccess
{
    //----------------------------------------------------------------------------------------------------////
    public class NavCRUD
    {   
        //------------------------------------------------------------------------------------------ static --
        public static NavSiteInfo GetSiteNav(bool resetCache)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if(resetCache)
                ASPdatabaseNET.Memory.AppCache.Reset();

            var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
            var usersPermissions = userSessionInfo.UserInfo.AllPermissions;

            var rtn = new NavSiteInfo();
            var tmpNavDatabaseArray = new List<NavDatabaseInfo>();
            var dbList = SQLObjectsCRUD.ASPdb_Connection__GetAll(true);
            foreach(var dbItem in dbList)
                if (dbItem.Active)
                    tmpNavDatabaseArray.Add(new NavDatabaseInfo() { ConnectionId = dbItem.ConnectionId, ConnectionName = dbItem.ConnectionName });
            rtn.Databases = tmpNavDatabaseArray.ToArray();

            foreach (var database in rtn.Databases)
            {
                try
                {
                    var aspdb_Tables = SQLObjectsCRUD.ASPdb_Table__GetAll(database.ConnectionId, false);
                    var tmpList1 = new List<NavSectionItemInfo>();
                    foreach (var table in aspdb_Tables)
                    {
                        if (!table.Hide && usersPermissions.CheckPermissions(table.ConnectionId, table.Schema, table.TableId).View)
                        {
                            string url = String.Format("#00-Table-{0}", table.TableId);
                            tmpList1.Add(new NavSectionItemInfo() { Id = table.TableId, Name = table.TableName, Schema = table.Schema, URL = url });
                        }
                    }
                    var aspdb_Views = SQLObjectsCRUD.ASPdb_View__GetAll(database.ConnectionId);
                    var tmpList2 = new List<NavSectionItemInfo>();
                    foreach(var view in aspdb_Views)
                    {
                        if (!view.Hide && usersPermissions.CheckPermissions(view.ConnectionId, view.Schema, -1).View)
                        {
                            string url = String.Format("#00-View-{0}", view.ViewId);
                            tmpList2.Add(new NavSectionItemInfo() { Id = view.ViewId, Name = view.ViewName, Schema = view.Schema, URL = url });
                        }
                    }

                    if (tmpList1.Count > 0)
                        database.Section_Tables = new NavSectionInfo()
                        {
                            SectionName = String.Format("Tables ({0})", tmpList1.Count),
                            SectionType = NavSectionInfo.SectionTypes.Tables,
                            Items = tmpList1.ToArray()
                        };
                    if (tmpList2.Count > 0)
                        database.Section_Views = new NavSectionInfo()
                        {
                            SectionName = String.Format("Views ({0})", tmpList2.Count),
                            SectionType = NavSectionInfo.SectionTypes.Views,
                            Items = tmpList2.ToArray()
                        };
                }
                catch (Exception exc)
                {
                    ASPdb.Framework.Debug.RecordException(exc);
                    database.ConnectionName += " (Error)";
                    string msg = exc.Message;
                    if (msg.Length > 171)
                    {
                        msg = msg.Substring(0, 170);
                        var arr = msg.Split(new char[] { ' ' });
                        msg = msg.Substring(0, msg.Length - arr.Last().Length) + "...";
                    }
                    database.Section_Tables = new NavSectionInfo() { SectionName = msg };
                }
            }

            var tmpDatabases = new List<NavDatabaseInfo>();
            foreach (var database in rtn.Databases)
            {
                bool isEmpty = true;
                if(database.Section_Tables != null && database.Section_Tables.Items != null && database.Section_Tables.Items.Length > 0)
                {
                    isEmpty = false;
                    database.Section_Tables.Items = (from r in database.Section_Tables.Items
                                                     orderby r.Schema, r.Name
                                                     select r).ToArray();
                }
                if (database.Section_Views != null && database.Section_Views.Items != null && database.Section_Views.Items.Length > 0)
                {
                    isEmpty = false;
                }

                if(!isEmpty)
                    tmpDatabases.Add(database);
            }
            rtn.Databases = tmpDatabases.ToArray();

            rtn.IsInDemoMode = Users.UserSessionLogic.IsInDemoMode;

            return rtn;
        }
    }
}