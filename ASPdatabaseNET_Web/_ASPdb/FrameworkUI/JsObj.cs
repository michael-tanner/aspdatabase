using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdb.FrameworkUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class JsObj : jQueryContext
    {
        public object Object;

        //------------------------------------------------------------------------------------- constructor --
        public JsObj()
        {
        }

        //------------------------------------------------------------------------------------------ static --
        public static JsObj O(object obj)
        {
            var rtn = new JsObj();
            rtn.Object = obj;
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public object CloneInto(object objToCloneInto)
        {
            var o2 = JsObj.O(objToCloneInto);
            var propNames = this.PropertyNames();
            for (int i = 0; i < propNames.Length; i++)
            {
                string propName = propNames[i];
                o2.Property_Set(propName, this.Property_Get(propName, false), false);
            }
            return objToCloneInto;
        }

        //----------------------------------------------------------------------------------------------------
        public string[] PropertyNames()
        {
            var rtn = new string[0];
            var obj = this.Object;
            int i = 0;
            try
            {
                eval("for(var propName in obj) { rtn[i++] = propName; }");
            }
            catch { }
            return rtn;
        }


        //----------------------------------------------------------------------------------------------------
        /// <summary>Return type is string, but .as&lt;type&gt;() can be used for other types.</summary>
        public string Property(string propertyName)
        {
            try
            {
                string rtn = "";
                var obj = this.Object;
                string propName = propertyName;
                eval("rtn = obj[propName];");
                return rtn;
            }
            catch { return null; }
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Returns default value if there was an error.</summary>
        public string Property2(string propertyName, string defaultValue)
        {
            string rtn = null;
            try
            {
                var obj = this.Object;
                string propName = propertyName;
                eval("rtn = obj[propName];");
            }
            catch { }

            if (rtn == null)
                rtn = defaultValue;

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Returns default value if there was an error.</summary>
        public string Property3(string propertyName, string defaultValue, bool useDefaultIfEmptyString)
        {
            string rtn = null;
            try
            {
                var obj = this.Object;
                string propName = propertyName;
                eval("rtn = obj[propName];");
            }
            catch { }

            if (rtn == null)
                rtn = defaultValue;
            else if (useDefaultIfEmptyString && rtn == "")
                rtn = defaultValue;

            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        /// <summary>Same as calling Property(), though the return type is object.  Using .as&lt;type&gt;() works for both.</summary>
        public object Property_Get(string propertyName, bool useGetterMethod)
        {
            object rtn = null;
            try
            {
                var obj = this.Object;
                string propName = propertyName;

                if (useGetterMethod)
                    eval("rtn = obj.get_" + propName + "();");
                else
                    eval("rtn = obj[propName];");
            }
            catch { rtn = null; }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Returns false if there was an error.</summary>
        public bool Property_Set(string propertyName, object value, bool useSetterMethod)
        {
            try
            {
                var obj = this.Object;
                string propName = propertyName;
                object val = value;

                if (useSetterMethod)
                    eval("rtn = obj.set_" + propName + "(val);");
                else
                    eval("rtn = obj[propName] = val;");

                return true;
            }
            catch { return false; }
        }


    }
}