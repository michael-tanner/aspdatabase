using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI.MRB;

namespace ASPdb.FrameworkUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class MRBPattern<M, VM> : jQueryContext, IMRBPattern<M, VM>
    {
        //------------------------------
        protected ModelBindingHelper<M, VM> ModelBindingHelper;

        //------------------------------
        public M _model;
        public M _previousModel;
        [JsProperty(NativeProperty = true)]
        public M Model
        {
            get
            {
                this.OnModel_Get();
                return _model;
            }
            set
            {
                _previousModel = _model;
                _model = value;
                this.OnModel_Set();
            }
        }
        private void OnModel_Get() { }
        private void OnModel_Set() { }
        //------------------------------
        public VM _viewModel;
        [JsProperty(NativeProperty = true)]
        public VM ViewModel
        {
            get
            {
                this.OnViewModel_Get();
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                this.OnViewModel_Set();
            }
        }
        private void OnViewModel_Get() { }
        private void OnViewModel_Set() { }


        //------------------------------
        public jQuery jRoot { get; set; }
        public jQuery Get_jRoot() { return this.jRoot; }
        //------------------------------
        public bool IsInstantiated = false;
        public bool IsOpen = false;
        public bool Get_IsInstantiated() { return this.IsInstantiated; }
        public bool Get_IsOpen() { return this.IsOpen; }
        //------------------------------
        public JsEvent_BeforeAfter OnInstantiation = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnOpen = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnChange = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnClose = new JsEvent_BeforeAfter();


        //------------------------------------------------------------------------------------- constructor --
        public MRBPattern()
        {
            this.ModelBindingHelper = new ModelBindingHelper<M, VM>(this);
        }

        //----------------------------------------------------------------------------------------------------
        public M SetModel(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            this.Model = ajaxResponse.ReturnObj.As<M>();
            return this.Model;
        }

        //----------------------------------------------------------------------------------------------------
        public M GetModelWithoutFiringEvents()
        {
            return this._model;
        }
        //----------------------------------------------------------------------------------------------------
        public jQuery jF(string selector)
        {
            try
            {
                return jRoot.find(selector);
            }
            catch
            {
                return null;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public jQuery jF2(string selector)
        {
            // Excludes children under .jRoot ... This method needs to be researched & optimized more.
            try
            {
                var list_ToExclude = this.jF(".jRoot").find(selector);
                var list_AllItems = this.jF(selector);

                int k = 0;
                var list_Filtered = new HtmlElement[0];
                for (int i = 0; i < list_AllItems.length; i++)
                {
                    bool keepItem = true;
                    for (int j = 0; j < list_ToExclude.length; j++)
                    {
                        if (list_ToExclude[j] == list_AllItems[i])
                        {
                            keepItem = false;
                            j = list_ToExclude.length + 1;
                        }
                    }
                    if (keepItem)
                        list_Filtered[k++] = list_AllItems[i];
                }
                return J(list_Filtered);
            }
            catch
            {
                return null;
            }
        }


        //----------------------------------------------------------------------------------------------------
        public void Instantiate()
        {
            this.OnInstantiation.Before.Fire();
            this.Instantiate_Sub();
            this.ConnectEvents();
            this.IsInstantiated = true;
            this.OnInstantiation.After.Fire();
        }
        //--------------------------------------------------
        private void Instantiate_Sub()
        {
            //jRoot = J("<div class='ObjectsClassName jRoot'>");
            //jRoot.append(this.GetHtmlRoot());
        }

        //----------------------------------------------------------------------------------------------------
        public void ConnectEvents()
        {
            ModelBinding_EventsHelper<M, VM>.BindEvents(this);

            this.ConnectEvents_Sub();
        }
        //--------------------------------------------------
        private void ConnectEvents_Sub()
        {
        }



        //----------------------------------------------------------------------------------------------------
        public void BindUI()
        {
            if (ModelBindingHelper == null)
                return;
            this.ModelBindingHelper.Bind_ModelToUI(this, "*");
        }
        //----------------------------------------------------------------------------------------------------
        public void BindUI_Single(string modelKey)
        {
            if (ModelBindingHelper == null)
                return;
            this.ModelBindingHelper.Bind_ModelToUI(this, modelKey);
        }



        //----------------------------------------------------------------------------------------------------
        public void Open()
        {
            this.OnOpen.Before.Fire();
            jRoot.show();
            this.Open_Sub();
            this.IsOpen = true;
            this.OnOpen.After.Fire();
        }
        //--------------------------------------------------
        private void Open_Sub()
        {
        }


        //----------------------------------------------------------------------------------------------------
        public void Close()
        {
            this.OnClose.Before.Fire();
            jRoot.hide();
            this.Close_Sub();
            this.IsOpen = false;
            this.OnClose.After.Fire();
        }
        //--------------------------------------------------
        private void Close_Sub()
        {
        }



        //-------------------------------------------------------------------------------------- CSS & HTML --
        public static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static string GetCssRoot()
        {
            return "";
        }
        //----------------------------------------------------------------------------------------------------
        public string GetHtmlRoot()
        {
            return this.GetHtml_FromTemplate();
        }
        //----------------------------------------------------------------------------------------------------
        public string GetTemplate()
        {
            return "";
        }




        //----------------------------------------------------------------------------------------------------
        private string GetHtml_FromTemplate()
        {
            string rtn = "";
            var arr = st.New(this.GetTemplate()).Split("<html>", true);
            if (arr.Length < 1) return "";
            
            var lines = arr[1].Split("\n", false);
            ElementHolder tree = null;
            ElementHolder previousElement = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("-", false))
                {
                    var element = new ElementHolder();
                    element.Parse(line);

                    if (tree == null)
                    {
                        tree = element;
                        tree.IsRoot = true;
                        tree.ClassesString = st.New(tree.ClassesString + " jRoot").Trim().TheString;
                    }
                    else if (element.TreeLevel < previousElement.TreeLevel)
                    {
                        int backTrack = previousElement.TreeLevel - element.TreeLevel;
                        var tmpParent = previousElement;
                        if (backTrack <= 50)
                            for (int b = 0; b < backTrack; b++)
                            {
                                if (tmpParent.Parent != null)
                                    tmpParent = tmpParent.Parent;
                            }
                        if (tmpParent != null)
                            tmpParent.AppendSibling(element);
                    }
                    else if (element.TreeLevel == previousElement.TreeLevel)
                    {
                        previousElement.AppendSibling(element);
                    }
                    else if (element.TreeLevel > previousElement.TreeLevel)
                    {
                        previousElement.AppendChild(element);
                    }
                    previousElement = element;
                }
            }
            rtn += tree.ToHtmlString();


            if (1 == 0)
                return "<div style='font-size: .65em; line-height: 1em;'>" + st.New(rtn).Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br />").TheString + "</div>";
            else
                return rtn;
        }

    }

    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ElementHolder
    {
        public enum ElementTypes { NotSet, HtmlLiteral, EmptyElementTag, HtmlHolder, ElementsHolder };

        public st MarkdownLine;
        public ElementTypes ElementType = ElementTypes.NotSet;
        public bool IsRoot = false;

        public string ElementName = "";
        public string ModelKey = "";
        public string IdClass = "";
        public string ClassesString = "";
        public string Value = "";
        public string InnerHtml = "";
        public string InputType = "";

        public string[] EventNames;
        public string AttributesStr = "";

        public int TreeLevel = -1;
        public ElementHolder Parent;
        public ElementHolder[] Children;

        
        //----------------------------------------------------------------------------------------------------
        public void Parse(st line)
        {
            this.MarkdownLine = line;
            this.TreeLevel = this.CountHyphens(line);
            line = line.TruncateLeft(this.TreeLevel).Trim();

            if(line.StartsWith("'", false))
            {
                this.ElementName = "";
                this.ElementType = ElementTypes.HtmlLiteral;
                line = line.TruncateLeft(1);
                if (line.EndsWith("'", false))
                    line = line.TruncateRight(1);
                this.InnerHtml = line.TheString;
            }
            else
            {
                this.ElementType = ElementTypes.ElementsHolder;
                if(line.Contains("html=", false))
                {
                    string tmpLine = "";
                    bool insideSingleQuote = false;
                    for (int i = 0; i < line.Length(); i++)
                    {
                        string c = line.Substring(i, 1);
                        if (c == "'") insideSingleQuote = !insideSingleQuote;
                        if (c == " " && insideSingleQuote) tmpLine += "(---[[[SPACE].].]---)"; else tmpLine += c;
                    }
                    line.TheString = tmpLine;
                }
                var tokens = line.Split(" ", false);
                if (tokens[0].StartsWith("i", true))
                {
                    this.ElementType = ElementTypes.EmptyElementTag;
                    this.ElementName = "input";
                    this.InputType = "text";
                    var inArr = tokens[0].Split(":", false);
                    if (inArr.Length > 1)
                        switch(inArr[1].TheString)
                        {
                            case "": this.InputType = "text"; break;
                            case "t": this.InputType = "text"; break;
                            case "b": this.InputType = "button"; break;
                            case "s": this.InputType = "submit"; break;
                            case "c": this.InputType = "checkbox"; break;
                            case "r": this.InputType = "radio"; break;
                            case "f": this.InputType = "file"; break;
                            case "h": this.InputType = "hidden"; break;
                            default: this.InputType = inArr[1].TheString; break;
                        }
                }
                else
                    switch (tokens[0].TheString)
                    {
                        case "d": this.ElementName = "div";
                            break;
                        case "s": this.ElementName = "span";
                            break;
                        case "br":
                            this.ElementName = "br";
                            this.ElementType = ElementTypes.EmptyElementTag;
                            break;
                        case "t": this.ElementName = "table";
                            break;
                        case "tr": this.ElementName = "tr";
                            break;
                        case "td": this.ElementName = "td";
                            break;
                        default: this.ElementName = tokens[0].TheString;
                            break;
                    }

                this.ClassesString = "";
                for (int i = 1; i < tokens.Length; i++)
                {
                    var token = tokens[i].Trim();
                    token = token.Replace("(---[[[SPACE].].]---)", " ");
                    if(token.Length() > 0)
                    {
                        if(token.StartsWith("#", false))
                        {
                            this.IdClass = token.TruncateLeft(1).Trim().TheString;
                        }
                        else if(token.StartsWith(".", true))
                        {
                            this.ClassesString += " " + token.TruncateLeft(1).Replace(".", " ").TheString;
                        }
                        else if(token.StartsWith("evt.", true))
                        {
                            if (this.EventNames == null)
                                this.EventNames = new string[0];
                            this.EventNames[this.EventNames.Length] = token.TruncateLeft(4).Trim().TheString;
                        }
                        else if(token.StartsWith("m=", true))
                        {
                            var modelKey = token.TruncateLeft(2).Trim();
                            if (modelKey.StartsWith("'", false)) modelKey = modelKey.TruncateLeft(1);
                            if (modelKey.EndsWith("'", false)) modelKey = modelKey.TruncateRight(1);
                            this.ModelKey = modelKey.TheString;
                        }
                        else if (token.StartsWith("html=", true))
                        {
                            var html = token.TruncateLeft(5).Trim();
                            if (html.StartsWith("'", false)) html = html.TruncateLeft(1);
                            if (html.EndsWith("'", false)) html = html.TruncateRight(1);
                            this.InnerHtml = html.TheString;
                        }
                        else if (token.StartsWith("v=", true))
                        {
                            var value = token.TruncateLeft(2).Trim();
                            if (value.StartsWith("'", false)) value = value.TruncateLeft(1);
                            if (value.EndsWith("'", false)) value = value.TruncateRight(1);
                            this.Value = value.TheString;
                        }
                        else if(token.Contains("=", false))
                        {
                            this.AttributesStr += " " + token.TheString + " ";
                        }
                    }
                }
                if(this.IdClass.Length > 0)
                    this.ClassesString += " " + this.IdClass;
                this.ClassesString = st.New(this.ClassesString).Trim().TheString;

                if (this.EventNames != null && this.IdClass.Length > 0)
                    for (int i = 0; i < this.EventNames.Length; i++)
                        this.AttributesStr += st.New(" On_{0}='{1}_{0}'").Format2(this.EventNames[0], this.IdClass).TheString;
            }
        }
        //----------------------------------------------------------------------------------------------------
        private int CountHyphens(st line)
        {
            int max = 50;
            if (line.Length() < max)
                max = line.Length() - 1;
            for (int i = 0; i < max; i++)
                if (line.Substring(i, 1) != "-")
                    return i;
            return -1;
        }


        //----------------------------------------------------------------------------------------------------
        public void AppendChild(ElementHolder element)
        {
            if (this.Children == null)
                this.Children = new ElementHolder[0];
            this.Children[this.Children.Length] = element;
            element.Parent = this;
        }
        //----------------------------------------------------------------------------------------------------
        public void AppendSibling(ElementHolder element)
        {
            if (this.Parent != null)
                this.Parent.AppendChild(element);
        }



        //----------------------------------------------------------------------------------------------------
        public string ToHtmlString()
        {
            string rtn = "";

            if (this.ElementType == ElementTypes.ElementsHolder && (this.Children == null || this.Children.Length < 0))
                this.ElementType = ElementTypes.HtmlHolder;

            string prefix = "";
            for (int i = 1; i < this.TreeLevel; i++) prefix += "&nbsp;&nbsp;&nbsp;&nbsp;";
            string suffix = "\n";

            if(1 != 0) 
            { prefix = ""; suffix = ""; }


            st attributesStr = st.New("");
            if (this.InputType.Length > 0)
                attributesStr.AppendST(st.New(" type='{0}'").Format1(this.InputType));
            if (this.ModelKey.Length > 0)
                attributesStr.AppendST(st.New(" ModelKey='{0}'").Format1(this.ModelKey));
            if (this.IdClass.Length > 0)
                attributesStr.AppendST(st.New(" IdClass='{0}'").Format1(this.IdClass));
            if (this.ClassesString.Length > 0)
                attributesStr.AppendST(st.New(" class='{0}'").Format1(this.ClassesString));
            if (this.Value.Length > 0)
                attributesStr.AppendST(st.New(" value='{0}'").Format1(this.Value));
            
            attributesStr = attributesStr.Trim();
            if (this.AttributesStr != "")
                attributesStr.Append(" " + this.AttributesStr);
            if (attributesStr.Length() > 0) 
                attributesStr.TheString = " " + attributesStr.TheString;


            if (this.ElementType == ElementTypes.ElementsHolder)
            {
                rtn += st.New("{0}<{1}{2}>{3}").Format4(prefix, this.ElementName, attributesStr.TheString, suffix).TheString;
                for (int i = 0; i < this.Children.Length; i++)
                    rtn += this.Children[i].ToHtmlString();
                rtn += st.New("{0}</{1}>{2}").Format3(prefix, this.ElementName, suffix).TheString;
            }
            else if (this.ElementType == ElementTypes.HtmlHolder)
            {
                rtn += st.New("{0}<{1}{2}>{3}</{1}>{4}").Format5(prefix, this.ElementName, attributesStr.TheString, this.InnerHtml, suffix).TheString;
            }
            else if(this.ElementType == ElementTypes.EmptyElementTag)
            {
                rtn += st.New("{0}<{1}{2} />{3}").Format4(prefix, this.ElementName, attributesStr.TheString, suffix).TheString;
            }
            else if (this.ElementType == ElementTypes.HtmlLiteral)
            {
                rtn += st.New("{0}{1}{2}").Format3(prefix, this.InnerHtml, suffix).TheString;
            } 

            return rtn;
        }
    }
}