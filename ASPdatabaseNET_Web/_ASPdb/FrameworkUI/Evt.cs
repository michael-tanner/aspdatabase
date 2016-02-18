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
    public class Evt : jQueryContext
    {
        //----------------------------------------------------------------------------------------------------
        public static void Attach_Click(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("click", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_Click2(object theControlObj, string className, object passThruDataObj)
        {
            string methodToCall = className + "_Click";
            Evt.Attach("click", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_DblClick(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("dblclick", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_DoubleClick(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("doubleclick", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_KeyDown(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("keydown", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_KeyPress(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("keypress", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_KeyUp(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("keyup", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_GotFocus(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("gotfocus", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_LostFocus(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("lostfocus", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_Blur(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("blur", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_Change(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("change", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_Change2(object theControlObj, string className, object passThruDataObj)
        {
            string methodToCall = className + "_Change";
            Evt.Attach("change", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_MouseDown(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("mousedown", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_MouseEnter(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("mouseenter", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_MouseLeave(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("mouseleave", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_MouseMove(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("mousemove", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_MouseOver(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("mouseover", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_MouseOut(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("mouseout", theControlObj, className, methodToCall, passThruDataObj);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_MouseUp(object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            Evt.Attach("mouseup", theControlObj, className, methodToCall, passThruDataObj);
        }


        //----------------------------------------------------------------------------------------------------
        public static void Attach(string eventType, object theControlObj, string className, string methodToCall, object passThruDataObj)
        {
            className = "." + className;
            HtmlElement element = null;
            try
            {
                var tmp1 = theControlObj.As<Evt_TempClass_CastHelper>();
                if (tmp1.jRoot != null)
                {
                    element = tmp1.jRoot.find(className)[0];
                }
                else if (tmp1._jRoot != null)
                {
                    element = tmp1._jRoot.find(className)[0];
                }
                Attach_ToElement(eventType, theControlObj, element, methodToCall, passThruDataObj);
            }
            catch (Exception exc1)
            {
                alert("Error in Evt.Attach('" + eventType + "', ?, '" + className + "', '" + methodToCall + "', ?)  \n" + exc1);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Attach_ToElement(string eventType, object theControlObj, HtmlElement element, string methodToCall, object passThruDataObj)
        {
            try
            {
                var eventInfo = new EventInfo();
                eventInfo.EventType = eventType;
                eventInfo.ObjectToCallOn = theControlObj;
                eventInfo.MethodToCall = methodToCall;
                eventInfo.PassThruDataObj = passThruDataObj;

                eventInfo.ElementAttachedTo = element;

                string propName = "EventInfoObject_" + eventType;
                J(eventInfo.ElementAttachedTo).prop(propName, eventInfo.As<string>());

                EventListener functionToCall = null;
                eval("functionToCall = function(evt) { ASPdb.FrameworkUI.Evt.DynamicEvents_ForwardToHandler(evt, '" + eventInfo.EventType + "', false); }");
                bool isIE = false; eval("isIE = $.browser.msie");
                if (isIE)
                    eval("eventInfo.ElementAttachedTo.attachEvent('on" + eventInfo.EventType + "', functionToCall);");
                else
                    eventInfo.ElementAttachedTo.addEventListener(eventInfo.EventType, functionToCall, false);
            }
            catch (Exception exc1)
            {
                alert("Error in Evt.Attach_ToElement('" + eventType + "', ?, ?, '" + methodToCall + "', ?)  \n" + exc1);
            }
        }
        
        //----------------------------------------------------------------------------------------------------
        public static void Attach_NonJRootElement(HtmlElement element, string eventType, object objToCallOn, string methodToCall, object passThruDataObj)
        {
            try
            {
                bool isIE = false; eval("isIE = $.browser.msie");
                bool elementIsWindow = (element == window.As<HtmlElement>());

                var eventInfo = new EventInfo();
                eventInfo.ElementAttachedTo = element;
                eventInfo.EventType = eventType;
                eventInfo.ObjectToCallOn = objToCallOn;
                eventInfo.MethodToCall = methodToCall;
                eventInfo.PassThruDataObj = passThruDataObj;

                string propName = "EventInfoObject_" + eventType;
                J(eventInfo.ElementAttachedTo).prop(propName, eventInfo.As<string>());

                EventListener functionToCall = null;
                eval("functionToCall = function(evt) { ASPdb.FrameworkUI.Evt.DynamicEvents_ForwardToHandler(evt, '" + eventInfo.EventType + "', " + elementIsWindow + "); }");
                if (isIE)
                    eval("eventInfo.ElementAttachedTo.attachEvent('on" + eventInfo.EventType + "', functionToCall);");
                else
                    eventInfo.ElementAttachedTo.addEventListener(eventInfo.EventType, functionToCall, false);
            }
            catch (Exception exc1)
            {
                alert("Error in Evt.Attach_NonJRootElement(?, '" + eventType + "', ?, '" + methodToCall + "', ?)  \n" + exc1);
            }
        }


        //----------------------------------------------------------------------------------------------------
        public static void DynamicEvents_ForwardToHandler(Event evt, string eventType, bool elementIsWindow)
        {
            try
            {
                bool isIE = false; eval("isIE = $.browser.msie");
                HtmlElement element = null;
                if (elementIsWindow)
                    element = window.As<HtmlElement>();
                if (element == null)
                {
                    if (isIE)
                        element = J(evt).prop("srcElement").As<HtmlElement>();
                    else
                        element = J(evt).prop("currentTarget").As<HtmlElement>();
                    //element = J(evt).prop("target").As<HtmlElement>(); // changed 4/6/12
                }

                string propertyName = "EventInfoObject_" + eventType;
                var eventInfo = J(element).prop(propertyName).As<EventInfo>();

                if (eventInfo != null)
                {
                    //if (isIE)
                    //    eventInfo.EventObj = Evt_TempClass_CastHelper.Get_IE_Event(evt);
                    //else
                        eventInfo.EventObj = evt;

                    if (eventInfo.ObjectToCallOn == null)
                        eval(eventInfo.MethodToCall + "(eventInfo);");
                    else
                        eval("eventInfo.ObjectToCallOn." + eventInfo.MethodToCall + "(eventInfo);");
                }
                else
                {
                    alert("Error in DynamicEvents_ForwardToHandler(evt, '" + eventType + "') \n" + "EventInfo is null.");
                }
            }
            catch (Exception exc1)
            {
                alert("Error in DynamicEvents_ForwardToHandler(evt, '" + eventType + "') \n" + exc1);
            }
        }


    }


    //----------------------------------------------------------------------------------------------------
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Evt_TempClass_CastHelper : jQueryContext
    {
        public jQuery jRoot = null;
        public jQuery _jRoot = null;

        public int clientX = -1;
        public int clientY = -1;

        public static Event Get_IE_Event(object evt)
        {
            Event rtn = null;
            eval("rtn = evt.event;");
            return rtn;
        }
    }
}