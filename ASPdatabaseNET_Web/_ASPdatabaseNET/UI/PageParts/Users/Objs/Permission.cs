using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.Users.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Permission
    {
        public enum PermissionTypes { NotSet, Connection, Schema, Table };

        public PermissionTypes PermissionType = PermissionTypes.NotSet;

        public int PermissionId;
        public int GroupId;
        public int ConnectionId;
        public string ConnectionName;
        public string Schema;
        public int TableId;
        public string TableName;

        public bool View = false;
        public bool Edit = false;
        public bool Insert = false;
        public bool Delete = false;

        public Permission[] SubPermissions;

        //------------------------------------------------------------------------------------- Constructor --
        public Permission()
        {

        }

        //------------------------------------------------------------------------------------- Constructor --
        public Permission(PermissionTypes permissionType, int permissionId, int groupId)
        {
            this.PermissionType = permissionType;
            this.PermissionId = permissionId;
            this.GroupId = groupId;
        }

        //----------------------------------------------------------------------------------------------------
        public void Set_PermissionType(string permissionType_SingleCharacter)
        {
            this.PermissionType = PermissionTypes.NotSet;
            switch (permissionType_SingleCharacter)
            {
                case "C": this.PermissionType = PermissionTypes.Connection; break;
                case "S": this.PermissionType = PermissionTypes.Schema; break;
                case "T": this.PermissionType = PermissionTypes.Table; break;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public string Get_UniqueKey()
        {
            string rtn = null;
            switch (this.PermissionType)
            {
                case PermissionTypes.Connection: rtn = "C_" + this.ConnectionId; break;
                case PermissionTypes.Schema: rtn = "S_" + this.ConnectionId + "_[" + this.Schema + "]"; break;
                case PermissionTypes.Table: rtn = "T_" + this.ConnectionId + "_[" + this.Schema + "]_[" + this.TableId + "]"; break;
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public bool IsEqual(Permission permission)
        {
            //if (this.PermissionId != permission.PermissionId) return false;
            if (this.PermissionType != permission.PermissionType) return false;
            if (this.GroupId != permission.GroupId) return false;
            if (this.ConnectionId != permission.ConnectionId) return false;
            if (this.Schema != permission.Schema) return false;
            if (this.TableId != permission.TableId) return false;
            if (this.View != permission.View) return false;
            if (this.Edit != permission.Edit) return false;
            if (this.Insert != permission.Insert) return false;
            if (this.Delete != permission.Delete) return false;
            return true;
        }
    }
}