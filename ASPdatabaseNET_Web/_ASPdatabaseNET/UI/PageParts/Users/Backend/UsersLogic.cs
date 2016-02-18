using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.Users.Objs;
using ASPdb.UniversalADO;
using System.Text.RegularExpressions;

namespace ASPdatabaseNET.UI.PageParts.Users.Backend
{
    public class UsersLogic
    {
        public static int WorkFactor = 12;

        //----------------------------------------------------------------------------------------------------
        public static UsersMenuInfo GetMenuInfo()
        {
            if (!ASPdatabaseNET.Subscription.SubscriptionAppState.ValidateActiveSubscribers())
                throw new Exception("Validation Error");

            var rtn = new UsersMenuInfo();
            rtn.UserSubscriptions_Total = ASPdatabaseNET.Subscription.SubscriptionAppState.GetSubscribersCount();
            //rtn.UserSubscriptions_Active = 7;
            rtn.UserSubscriptions_Message = "User Subscriptions Message";

            var users_Active = new List<UsersMenuItem>();
            var users_Inactive = new List<UsersMenuItem>();
            var groups_Active = new List<UsersMenuItem>();
            var groups_Inactive = new List<UsersMenuItem>();

            string sql = String.Format(@"
                select [UserId], [Username], [FirstName], [LastName], [Active] 
                from [{0}].[ASPdb_Users] 
                order by [Active], [LastName], [FirstName], [Username]", Config.SystemProperties.AppSchema);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while(reader.Read())
                    {
                        var item = new UsersMenuItem() { Id = reader.Get("UserId", -1), Active = reader.Get("Active", false), MenuType = UsersMenuItem.MenuTypes.User };
                        string firstName = reader.Get("FirstName", "");
                        string lastName = reader.Get("LastName", "");
                        string username = reader.Get("Username", "?");

                        if (firstName.Length > 0 && lastName.Length > 0)
                            item.DisplayName = String.Format("<b>{0}, {1}</b> <br />({2})", lastName, firstName, username);
                        else //if(firstName.Length > 0 || lastName.Length > 0)
                            item.DisplayName = String.Format("<b>{0} {1}</b> <br />({2})", lastName, firstName, username);
                        //else 
                        //    item.DisplayName = String.Format("({2})", lastName, firstName, username);

                        if (item.Active)
                            users_Active.Add(item);
                        else 
                            users_Inactive.Add(item);
                    }
                }
            }
            sql = "select [GroupId], [GroupName], [Active] from [" + Config.SystemProperties.AppSchema + "].[ASPdb_UserGroups] order by [Active], [GroupName]";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        var item = new UsersMenuItem() { Id = reader.Get("GroupId", -1), DisplayName = reader.Get("GroupName", "?"), Active = reader.Get("Active", false), MenuType = UsersMenuItem.MenuTypes.Group };
                        if (item.Active)
                            groups_Active.Add(item);
                        else
                            groups_Inactive.Add(item);
                    }
                }
            }
            rtn.Users_Active = users_Active.ToArray();
            rtn.Users_Inactive = users_Inactive.ToArray();
            rtn.Groups_Active = groups_Active.ToArray();
            rtn.Groups_Inactive = groups_Inactive.ToArray();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static UserInfo GetUser(int userId)
        {
            UserInfo rtn = null;
            try
            {
                string sql = "";
                if (userId >= 0)
                {
                    sql = "select * from [" + Config.SystemProperties.AppSchema + "].[ASPdb_Users] where [UserId] = @UserId";
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        command.AddParameter("@UserId", userId);
                        using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            if (reader.Read())
                                rtn = new UserInfo()
                                {
                                    UserId = reader.Get("UserId", -1),
                                    Username = reader.Get("Username", ""),
                                    FirstName = reader.Get("FirstName", ""),
                                    LastName = reader.Get("LastName", ""),
                                    Email = reader.Get("Email", ""),
                                    Active = reader.Get("Active", false),
                                    TimeCreated_Str = reader.Get("TimeCreated", ""),
                                    LastLoginTime_Str = reader.Get("LastLoginTime", ""),
                                    RequirePasswordReset = reader.Get("RequirePasswordReset", false),
                                    IsAdmin = reader.Get("IsAdmin", false),
                                    UserGroups = new UserToGroup_Assignment[0]
                                };
                        }
                    }
                }
                else rtn = new UserInfo() { UserId = -1, Active = true };

                if(rtn != null)
                {
                    if (!String.IsNullOrEmpty(rtn.TimeCreated_Str))
                        rtn.TimeCreated_Str = DateTime.Parse(rtn.TimeCreated_Str).ToString("M/dd/yy h:mm tt");
                    if (!String.IsNullOrEmpty(rtn.LastLoginTime_Str))
                        rtn.LastLoginTime_Str = DateTime.Parse(rtn.LastLoginTime_Str).ToString("M/dd/yy h:mm tt");

                    var userGroups = new Dictionary<int, UserToGroup_Assignment>();
                    sql = "select * from [" + Config.SystemProperties.AppSchema + "].[ASPdb_UserGroups] where [Active] = 1 order by [GroupName]";
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            while(reader.Read())
                            {
                                int groupId = reader.Get("GroupId", -1);
                                if (!userGroups.ContainsKey(groupId))
                                    userGroups.Add(groupId, new UserToGroup_Assignment()
                                    {
                                        GroupId = groupId,
                                        GroupName = reader.Get("GroupName", ""),
                                        IsMember = false,
                                        UserId = rtn.UserId
                                    });
                            }
                        }
                    }
                    
                    sql = "select * from [" + Config.SystemProperties.AppSchema + "].[ASPdb_UsersToGroups] where [UserId] = @UserId";
                    using(DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        command.AddParameter("@UserId", userId);
                        using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            while(reader.Read())
                            {
                                int groupId = reader.Get("GroupId", -1);
                                if (groupId >= 0 && userGroups.ContainsKey(groupId))
                                    userGroups[groupId].IsMember = true;
                            }
                        }
                    }
                    rtn.UserGroups = userGroups.Values.ToArray();
                }
            }
            catch { }
            if (rtn == null)
                throw new Exception("UserId not found.");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static GroupInfo GetGroup(int groupId)
        {
            GroupInfo rtn = null;
            try
            {
                string sql = "";
                if (groupId >= 0)
                {
                    sql = "select * from [" + Config.SystemProperties.AppSchema + "].[ASPdb_UserGroups] where [GroupId] = @GroupId";
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        command.AddParameter("@GroupId", groupId);
                        using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            if (reader.Read())
                                rtn = new GroupInfo()
                                {
                                    GroupId = reader.Get("GroupId", -1),
                                    GroupName = reader.Get("GroupName", ""),
                                    Active = reader.Get("Active", false),
                                    TimeCreated_Str = reader.Get("TimeCreated", ""),
                                    Permissions = new Permission[0]
                                };
                        }
                    }
                }
                else rtn = new GroupInfo() { GroupId = -1, Active = true };


                if(rtn != null)
                {

                    if (!String.IsNullOrEmpty(rtn.TimeCreated_Str))
                        rtn.TimeCreated_Str = DateTime.Parse(rtn.TimeCreated_Str).ToString("M/dd/yy h:mm tt");

                    var allPermissions = new Dictionary<string, Permission>();
                    sql = @"select 
                            C.ConnectionId, C.ConnectionName, T.TableId, T.[Schema], T.TableName
                            from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections] C inner join [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Tables] T on C.ConnectionId = T.ConnectionId
                            where C.Active = 1 and T.Hide = 0
                            order by C.ConnectionName, T.[Schema], T.TableName";
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            while (reader.Read())
                            {
                                var permission = new Permission()
                                {
                                    GroupId = groupId,
                                    ConnectionId = reader.Get("ConnectionId", -1),
                                    ConnectionName = reader.Get("ConnectionName", ""),
                                    TableId = reader.Get("TableId", -1),
                                    Schema = reader.Get("Schema", ""),
                                    TableName = reader.Get("TableName", "")
                                };
                                string connection_Key = "C_" + permission.ConnectionId;
                                string schema_Key = "S_" + permission.ConnectionId + "_[" + permission.Schema + "]";
                                string table_Key = "T_" + permission.ConnectionId + "_[" + permission.Schema + "]_[" + permission.TableId + "]";

                                if (!allPermissions.ContainsKey(connection_Key))
                                    allPermissions.Add(connection_Key, new Permission(Permission.PermissionTypes.Connection, permission.PermissionId, groupId)
                                    {
                                        ConnectionId = permission.ConnectionId,
                                        ConnectionName = permission.ConnectionName,
                                        Schema = "",
                                        TableId = -1,
                                        TableName = ""
                                    });
                                if (!allPermissions.ContainsKey(schema_Key))
                                    allPermissions.Add(schema_Key, new Permission(Permission.PermissionTypes.Schema, permission.PermissionId, groupId)
                                    {
                                        ConnectionId = permission.ConnectionId,
                                        ConnectionName = permission.ConnectionName,
                                        Schema = permission.Schema,
                                        TableId = -1,
                                        TableName = ""
                                    });
                                if (!allPermissions.ContainsKey(table_Key))
                                    allPermissions.Add(table_Key, new Permission(Permission.PermissionTypes.Table, permission.PermissionId, groupId)
                                    {
                                        ConnectionId = permission.ConnectionId,
                                        ConnectionName = permission.ConnectionName,
                                        Schema = permission.Schema,
                                        TableId = permission.TableId,
                                        TableName = permission.TableName
                                    });
                            }
                        }
                    }

                    sql = @"
                        select * from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions] 
                        where [GroupId] = @GroupId 
                        order by [ConnectionId], [Schema], [TableId]";
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                    {
                        command.AddParameter("@GroupId", groupId);
                        using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            while (reader.Read())
                            {
                                var permission = new Permission()
                                {
                                    GroupId = groupId,
                                    PermissionId = reader.Get("PermissionId", -1),
                                    PermissionType = Permission.PermissionTypes.NotSet,
                                    ConnectionId = reader.Get("ConnectionId", -1),
                                    Schema = reader.Get("Schema", ""),
                                    TableId = reader.Get("TableId", -1),
                                    View = reader.Get("View", false),
                                    Edit = reader.Get("Edit", false),
                                    Insert = reader.Get("Insert", false),
                                    Delete = reader.Get("Delete", false),
                                };
                                permission.Set_PermissionType(reader.Get("PermissionType", "N"));
                                string key = permission.Get_UniqueKey();
                                if(key != null && allPermissions.ContainsKey(key))
                                {
                                    allPermissions[key].PermissionId = permission.PermissionId;
                                    allPermissions[key].View = permission.View;
                                    allPermissions[key].Edit = permission.Edit;
                                    allPermissions[key].Insert = permission.Insert;
                                    allPermissions[key].Delete = permission.Delete;
                                }
                            }
                        }
                    }
                    rtn.Permissions = (from r in allPermissions.Values where r.PermissionType == Permission.PermissionTypes.Connection orderby r.ConnectionName select r).ToArray();
                    foreach(var item_Connection in rtn.Permissions)
                    {
                        item_Connection.SubPermissions = (from r in allPermissions.Values
                                                where r.PermissionType == Permission.PermissionTypes.Schema
                                                && r.ConnectionId == item_Connection.ConnectionId
                                                select r).ToArray();
                        foreach(var item_Schema in item_Connection.SubPermissions)
                        {
                            item_Schema.SubPermissions = (from r in allPermissions.Values
                                                          where r.PermissionType == Permission.PermissionTypes.Table
                                                          && r.ConnectionId == item_Connection.ConnectionId
                                                          && r.Schema == item_Schema.Schema
                                                          select r).ToArray();
                        }
                    }
                }
            }
            catch (Exception exc) { ASPdb.Framework.Debug.RecordException(exc); }
            if (rtn == null)
                throw new Exception("GroupId not found.");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static void SaveUser_BasicInfo(UserInfo userInfo)
        {
            // CONFIRM this is the user currently logged in ! ! !

            if (userInfo.UserId < 0)
                throw new Exception("Invalid UserInfo");

            string sql = @" update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users]
                            set [FirstName]            = @FirstName,
                                [LastName]             = @LastName,
                                [Email]                = @Email
                            where [UserId] = @UserId; ";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@FirstName", userInfo.FirstName);
                command.AddParameter("@LastName", userInfo.LastName);
                command.AddParameter("@Email", userInfo.Email);
                command.AddParameter("@UserId", userInfo.UserId);
                command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static int SaveUser(UserInfo userInfo)
        {
            // CONFIRM Username is not already taken ! ! !

            // CONFIRM At least one user is still an Admin ! ! !

            string sql = "";
            bool doInsert = (userInfo.UserId < 0);
            if (doInsert)
                sql = @"insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users]
                        ([Username], [FirstName], [LastName], [Email], [Active], [RequirePasswordReset], [IsAdmin], [TimeCreated], [Password])
                        values
                        (@Username, @FirstName, @LastName, @Email, @Active, @RequirePasswordReset, @IsAdmin, @TimeCreated, ''); 
                        select scope_identity();
                        ";
            else
                sql = @"update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users]
                        set [Username]             = @Username,
                            [FirstName]            = @FirstName,
                            [LastName]             = @LastName,
                            [Email]                = @Email,
                            [Active]               = @Active,
                            [RequirePasswordReset] = @RequirePasswordReset,
                            [IsAdmin]              = @IsAdmin
                        where [UserId] = @UserId; ";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@Username", userInfo.Username);
                command.AddParameter("@FirstName", userInfo.FirstName);
                command.AddParameter("@LastName", userInfo.LastName);
                command.AddParameter("@Email", userInfo.Email);
                command.AddParameter("@Active", userInfo.Active);
                command.AddParameter("@RequirePasswordReset", userInfo.RequirePasswordReset);
                command.AddParameter("@IsAdmin", userInfo.IsAdmin);
                if (doInsert)
                {
                    command.AddParameter("@TimeCreated", DateTime.Now);
                    using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        if(reader.Read())
                            try { userInfo.UserId = Int32.Parse(reader.Reader[0].ToString()); }
                            catch { }
                    }
                }
                else
                {
                    command.AddParameter("@UserId", userInfo.UserId);
                    command.ExecuteNonQuery();
                }
            }

            sql = "";
            if (!doInsert)
                sql = @" delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_UsersToGroups] where [UserId] = @UserId; ";
            if (userInfo.UserGroups != null && userInfo.UserGroups.Length > 0)
            {
                var groupIds = (from r in userInfo.UserGroups where r.IsMember == true orderby r.GroupId select r.GroupId).ToList();
                foreach (int groupId in groupIds)
                    sql += String.Format(@" insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_UsersToGroups] ([UserId], [GroupId]) values (@UserId, {0});
                        ", groupId);
            }
            if(sql.Length > 0)
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@UserId", userInfo.UserId);
                    command.ExecuteNonQuery();
                }
            return userInfo.UserId;
        }
        //----------------------------------------------------------------------------------------------------
        public static int SaveGroup(GroupInfo groupInfo)
        {
            string sql = "";
            bool doInsert = (groupInfo.GroupId < 0);
            if (doInsert)
                sql = @"insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_UserGroups]
                        ([GroupName], [Active], [TimeCreated])
                        values
                        (@GroupName, @Active, @TimeCreated);
                        select scope_identity(); ";
            else
                sql = @"update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_UserGroups]
                        set [GroupName] = @GroupName,
                            [Active]    = @Active
                        where [GroupId] = @GroupId; ";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@GroupName", groupInfo.GroupName);
                command.AddParameter("@Active", groupInfo.Active);
                if (doInsert)
                {
                    command.AddParameter("@TimeCreated", DateTime.Now);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        if (reader.Read())
                            try { groupInfo.GroupId = Int32.Parse(reader.Reader[0].ToString()); }
                            catch { }
                    }
                }
                else
                {
                    command.AddParameter("@GroupId", groupInfo.GroupId);
                    command.ExecuteNonQuery();
                }
            }


            var all_CurrentRecs = new Dictionary<string, Permission>();
            var current_GroupInfo = GetGroup(groupInfo.GroupId);
            if (current_GroupInfo.Permissions != null)
                foreach (var item1 in current_GroupInfo.Permissions) // Connections
                {
                    string key1 = item1.Get_UniqueKey();
                    if (key1 != null && !all_CurrentRecs.ContainsKey(key1))
                        all_CurrentRecs.Add(key1, item1);
                    if (item1.SubPermissions != null)
                        foreach (var item2 in item1.SubPermissions) // Schemas
                        {
                            string key2 = item2.Get_UniqueKey();
                            if (key2 != null && !all_CurrentRecs.ContainsKey(key2))
                                all_CurrentRecs.Add(key2, item2);
                            if (item2.SubPermissions != null)
                                foreach (var item3 in item2.SubPermissions) // Tables
                                {
                                    string key3 = item3.Get_UniqueKey();
                                    if (key3 != null && !all_CurrentRecs.ContainsKey(key3))
                                        all_CurrentRecs.Add(key3, item3);
                                }
                        }
                }
            var tmpFiltered1 = new Dictionary<string, Permission>();
            foreach (string key in all_CurrentRecs.Keys)
            {
                var item = all_CurrentRecs[key];
                if (item.View || item.Edit || item.Insert || item.Delete)
                    tmpFiltered1.Add(key, item);
            }
            all_CurrentRecs = tmpFiltered1;

            var all_NewRecs = new Dictionary<string, Permission>();
            if (groupInfo.Permissions != null)
                foreach (var item1 in groupInfo.Permissions) // Connections
                {
                    string key1 = item1.Get_UniqueKey();
                    if (key1 != null && !all_NewRecs.ContainsKey(key1))
                        all_NewRecs.Add(key1, item1);
                    if (item1.SubPermissions != null)
                        foreach (var item2 in item1.SubPermissions) // Schemas
                        {
                            string key2 = item2.Get_UniqueKey();
                            if (key2 != null && !all_NewRecs.ContainsKey(key2))
                                all_NewRecs.Add(key2, item2);
                            if (item2.SubPermissions != null)
                                foreach (var item3 in item2.SubPermissions) // Tables
                                {
                                    string key3 = item3.Get_UniqueKey();
                                    if (key3 != null && !all_NewRecs.ContainsKey(key3))
                                        all_NewRecs.Add(key3, item3);
                                }
                        }
                }
            var tmpFiltered2 = new Dictionary<string, Permission>();
            foreach (string key in all_NewRecs.Keys)
            {
                var item = all_NewRecs[key];
                if (item.View || item.Edit || item.Insert || item.Delete)
                    tmpFiltered2.Add(key, item);
            }
            all_NewRecs = tmpFiltered2;


            var recs_ToDelete = new List<Permission>();
            var recs_ToInsert = new List<Permission>();
            var recs_ToUpdate = new List<Permission>();

            foreach (string key in all_CurrentRecs.Keys)
                if (!all_NewRecs.ContainsKey(key))
                    recs_ToDelete.Add(all_CurrentRecs[key]);

            foreach (string key in all_NewRecs.Keys)
                if (!all_CurrentRecs.ContainsKey(key))
                {
                    all_NewRecs[key].GroupId = groupInfo.GroupId;
                    recs_ToInsert.Add(all_NewRecs[key]);
                }
                else // check for update
                {
                    var currentRec = all_CurrentRecs[key];
                    var newRec = all_NewRecs[key];
                    if (!currentRec.IsEqual(newRec))
                        recs_ToUpdate.Add(newRec);
                }

            foreach (var item in recs_ToDelete)
            {
                sql = "delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions] where [PermissionId] = @PermissionId";
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@PermissionId", item.PermissionId);
                    command.ExecuteNonQuery();
                }
            }

            foreach (var item in recs_ToInsert)
            {
                sql = "";
                switch (item.PermissionType)
                {
                    case Permission.PermissionTypes.Connection:
                        sql = @"insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions] 
                        ([GroupId], [PermissionType], [ConnectionId], [View], [Edit], [Insert], [Delete]) values (@GroupId, @PermissionType, @ConnectionId, @View, @Edit, @Insert, @Delete) ";
                        break;
                    case Permission.PermissionTypes.Schema:
                        sql = @"insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions] 
                        ([GroupId], [PermissionType], [ConnectionId], [Schema], [View], [Edit], [Insert], [Delete]) values (@GroupId, @PermissionType, @ConnectionId, @Schema, @View, @Edit, @Insert, @Delete) ";
                        break;
                    case Permission.PermissionTypes.Table:
                        sql = @"insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions] 
                        ([GroupId], [PermissionType], [ConnectionId], [Schema], [TableId], [View], [Edit], [Insert], [Delete]) values (@GroupId, @PermissionType, @ConnectionId, @Schema, @TableId, @View, @Edit, @Insert, @Delete) ";
                        break;
                }
                SaveGroup__Helper(true, sql, item);
            }

            foreach (var item in recs_ToUpdate)
            {
                sql = "";
                switch (item.PermissionType)
                {
                    case Permission.PermissionTypes.Connection:
                        sql = @"update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions]
                        set [GroupId] = @GroupId, [PermissionType] = @PermissionType, [ConnectionId] = @ConnectionId, [View] = @View, [Edit] = @Edit, [Insert] = @Insert, [Delete] = @Delete
                        where [PermissionId] = @PermissionId ";
                        break;
                    case Permission.PermissionTypes.Schema:
                        sql = @"update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions]
                        set [GroupId] = @GroupId, [PermissionType] = @PermissionType, [ConnectionId] = @ConnectionId, [View] = @View, [Edit] = @Edit, [Insert] = @Insert, [Delete] = @Delete,
                            [Schema] = @Schema
                        where [PermissionId] = @PermissionId ";
                        break;
                    case Permission.PermissionTypes.Table:
                        sql = @"update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions]
                        set [GroupId] = @GroupId, [PermissionType] = @PermissionType, [ConnectionId] = @ConnectionId, [View] = @View, [Edit] = @Edit, [Insert] = @Insert, [Delete] = @Delete,
                            [Schema] = @Schema, 
                            [TableId] = @TableId
                        where [PermissionId] = @PermissionId ";
                        break;
                }
                SaveGroup__Helper(false, sql, item);
            }
            
            return groupInfo.GroupId;
        }
        //----------------------------------------------------------------------------------------------------
        private static void SaveGroup__Helper(bool doInsert, string sql, Permission permission)
        {
            string tmp_PermissionType = "N";
            int? tmp_TableId = null;
            switch (permission.PermissionType)
            {
                case Permission.PermissionTypes.Connection:
                    tmp_PermissionType = "C";
                    permission.Schema = null;
                    break;
                case Permission.PermissionTypes.Schema:
                    tmp_PermissionType = "S";
                    break;
                case Permission.PermissionTypes.Table:
                    tmp_PermissionType = "T";
                    tmp_TableId = permission.TableId;
                    break;
            }

            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@GroupId", permission.GroupId);
                command.AddParameter("@PermissionType", tmp_PermissionType);
                command.AddParameter("@ConnectionId", permission.ConnectionId);
                //command.AddParameter("@Schema", permission.Schema);
                //command.AddParameter("@TableId", tmp_TableId);
                command.AddParameter("@View", permission.View);
                command.AddParameter("@Edit", permission.Edit);
                command.AddParameter("@Insert", permission.Insert);
                command.AddParameter("@Delete", permission.Delete);
                if (!doInsert)
                    command.AddParameter("@PermissionId", permission.PermissionId);

                if (permission.PermissionType != Permission.PermissionTypes.Connection)
                    command.AddParameter("@Schema", permission.Schema);
                if (permission.PermissionType == Permission.PermissionTypes.Table)
                    command.AddParameter("@TableId", tmp_TableId);

                command.ExecuteNonQuery();
            }
        }

        //----------------------------------------------------------------------------------------------------
        public static void DeleteUser(int userId)
        {
            var user = GetUser(userId);
            if (user.IsAdmin)
                throw new Exception("This user is an admin and therefore cannot be deleted.");
            try
            {
                string sql = @"
                    delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_UsersToGroups] where [UserId] = @UserId;
                    delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users]  where [UserId] = @UserId;
                    ";
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { throw new Exception("User could not be deleted. \n\nMost likely this user's UserId is associated in another application table."); }
        }
        //----------------------------------------------------------------------------------------------------
        public static void DeleteGroup(int groupId)
        {
            try
            {
                string sql = @"
                    delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_UsersToGroups] where [GroupId] = @GroupId;
                    delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Permissions]  where [GroupId] = @GroupId;
                    delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_UserGroups]  where [GroupId] = @GroupId;
                    ";
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@GroupId", groupId);
                    command.ExecuteNonQuery();
                }
            }
            catch { throw new Exception("Group could not be deleted. \n\nMost likely this group's GroupId is associated in another application table."); }
        }
        //----------------------------------------------------------------------------------------------------
        public static void SavePassword(int userId, string newPassword)
        {
            ValidatePassword(newPassword);
            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(newPassword, BCrypt.Net.BCrypt.GenerateSalt(UsersLogic.WorkFactor));
                newPassword = "";
                string sql = @" update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users] set [Password] = @Password where [UserId] = @UserId ";
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@Password", hash);
                    command.AddParameter("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
            catch { throw new Exception("Could not save password."); }
        }
        //----------------------------------------------------------------------------------------------------
        public static void SavePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                string sql = String.Format("select * from [{0}].[ASPdb_Users] where [UserId] = @UserId", Config.SystemProperties.AppSchema);
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@UserId", userId);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        reader.Read();
                        if (!BCrypt.Net.BCrypt.Verify(oldPassword, reader.Get("Password", "")))
                            throw new Exception("");
                    }
                }
            }
            catch { throw new Exception("Incorrect Old Password"); }

            SavePassword(userId, newPassword);
        }
        
        
        //----------------------------------------------------------------------------------------------------
        public static void ValidatePassword(string password)
        {
            string message = null;

            if (password == null || password.Length < 8)
                message = "Password must have at least 8 characters.";

            if (Regex.Replace(password, @"[^0-9]", "").Length < 1)
                message = "Password must contain letters and numbers.";

            if (Regex.Replace(password, @"[^a-zA-Z]", "").Length < 1)
                message = "Password must contain letters and numbers.";

            if (message != null)
                throw new Exception(message);
        }
        //---------------------------------------------------------------------------------------------------- Used by Admin
        public static void SendPasswordLink(int userId, string email)
        {
            throw new Exception("Not Implemented");
        }
        //---------------------------------------------------------------------------------------------------- Used by User
        public static void SendPasswordLink(string emailOrUsername)
        {
            throw new Exception("Not Implemented");
        }

    }
}