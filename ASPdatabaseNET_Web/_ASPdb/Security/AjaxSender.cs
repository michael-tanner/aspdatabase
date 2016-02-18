using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdb.Security
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AjaxSender : jQueryContext
    {
        public JsEvent_BeforeAfter OnReady = new JsEvent_BeforeAfter();
        public bool IsReady;

        public string PublicKeyPem;
        public string AESIndex;
        private AESKeyInfo AESKey;


        //----------------------------------------------------------------------------------- Static Getter --
        public static AjaxSender GetObj()
        {
            AjaxSender rtn = null;
            try
            {
                eval("rtn = document.ASPdb_Security_AjaxSender;");
            }
            catch { }
            if(rtn == null)
            {
                rtn = new AjaxSender();
                eval("document.ASPdb_Security_AjaxSender = rtn;");
            }
            return rtn;
        }

        //------------------------------------------------------------------------------------- Constructor --
        public AjaxSender()
        {
            this.IsReady = false;
        }
        //----------------------------------------------------------------------------------------------------
        public AESKeyInfo Get_AESKey()
        {
            return this.AESKey;
        }
        //----------------------------------------------------------------------------------------------------
        public void Initialize()
        {
            ASPdatabaseNET.AjaxService.ASPdatabaseService
                .New(this, "GetSessionPublicKey_Return")
                .NoEncryption()
                .Authentication__GetSessionPublicKey();
        }
        //----------------------------------------------------------------------------------------------------
        public void GetSessionPublicKey_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (this.HandleError(ajaxResponse, "GetSessionPublicKey_Return"))
                return;
            var arr = ajaxResponse.ReturnObj.As<string[]>();

            this.AESIndex = arr[0];
            this.PublicKeyPem = arr[1];
            this.Create_and_Send_AESKey();
        }
        //----------------------------------------------------------------------------------------------------
        private void Create_and_Send_AESKey()
        {
            this.AESKey = AESLogic.CreateNewAES();
            JsString json = this.AESKey.ToJson();

            string base64 = "";
            string pem = this.PublicKeyPem;
            eval(@"
                var jsEncrypt = new JSEncrypt();
                jsEncrypt.setKey(pem);
                base64 = jsEncrypt.encrypt(json);
            ");

            ASPdatabaseNET.AjaxService.ASPdatabaseService
                .New(this, "SendAESKey_Return")
                .NoEncryption()
                .Authentication__SendAESKey(this.AESIndex, base64);
        }

        //----------------------------------------------------------------------------------------------------
        public void SendAESKey_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (this.HandleError(ajaxResponse, "SendAESKey_Return"))
                return;
            var response = ajaxResponse.ReturnObj.As<JsBoolean>();

            this.OnReady.Before.Fire();
            this.IsReady = true;
            
            ASPdatabaseNET.AjaxService.ASPdatabaseService
                .New(this, "SendAESTest_Return")
                .YesEncryption()
                .Authentication__SendAESTest("Hello Secure World!");
        }
        //----------------------------------------------------------------------------------------------------
        public void SendAESTest_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (this.HandleError(ajaxResponse, "SendAESTest_Return"))
                return;
            JsString response = ajaxResponse.ReturnObj.As<string>();

            var arr = response.split(" ... ");
            if (arr[0] == "Your message: Hello Secure World!")
                this.OnReady.After.Fire();
            else
                alert("Error in SendAESTest_Return()");
        }


        //----------------------------------------------------------------------------------------------------
        public bool HandleError(ASPdb.Ajax.AjaxResponse ajaxResponse, string methodName)
        {
            if (ajaxResponse.Error != null)
            {
                alert(@"An error occurred while exchanging security keys.
This is normal and is just a precaution to ensure security.

Please refresh your browser and the error should resolve.

Thank you!");
                console.log("------------ In SendAESKey_Return() ---------------------");
                console.log("Error      : " + ajaxResponse.Error.Message);
                console.log("StackTrace : " + ajaxResponse.Error.StackTrace);
                return true;
            }
            else
                return false;
        }
    }
}