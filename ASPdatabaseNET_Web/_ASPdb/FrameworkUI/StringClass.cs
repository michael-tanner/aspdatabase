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
    public class StringClass : jQueryContext
    {
        private string _stringValue = "";
        public string StringValue
        {
            get
            {
                return this._stringValue;
            }
            set
            {
                this._stringValue = value;
            }
        }

        //------------------------------------------------------------------------------------- Constructor --
        public StringClass(string value)
        {
            this.StringValue = value;
        }

        //----------------------------------------------------------------------------------------------------
        public bool Contains(string value, bool ignoreCase)
        {
            return StringStatic.Contains(this.StringValue, value, ignoreCase);
        }

        //----------------------------------------------------------------------------------------------------
        public int RemoveNonNumericChars(int defaultIfError)
        {
            return StringStatic.RemoveNonNumericChars(this.StringValue, defaultIfError);
        }

        //----------------------------------------------------------------------------------------------------
        public bool IsNullOrEmpty()
        {
            return StringStatic.IsNullOrEmpty(this.StringValue);
        }
        //----------------------------------------------------------------------------------------------------
        public bool IsNullOrWhiteSpace()
        {
            return StringStatic.IsNullOrWhiteSpace(this.StringValue);
        }

    }
}