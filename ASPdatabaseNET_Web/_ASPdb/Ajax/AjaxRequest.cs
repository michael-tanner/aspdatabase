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
    public class AjaxRequest : HtmlContext
    {
        public string AjaxUrl { get; set; }
        public string RemoteMethod { get; set; }
        public AjaxReturnInfo ReturnInfo { get; set; }
        public object[] Parameters { get; set; }
        public bool DoEncryption = false;

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public static AjaxRequest New_ServerSide(string remoteMethod, int parameterCount)
        {
            var rtn = new AjaxRequest();
            rtn.RemoteMethod = remoteMethod;
            rtn.Parameters = new object[parameterCount];
            rtn.AjaxUrl = ASPdatabaseNET.Config.SystemProperties.wwwASPdatabaseURL;
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static AjaxRequest New(string ajaxUrl, string remoteMethod, AjaxReturnInfo returnInfo)
        {
            var rtn = new AjaxRequest();
            rtn.AjaxUrl = ajaxUrl;
            rtn.RemoteMethod = remoteMethod;
            rtn.ReturnInfo = returnInfo;

            rtn.Parameters = new object[0];

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static AjaxRequest New2(string ajaxUrl, string remoteMethod, object callback_Object, string callback_Method)
        {
            var returnInfo = new AjaxReturnInfo();
            returnInfo.Callback_Object = callback_Object;
            returnInfo.Callback_Method = callback_Method;
            return New(ajaxUrl, remoteMethod, returnInfo);
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public static AjaxRequest GetFromHttpRequest()
        {
            string json = HttpContext.Current.Request.Form["AjaxRequest"];
            return (new AjaxHelper()).FromJson<AjaxRequest>(json);
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public static AjaxRequest GetFromHttpRequest_AndDecrypt()
        {
            string aesIndex = HttpContext.Current.Request.Form["AESIndex"];
            string cipherText = HttpContext.Current.Request.Form["AjaxRequest"];
            string json = ASPdatabaseNET.Users.UserSessionLogic.DecryptAES(aesIndex, cipherText);
            return (new AjaxHelper()).FromJson<AjaxRequest>(json);
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public static string GetFromHttp_AESIndex()
        {
            string rtn = null;
            try
            {
                rtn = HttpContext.Current.Request.Form["AESIndex"];
                if (rtn == "")
                    rtn = null;
            }
            catch { }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public Exception Send()
        {
            var ajaxRequest_This = this;
            var ajaxRequest_ToSend = new AjaxRequest();
            try
            {
                ajaxRequest_ToSend.AjaxUrl = this.AjaxUrl;
                ajaxRequest_ToSend.RemoteMethod = this.RemoteMethod;
                ajaxRequest_ToSend.Parameters = this.Parameters;

                var ajaxSender = ASPdb.Security.AjaxSender.GetObj();
                if (this.DoEncryption)
                {
                    if(!ajaxSender.IsReady) { alert("Required Ajax Encryption could not be established."); return null; }

                    string json = "";
                    eval("json = $.toJSON(ajaxRequest_ToSend);");
                    json = Security.AESLogic.EncryptClient(ajaxSender.Get_AESKey(), json);
                    var aesIndex = ajaxSender.AESIndex;
                    eval(@"
                        var jsonRequestMap = { AjaxRequest: json, AESIndex: aesIndex };
                        $.post(
                            this.AjaxUrl, 
                            jsonRequestMap, 
                            function (responseString) 
                            { 
                                ASPdb.Ajax.AjaxRequest.AjaxReturnMethod_AES(ajaxRequest_This, responseString, aesIndex); 
                            });
                        ");
                }
                else 
                {
                    eval(@"
                        var jsonRequestMap = { AjaxRequest: $.toJSON(ajaxRequest_ToSend) };
                        $.post(
                            this.AjaxUrl, 
                            jsonRequestMap, 
                            function (responseString) 
                            { 
                                ASPdb.Ajax.AjaxRequest.AjaxReturnMethod(ajaxRequest_This, responseString); 
                            });
                        ");
                }
                return null;
            }
            catch (Exception exc)
            {
                alert("Error in AjaxRequest.Send()");
                console.log("AjaxRequest.Send() -- Exception: " + exc);
                return exc;
            }
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public AjaxResponse_Generic<T> Send_ServerSide<T>()
        {
            string json = ASPdb.Framework.Http.Fetch_AjaxPost(this.AjaxUrl, this);

            var rtn = (new JavaScriptSerializer()).Deserialize<AjaxResponse_Generic<T>>(json);

            if (rtn.Error != null)
            {
                var exc = new Exception(rtn.Error.Message);
                ASPdb.Framework.Debug.RecordException(exc);
                ASPdb.Framework.Debug.WriteLine(rtn.Error.StackTrace);
                throw exc;
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public static void AjaxReturnMethod(AjaxRequest ajaxRequest, string ajaxResponseString)
        {
            if (ajaxRequest.ReturnInfo != null)
            {
                var ajaxResponse = AjaxResponse.GetFromJson(ajaxResponseString);
                ajaxResponse.AjaxRequest = ajaxRequest;

                if (ajaxRequest.ReturnInfo.Callback_Object != null)
                {
                    var objToCallOn = ajaxRequest.ReturnInfo.Callback_Object;
                    eval("objToCallOn." + ajaxRequest.ReturnInfo.Callback_Method + "(ajaxResponse);");
                }
                else
                {
                    eval(ajaxRequest.ReturnInfo.Callback_Method + "(ajaxResponse);");
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void AjaxReturnMethod_AES(AjaxRequest ajaxRequest, string ajaxResponseString, string aesIndex)
        {
            var ajaxSender = Security.AjaxSender.GetObj();
            ajaxResponseString = Security.AESLogic.DecryptClient(ajaxSender.Get_AESKey(), ajaxResponseString);
            AjaxReturnMethod(ajaxRequest, ajaxResponseString);
        }


    }
}