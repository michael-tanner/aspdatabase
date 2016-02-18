using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.TableGrid.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class GridViewModel
    {
        public GridRequest.TableTypes TableType = GridRequest.TableTypes.NotSet;
        public int Id = -1;

        public int DisplayTopNRows = 50;

        public string[] AllColumnNames = new string[0];

        public int SelectionCount = 0;

        public FilterAndSort FilterAndSort = new FilterAndSort();
        //public ViewOptions_FilterField[] FilterFields;
        //public ViewOptions_SortField[] SortFields;

        public string LocalStorage_Key_ViewOptions
        {
            get
            {
                return "ASPdatabaseNET.UI.TableGrid.ViewOptions__" + this.TableType + "_" + this.Id;
            }
        }
    }
}