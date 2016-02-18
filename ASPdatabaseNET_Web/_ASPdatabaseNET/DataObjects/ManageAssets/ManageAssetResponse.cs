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
    public class ManageAssetResponse
    {
        public enum ResponseTypesEnum { NotSet, GetAssetsLists };


        public ResponseTypesEnum ResponseType = ResponseTypesEnum.NotSet;

        public AssetsLists AssetsListsInfo;

    }
}