using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.GlobalParts.Objs;

namespace ASPdatabaseNET.UI.GlobalParts.Backend
{
    public class LogoBoxLogic
    {
        public static LogoBoxModel Get()
        {
            var rtn = new LogoBoxModel() { CustomLogoType = LogoBoxModel.CustomLogoTypes.None };
            try
            {
                string logoValue = Config.ASPdb_Values.GetFirstValue("Logo", "");
                var arr = logoValue.Split(new char[] { ',' });

                foreach(var item in arr)
                {
                    string item_L = item.Trim().ToLower();
                    if(item_L.Length > 0)
                    {
                        if (item_L.EndsWith(".jpg") || item_L.EndsWith(".png") || item_L.EndsWith(".gif"))
                            rtn.LogoURL = item.Trim();
                        else
                            rtn.LogoText = item.Trim();
                    }
                }
            }
            catch (Exception exc) { ASPdb.Framework.Debug.RecordException(exc); }

            if (!String.IsNullOrEmpty(rtn.LogoText))
                rtn.CustomLogoType = LogoBoxModel.CustomLogoTypes.Text;
            if (!String.IsNullOrEmpty(rtn.LogoURL))
                rtn.CustomLogoType = LogoBoxModel.CustomLogoTypes.Image;

            return rtn;
        }
    }
}