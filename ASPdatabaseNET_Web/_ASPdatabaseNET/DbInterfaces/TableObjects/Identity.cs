using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.DbInterfaces.TableObjects
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Identity
    {
        public int ConnectionId;
        public string Schema;
        public string TableName;

        public string ColumnName;
        public int Seed;
        public int Increment;
        public int CurrentIdentity;

        //----------------------------------------------------------------------------------------------------
        public static Identity Clone(Identity identity)
        {
            var rtn = new Identity();
            rtn.ConnectionId = identity.ConnectionId;
            rtn.Schema = identity.Schema;
            rtn.TableName = identity.TableName;
            rtn.ColumnName = identity.ColumnName;
            rtn.Seed = identity.Seed;
            rtn.Increment = identity.Increment;
            rtn.CurrentIdentity = identity.CurrentIdentity;
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Validate()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
        }
    }
}