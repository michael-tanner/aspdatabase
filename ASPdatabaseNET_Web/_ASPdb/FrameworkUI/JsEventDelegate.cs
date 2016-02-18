using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;

namespace ASPdb.FrameworkUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class JsEventDelegate : jQueryContext
    {
        public object[] CallBack_Objects;
        public string[] CallBack_Methods;
        public int[] CallBack_NumbOfParams;

        public object PassThruObj1 = null;
        public object PassThruObj2 = null;
        public object PassThruObj3 = null;
        public object PassThruObj4 = null;
        public object PassThruObj5 = null;

        //------------------------------------------------------------------------------------- Constructor --
        public JsEventDelegate()
        {
            this.CallBack_Objects = Arr.GetNewGenericArray<object[]>(null);
            this.CallBack_Methods = Arr.GetNewGenericArray<string[]>(null);
            this.CallBack_NumbOfParams = Arr.GetNewGenericArray<int[]>(null);
        }
        //----------------------------------------------------------------------------------------------------
        public void AddHandler(object callBack_Object, string callBack_Method, int callBack_NumbOfParams)
        {
            if (this.CallBack_Objects == null)
                this.CallBack_Objects = new object[0];
            if (this.CallBack_Methods == null)
                this.CallBack_Methods = new string[0];
            if (this.CallBack_NumbOfParams == null)
                this.CallBack_NumbOfParams = new int[0];

            int i = Arr.Len(this.CallBack_Objects);
            this.CallBack_Objects[i] = callBack_Object;
            this.CallBack_Methods[i] = callBack_Method;
            this.CallBack_NumbOfParams[i] = callBack_NumbOfParams;
        }
        //----------------------------------------------------------------------------------------------------
        public void Fire()
        {
            this.Fire5(null, null, null, null, null);
        }
        //----------------------------------------------------------------------------------------------------
        public void Fire1(object obj1)
        {
            this.Fire5(obj1, null, null, null, null);
        }
        //----------------------------------------------------------------------------------------------------
        public void Fire2(object obj1, object obj2)
        {
            this.Fire5(obj1, obj2, null, null, null);
        }
        //----------------------------------------------------------------------------------------------------
        public void Fire3(object obj1, object obj2, object obj3)
        {
            this.Fire5(obj1, obj2, obj3, null, null);
        }
        //----------------------------------------------------------------------------------------------------
        public void Fire4(object obj1, object obj2, object obj3, object obj4)
        {
            this.Fire5(obj1, obj2, obj3, obj4, null);
        }
        //----------------------------------------------------------------------------------------------------
        public void Fire5(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            for (int i = 0; i < Arr.Len(this.CallBack_Objects); i++)
            {
                var callObj = this.CallBack_Objects[i];
                string callMethod = this.CallBack_Methods[i];
                int numbParams = this.CallBack_NumbOfParams[i];
                var o1 = obj1;
                var o2 = obj2;
                var o3 = obj3;
                var o4 = obj4;
                var o5 = obj5;

                if (o1 == null) o1 = this.PassThruObj1;
                if (o2 == null) o2 = this.PassThruObj2;
                if (o3 == null) o3 = this.PassThruObj3;
                if (o4 == null) o4 = this.PassThruObj4;
                if (o5 == null) o5 = this.PassThruObj5;

                string evalStr = "callObj." + callMethod + "();";
                switch (numbParams)
                {
                    case 1: evalStr = "callObj." + callMethod + "(o1);";
                        break;
                    case 2: evalStr = "callObj." + callMethod + "(o1, o2);";
                        break;
                    case 3: evalStr = "callObj." + callMethod + "(o1, o2, o3);";
                        break;
                    case 4: evalStr = "callObj." + callMethod + "(o1, o2, o3, o4);";
                        break;
                    case 5: evalStr = "callObj." + callMethod + "(o1, o2, o3, o4, o5);";
                        break;
                }
                eval(evalStr);
            }
        }

    }
}