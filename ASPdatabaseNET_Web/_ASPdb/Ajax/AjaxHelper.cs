using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using SharpKit;
using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;

namespace ASPdb.Ajax
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AjaxHelper : HtmlContext
    {
        public bool IsClientCode;

        //------------------------------------------------------------------------------------- constructor --
        public AjaxHelper()
        {
            try
            {
                string tmp = typeof(System.Int32).ToString();
                this.IsClientCode = false;
            }
            catch { this.IsClientCode = true; }
        }


        //------------------------------------------------------------------------------------------ static --
        public static AjaxHelper New
        {
            get
            {
                return new AjaxHelper();
            }
        }

        //----------------------------------------------------------------------------------------------------
        public string ToJson(object obj)
        {
            //if (!isClientCode) { }
            try
            {
                return (new JavaScriptSerializer()).Serialize(obj);
            }
            catch { }

            // Client Code:
            string rtn = "";
            object o = obj;
            eval("rtn = $.toJSON(o);");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public T FromJson<T>(string json) where T : class
        {
            if (!this.IsClientCode)
            {
                try
                {
                    return (new JavaScriptSerializer()).Deserialize<T>(json);
                }
                catch (Exception exc) { ASPdb.Framework.Debug.RecordException(exc); return null; }
            }
            else
            {
                // Client Code:
                T rtn = null;
                eval("rtn = $.evalJSON(json);");
                return rtn;
            }
        }


        //----------------------------------------------------------------------------------------------------
        public object[] Parameters = new object[0];
        public int ParameterIndex = 0;
        //----------------------------------------------------------------------------------------------------
        public T _Json<T>() where T : class
        {
            T rtn = null;
            try
            {
                rtn = this.FromJson<T>(this.Parameters[this.ParameterIndex].ToString());
            }
            catch (Exception exc) { ASPdb.Framework.Debug.RecordException(exc); }
            this.ParameterIndex++;
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public object _Json2(string fullTypeName)
        {
            var method = typeof(AjaxHelper).GetMethod("_Json");
            var methodGeneric = method.MakeGenericMethod(Type.GetType(fullTypeName));
            object rtn = methodGeneric.Invoke(this, null);
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public int _Int32
        {
            get
            {
                int rtn = -1;
                try
                {
                    rtn = Int32.Parse(this.Parameters[this.ParameterIndex].ToString().Trim());
                }
                catch { }
                this.ParameterIndex++;
                return rtn;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public bool _Boolean
        {
            get
            {
                bool rtn = false;
                try
                {
                    rtn = Boolean.Parse(this.Parameters[this.ParameterIndex].ToString().Trim());
                }
                catch { }
                this.ParameterIndex++;
                return rtn;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public DateTime _DateTime
        {
            get
            {
                DateTime rtn = default(DateTime);
                try
                {
                    rtn = DateTime.Parse(this.Parameters[this.ParameterIndex].ToString().Trim());
                }
                catch { }
                this.ParameterIndex++;
                return rtn;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public string _String
        {
            get
            {
                string rtn = "";
                try
                {
                    rtn = this.Parameters[this.ParameterIndex].ToString();
                }
                catch { }
                this.ParameterIndex++;
                return rtn;
            }
        }

        //----------------------------------------------------------------------------------------------------
    }
}