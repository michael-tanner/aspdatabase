using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using SharpKit;
using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using System.Reflection;

namespace ASPdb.Ajax
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AjaxServiceProxy<A> : jQueryContext where A : class, new()
    {
        protected bool IsClientCode = true;
        public string AjaxUrl = "";
        public AjaxHelper aj = new AjaxHelper();
        public AjaxRequest Ajax;

        //------------------------------------------------------------------------------------- constructor --
        public AjaxServiceProxy()
        {
            try // Client-Side Only
            {
                this.AjaxUrl = ASPdb.FrameworkUI.Cookies.ASPdb_AjaxGatewayUrl;
            }
            catch (Exception exc) {
                var msg = exc;

            }
            if (this.AjaxUrl == null || this.AjaxUrl == "")
                alert("In AjaxServiceProxy() -- Cookie not found ");
        }

        //----------------------------------------------------------------------------------------------------
        public void SetCallback(object callback_Object, string callback_Method, object callback_DataObj)
        {
            this.Ajax = AjaxRequest.New2(this.AjaxUrl, "", callback_Object, callback_Method);
            this.Ajax.ReturnInfo.DataObj = callback_DataObj;
        }

        
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void GatewayEntry_Run(Type childType)
        {
            this.GatewayEntry_Run(childType, false, null);
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void GatewayEntry_Run(Type childType, bool userNotLoggedIn)
        {
            this.GatewayEntry_Run(childType, userNotLoggedIn, null);
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void GatewayEntry_Run(Type childType, bool userNotLoggedIn, string[] publicMethods)
        {
            this.IsClientCode = false;
            AjaxRequest request = null;

            string aesIndex = AjaxRequest.GetFromHttp_AESIndex();
            if(aesIndex == null)
                request = AjaxRequest.GetFromHttpRequest();
            else
                request = AjaxRequest.GetFromHttpRequest_AndDecrypt();
            
            var response = new AjaxResponse();
            try
            {
                if (publicMethods == null)
                    publicMethods = new string[0];
                if (!publicMethods.Contains(request.RemoteMethod))
                    if (userNotLoggedIn)
                        throw new Exception("User Session Expired");

                this.aj.Parameters = request.Parameters;
                var method = childType.GetMethod(request.RemoteMethod); // reflection
                if (method != null)
                {
                    var paremetersList = new List<object>();
                    foreach (var reflectionParam in method.GetParameters())
                    {
                        switch (reflectionParam.ParameterType.Name)
                        {
                            case "Int32":
                                paremetersList.Add(aj._Int32);
                                break;
                            case "Boolean":
                                paremetersList.Add(aj._Boolean);
                                break;
                            case "DateTime":
                                paremetersList.Add(aj._DateTime);
                                break;
                            case "String":
                                paremetersList.Add(aj._String);
                                break;
                            default:
                                //paremetersList.Add(aj._Json2(reflectionParam.ParameterType.FullName));

                                string fullTypeName = reflectionParam.ParameterType.FullName;
                                var obj = aj._Json2(fullTypeName);
                                paremetersList.Add(obj);
                                break;
                        }
                    }
                    object[] paramArr = paremetersList.ToArray();
                    if (paramArr.Length < 1)
                        paramArr = null;
                    response.ReturnObj = method.Invoke(this, paramArr);
                }
            }
            catch (Exception exc)
            {
                if (exc.InnerException != null)
                    exc = exc.InnerException;

                if (exc.Message == "User Session Expired")
                {
                    response.SetException(exc);
                    response.DoLogout = true;
                }
                else
                {
                    ASPdb.Framework.Debug.RecordException(exc);
                    response.SetException(exc);
                }
            }

            if(aesIndex == null)
                response.Send();
            else
                response.Send_AES(aesIndex);
        }


        //----------------------------------------------------------------------------------------------------
        public A Bind()
        {
            A rtn = null;
            var thisObj = this;
            eval("rtn = thisObj");

            
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public A IgnoreError()
        {
            A rtn = null;
            var thisObj = this;
            eval("rtn = thisObj");


            return rtn;
        }
    }
}