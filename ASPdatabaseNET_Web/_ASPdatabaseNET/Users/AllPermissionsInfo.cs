using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.Users
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    [Serializable()]
    public class AllPermissionsInfo
    {
        public int UserId;
        public bool IsAdmin;

        public Dictionary<string, PermissionValues> PermissionValuesDict;



        //----------------------------------------------------------------------------------------------------
        public PermissionValues CheckPermissions(int connectionId, string schema, int tableId)
        {
            if (this.IsAdmin)
                return new PermissionValues() { View = true, Edit = true, Insert = true, Delete = true };

            var rtn = new PermissionValues();
            if (this.PermissionValuesDict == null)
                return rtn;

            string key1 = "C_" + connectionId;
            string key2 = "S_" + connectionId + "_" + schema.ToLower();
            string key3 = "T_" + connectionId + "_" + schema.ToLower() + "_" + tableId;

            if (this.PermissionValuesDict.ContainsKey(key1))
                UpdateTrues_InPermissionValues(this.PermissionValuesDict[key1], rtn);
            if (this.PermissionValuesDict.ContainsKey(key2))
                UpdateTrues_InPermissionValues(this.PermissionValuesDict[key2], rtn);
            if (this.PermissionValuesDict.ContainsKey(key3))
                UpdateTrues_InPermissionValues(this.PermissionValuesDict[key3], rtn);

            if (!rtn.View)
                if (rtn.Edit || rtn.Insert || rtn.Delete)
                    rtn.View = true;

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private void UpdateTrues_InPermissionValues(PermissionValues source, PermissionValues target)
        {
            if (source.View) target.View = true;
            if (source.Edit) target.Edit = true;
            if (source.Insert) target.Insert = true;
            if (source.Delete) target.Delete = true;
        }
    }
}