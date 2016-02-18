using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DataObjects.ManageAssets
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class BasicAssetInfo
    {
        public enum E_AssetTypes { NotSet, Table, AppView, SqlView, Schema };

        public int ConnectionId = -1;
        public E_AssetTypes AssetType = E_AssetTypes.NotSet;
        public int GenericId = -1;
        public string Schema = "";
        public string GenericName = "";
        public bool Active = true;

        public bool UseSquareBrackets_Schema = false;
        public bool UseSquareBrackets_GenericName = false;

        //------------------------------------------------------------------------------------- constructor --
        public BasicAssetInfo()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
        }
        //------------------------------------------------------------------------------------- constructor --
        public BasicAssetInfo(bool active, E_AssetTypes assetType)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            this.Active = active;
            this.AssetType = assetType;
        }
    }
}