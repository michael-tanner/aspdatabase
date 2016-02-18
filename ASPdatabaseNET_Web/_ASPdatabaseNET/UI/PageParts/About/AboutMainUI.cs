using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.About.Objs;

namespace ASPdatabaseNET.UI.PageParts.About
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AboutMainUI : MRBPattern<AboutPageInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public AboutMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='AboutMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            var otherToolsDiv = jF(".OtherToolsDiv");

            otherToolsDiv.append(this.GetNotice_SharpKit());
            otherToolsDiv.append(this.GetNotice_jQuery());
            otherToolsDiv.append(this.GetNotice_jQueryJSON());
            otherToolsDiv.append(this.GetNotice_CryptoJS());
            otherToolsDiv.append(this.GetNotice_bcrypt());
            otherToolsDiv.append(this.GetNotice_bouncycastle());
            otherToolsDiv.append(this.GetNotice_EPPlus());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            UI.PagesFramework.BasePage.WindowResized();
            AjaxService.ASPdatabaseService.New(this, "Return").AboutPage_GetInfo();
        }
        public void Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<AboutPageInfo>();

            this.BindUI();
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .AboutMainUI { }
                .AboutMainUI a { color: blue; padding: 0px 2px; }
                .AboutMainUI a:hover { background: #14498f; color: #fff; }
                .AboutMainUI .OtherToolsDiv { overflow-y:scroll; max-height: 10em; background: #f3f3f3; padding: .5em; }
                .AboutMainUI .OtherToolsDiv .Div1 { margin: .5em 0em .5em 1.4em; border-top: 1px solid #ccc; padding-top: .3em; }
                .AboutMainUI .OtherToolsDiv .Div1 .A1 { display:block; width: 16em; padding: .1em .5em; margin-bottom: .2em; }
                .AboutMainUI .OtherToolsDiv .Div1 div { padding: 0.125em 0.625em; font-size:.7em; }
                .AboutMainUI .Holder_SubscriptionAgreement {  }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            <div class='Head'>About ASPdatabase.NET</div>
            <div class='Main AutoResize' AutoResize_BottomSpace='21'>

                <br />
                <a href='http://www.aspdatabase.net/' target='_new'>www.ASPdatabase.NET</a>
                <br />
                <br />
                Version: <span ModelKey='Version'></span>
                <br />
                Version Date: <span ModelKey='VersionDate'></span>
                <br />
                <br />
                Created by <a href='https://twitter.com/mdtanner' target='_new'>Michael Tanner</a>
                <br />
                <br />
                <div class='CopyrightLine' ModelKey='CopyrightLine'></div>
                <br />

                <div class='OtherToolsDiv'>
                    3rd Party Tools<br />
                </div>

                <br />
                <br />
                <a href='ASPdatabase.NET.aspx?SubscriptionAgreement' target='_new'>Open Subscription Agreement <i>(In new tab/window)</i></a>
                <br />
                <br />
                <div class='SubscriptionAgreement' ModelKey='SubscriptionAgreement'></div>
                <br />
                <br />
                <br />
            </div>
            ";
        }

        //----------------------------------------------------------------------------------------------------
        public string GetNotice_SharpKit()
        {
            return @"
            <div class='Div1'>
                <a class='A1' href='https://github.com/SharpKit/SharpKit' target='_blank'>SharpKit</a>
                <div>
                    SharpKit is licensed under GPL. It has all features and has no restrictions. 
                    <br />
                    For commercial licenses please refer to <a href='http://sharpkit.net' target='_blank'>http://sharpkit.net</a>
                </div>
            </div>
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public string GetNotice_jQuery()
        {
            return @"
            <div class='Div1'>
                <a class='A1' href='http://jquery.com/' target='_blank'>jQuery & jQuery UI</a>
                <div>
                    Copyright (C) 2006-2014 jQuery Foundation
                    <br /><br />
                    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
                    <br /><br />
                    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
                    <br /><br />
                    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
                </div>
            </div>
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public string GetNotice_jQueryJSON()
        {
            return @"
            <div class='Div1'>
                <a class='A1' href='https://github.com/Krinkle/jquery-json' target='_blank'>jQuery JSON</a>
                <div>
                    Copyright 2009-2011 Brantley Harris<br />
                    Copyright 2010–2014 Timo Tijhof<br />
                    <br /><br />
                    Permission is hereby granted, free of charge, to any person obtaining
                    a copy of this software and associated documentation files (the
                    ""Software""), to deal in the Software without restriction, including
                    without limitation the rights to use, copy, modify, merge, publish,
                    distribute, sublicense, and/or sell copies of the Software, and to
                    permit persons to whom the Software is furnished to do so, subject to
                    the following conditions:
                    <br /><br />
                    The above copyright notice and this permission notice shall be
                    included in all copies or substantial portions of the Software.
                    <br /><br />
                    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
                    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
                    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
                    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
                    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
                    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
                    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
                </div>
            </div>
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public string GetNotice_CryptoJS()
        {
            return @"
            <div class='Div1'>
                <a class='A1' href='https://code.google.com/p/crypto-js/' target='_blank'>CryptoJS</a>
                <div>
                    (c) 2009-2013 by Jeff Mott. All rights reserved.
                    <br /><br />
                    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
                    <br /><br />
                    Redistributions of source code must retain the above copyright notice, this list of conditions, and the following disclaimer.
                    <br /><br />
                    Redistributions in binary form must reproduce the above copyright notice, this list of conditions, and the following disclaimer in the documentation or other materials provided with the distribution.
                    <br /><br />
                    Neither the name CryptoJS nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
                    <br /><br />
                    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS ""AS IS,"" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE, ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.<br />
                </div>
            </div>
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public string GetNotice_bcrypt()
        {
            return @"
            <div class='Div1'>
                <a class='A1' href='http://bcrypt.codeplex.com/' target='_blank'>BCrypt.Net</a>
                <div>
                    Copyright (c) 2006, 2010, Damien Miller <djm@mindrot.org>, Ryan Emerle
                    All rights reserved.
                    <br /><br />
                    Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
                    <br /><br />
                    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
                    <br /><br />
                    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
                    <br /><br />
                    * Neither the name of BCrypt.Net nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
                    <br /><br />
                    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS ""AS IS"" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
                </div>
            </div>
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public string GetNotice_bouncycastle()
        {
            return @"
            <div class='Div1'>
                <a class='A1' href='http://www.bouncycastle.org/csharp/' target='_blank'>Bouncy Castle C# API</a>
                <div>
                    Copyright (c) 2000 - 2011 The Legion of the Bouncy Castle Inc. (<a href='http://www.bouncycastle.org' target='_blank'>http://www.bouncycastle.org</a>)
                    <br /><br />
                    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
                    <br /><br />
                    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
                    <br /><br />
                    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
                </div>
            </div>
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public string GetNotice_EPPlus()
        {
            return @"
            <div class='Div1'>
                <a class='A1' href='http://epplus.codeplex.com/' target='_blank'>EPPlus</a>
                <div>
                    GNU Library General Public License (LGPL)
                    <br />
                    Version 2.1, February 1999
                    <br />
                    <br />
                    Copyright (C) 1991, 1999 Free Software Foundation, Inc. 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA Everyone is permitted to copy and distribute verbatim copies of this license document, but changing it is not allowed.
                    <br />
                    <br />
                    [This is the first released version of the Lesser GPL. It also counts as the successor of the GNU Library Public License, version 2, hence the version number 2.1.]
                    <br />
                    <br />
                    <a href='http://epplus.codeplex.com/license' target='_blank'>http://epplus.codeplex.com/license</a>
                </div>
            </div>
            ";
        }

    }
}