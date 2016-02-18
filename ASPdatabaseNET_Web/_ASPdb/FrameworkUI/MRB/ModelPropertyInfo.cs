using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdb.FrameworkUI.MRB
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ModelPropertyInfo
    {
        public enum DataTypes { String, Int, Bool, DateTime };

        public string PropertyName = "";
        public DataTypes DataType = DataTypes.String;
        public bool UseGetterSetter = false;

        //----------------------------------------------------------------------------------------------------
        public static ModelPropertyInfo New1(string propertyName, DataTypes dataType, bool useGetterSetter)
        {
            var rtn = new ModelPropertyInfo();
            rtn.PropertyName = propertyName;
            rtn.DataType = dataType;
            rtn.UseGetterSetter = useGetterSetter;
            return rtn;
        }
    }
}