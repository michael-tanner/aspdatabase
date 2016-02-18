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
    public class AssetsLists
    {
        public int ConnectionId;
        public string ConnectionName;

        public BasicAssetInfo[] Tables_Active;
        public BasicAssetInfo[] Tables_Hidden;

        public BasicAssetInfo[] AppViews_Active;
        public BasicAssetInfo[] AppViews_Hidden;

        public BasicAssetInfo[] SqlViews_Active;
        public BasicAssetInfo[] SqlViews_Hidden;

        public BasicAssetInfo[] Schemas;
    }

    //----------------------------------------------------------------------------------------------------////
    public class AssetsLists_PopulatorHelper
    {
        public int ConnectionId;
        public string ConnectionName;

        public List<BasicAssetInfo> Tables_Active;
        public List<BasicAssetInfo> Tables_Hidden;

        public List<BasicAssetInfo> SqlViews_Active;
        public List<BasicAssetInfo> SqlViews_Hidden;

        public List<BasicAssetInfo> Schemas;

        //------------------------------------------------------------------------------------- constructor --
        public AssetsLists_PopulatorHelper(int connectionId, bool createLists)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            this.ConnectionId = connectionId;
            if (createLists)
            {
                this.Tables_Active = new List<BasicAssetInfo>();
                this.Tables_Hidden = new List<BasicAssetInfo>();
                this.SqlViews_Active = new List<BasicAssetInfo>();
                this.SqlViews_Hidden = new List<BasicAssetInfo>();
                this.Schemas = new List<BasicAssetInfo>();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public AssetsLists ToAssetsLists()
        {
            return new AssetsLists()
            {
                ConnectionId = this.ConnectionId,
                ConnectionName = this.ConnectionName,
                Tables_Active = this.Tables_Active.ToArray(),
                Tables_Hidden = this.Tables_Hidden.ToArray(),
                SqlViews_Active = this.SqlViews_Active.ToArray(),
                SqlViews_Hidden = this.SqlViews_Hidden.ToArray(),
                Schemas = this.Schemas.ToArray()
            };
        }
    }
}