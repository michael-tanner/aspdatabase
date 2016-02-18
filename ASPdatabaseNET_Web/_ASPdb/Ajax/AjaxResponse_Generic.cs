using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit;
using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using System.Web.Script.Serialization;
using ASPdb.FrameworkUI;

namespace ASPdb.Ajax
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AjaxResponse_Generic<T> : HtmlContext
    {
        public T ReturnObj;
        public string ReturnType;

        public ASPdb.FrameworkUI.ErrorInfo Error;
        public AjaxRequest AjaxRequest;

        //------------------------------------------------------------------------------------- Constructor --
        public AjaxResponse_Generic()
        {
        }
        //----------------------------------------------------------------------------------------------------
        //public string ToJson()
        //{
        //    return (new AjaxHelper()).ToJson(this);
        //}
        ////----------------------------------------------------------------------------------------------------
        //public static AjaxResponse_JsonString GetFromJson(string json)
        //{
        //    var obj = (new AjaxHelper()).FromJson<AjaxResponse_JsonString>(json);
        //    string returnType = "";
        //    eval("returnType = obj.ReturnType;");

        //    var rtn = new AjaxResponse_JsonString();
        //    rtn.ReturnObj = obj.ReturnObj;
        //    rtn.ReturnType = returnType;
        //    rtn.Error = obj.Error;
        //    rtn.AjaxRequest = obj.AjaxRequest;

        //    return rtn;
        //}

        ////----------------------------------------------------------------------------------------------------
        //public void Send()
        //{
        //    HttpContext.Current.Response.Write(this.ToJson());
        //}
        ////----------------------------------------------------------------------------------------------------
        //public void Send_AES(string aesIndex)
        //{
        //    string cipherText = ASPdatabaseNET.Users.UserSessionLogic.EncryptAES(aesIndex, this.ToJson());
        //    HttpContext.Current.Response.Write(cipherText);
        //}



        ////----------------------------------------------------------------------------------------------------
        //public void SetException(Exception exc)
        //{
        //    this.Error = new ErrorInfo()
        //    {
        //        Message = exc.Message,
        //        StackTrace = exc.StackTrace
        //    };
        //}
    }
}