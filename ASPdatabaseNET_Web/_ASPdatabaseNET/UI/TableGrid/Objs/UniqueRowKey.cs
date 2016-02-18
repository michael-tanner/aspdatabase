using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ASPdatabaseNET.UI.TableGrid.Objs
{
    //----------------------------------------------------------------------------------------------------////
    public class UniqueRowKey
    {
        public enum ActionTypes { Get, New, Clone };

        // Format: [t][99][new]
        //         [t][99][get][value1][value2][value3]

        /*  Type:
         *      t == Table
         *      a == AppView
         *      v == View
         */
        public GridRequest.TableTypes TableType = GridRequest.TableTypes.NotSet;
        public int Id = -1;
        public string[] Values;
        public ActionTypes ActionType = ActionTypes.Get;
        //public bool IsNew = false;
        public bool IsValid = true;

        //------------------------------------------------------------------------------------- Constructor --
        public UniqueRowKey()
        {
        }

        //----------------------------------------------------------------------------------------------------
        public string To_Base64Json()
        {
            string strType = "";
            switch(this.TableType)
            {
                case GridRequest.TableTypes.Table: strType = "t"; break;
                case GridRequest.TableTypes.View: strType = "v"; break;
            }
            if (this.Values == null || this.Values.Length < 1)
                this.ActionType = ActionTypes.New;

            var list = new List<string>();
            list.Add(strType);
            list.Add(this.Id.ToString());
            switch(this.ActionType)
            {
                case ActionTypes.New: list.Add("new"); break;
                case ActionTypes.Clone: list.Add("clone"); break;
                default: list.Add("get"); break;
            }

            if (this.ActionType != ActionTypes.New)
                foreach (var item in this.Values)
                    list.Add(item);

            string json = (new JavaScriptSerializer()).Serialize(list.ToArray());
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            string base64 = System.Convert.ToBase64String(bytes);
            //return HttpContext.Current.Server.HtmlEncode(base64);
            return base64.Replace("=", "%3D").Replace("+", "%2B").Replace("/", "%2F");
        }

        //------------------------------------------------------------------------------------------ Static --
        public static UniqueRowKey GetFrom_Base64Json(string base64)
        {
            var rtn = new UniqueRowKey();
            try
            {
                base64 = base64.Replace("%3D", "=").Replace("%2B", "+").Replace("%2F", "/");
                byte[] bytes = System.Convert.FromBase64String(base64);
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                var arr = (new JavaScriptSerializer()).Deserialize<string[]>(json);

                switch(arr[0].ToLower().Trim())
                {
                    case "t": rtn.TableType = GridRequest.TableTypes.Table; break;
                    case "b": rtn.TableType = GridRequest.TableTypes.View; break;
                }
                rtn.Id = Int32.Parse(arr[1]);

                string action = arr[2].ToLower();
                switch(action)
                {
                    case "new": rtn.ActionType = ActionTypes.New; break;
                    case "clone": rtn.ActionType = ActionTypes.Clone; break;
                    default: rtn.ActionType = ActionTypes.Get; break;
                }

                rtn.Values = new string[0];
                if (rtn.ActionType != ActionTypes.New)
                {
                    var values = new List<string>();
                    if (arr.Length < 4)
                        rtn.ActionType = ActionTypes.New;
                    else
                        for (int i = 3; i < arr.Length; i++)
                            values.Add(arr[i]);
                    rtn.Values = values.ToArray();
                }

            }
            catch { rtn.IsValid = false; }
            return rtn;
        }
    }
}