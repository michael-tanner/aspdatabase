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
    public class st
    {
        public static bool IsServerSide;
        private static bool IsServerSide_StaticCheck = false;

        public string TheString;

        //----------------------------------------------------------------------------------------------------
        public static st New(string input)
        {
            var rtn = new st();
            rtn.TheString = input;

            if (!IsServerSide_StaticCheck)
            {
                IsServerSide = false;
                try { var tmp = ("ABC").As<JsString>().toLowerCase(); }
                catch { IsServerSide = true; }
                IsServerSide_StaticCheck = true;
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public JsString AsJsString()
        {
            return this.TheString.As<JsString>();
        }
        //----------------------------------------------------------------------------------------------------
        public st Replace(string oldValue, string newValue)
        {
            var rtn = new st();
            if (IsServerSide)
                rtn.TheString = this.TheString.Replace(oldValue, newValue);
            else
                rtn.TheString = this.AsJsString().split(oldValue).join(newValue);
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public st Format1(object param0)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0));
            else
                return this.Replace("{0}", param0.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format2(object param0, object param1)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format3(object param0, object param1, object param2)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1, param2));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>())
                    .Replace("{2}", param2.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format4(object param0, object param1, object param2, object param3)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1, param2, param3));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>())
                    .Replace("{2}", param2.As<string>())
                    .Replace("{3}", param3.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format5(object param0, object param1, object param2, object param3, object param4)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1, param2, param3, param4));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>())
                    .Replace("{2}", param2.As<string>())
                    .Replace("{3}", param3.As<string>())
                    .Replace("{4}", param4.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format6(object param0, object param1, object param2, object param3, object param4, object param5)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1, param2, param3, param4, param5));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>())
                    .Replace("{2}", param2.As<string>())
                    .Replace("{3}", param3.As<string>())
                    .Replace("{4}", param4.As<string>())
                    .Replace("{5}", param5.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format7(object param0, object param1, object param2, object param3, object param4, object param5, object param6)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1, param2, param3, param4, param5, param6));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>())
                    .Replace("{2}", param2.As<string>())
                    .Replace("{3}", param3.As<string>())
                    .Replace("{4}", param4.As<string>())
                    .Replace("{5}", param5.As<string>())
                    .Replace("{6}", param6.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format8(object param0, object param1, object param2, object param3, object param4, object param5, object param6, object param7)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1, param2, param3, param4, param5, param6, param7));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>())
                    .Replace("{2}", param2.As<string>())
                    .Replace("{3}", param3.As<string>())
                    .Replace("{4}", param4.As<string>())
                    .Replace("{5}", param5.As<string>())
                    .Replace("{6}", param6.As<string>())
                    .Replace("{7}", param7.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public st Format9(object param0, object param1, object param2, object param3, object param4, object param5, object param6, object param7, object param8)
        {
            if (IsServerSide)
                return st.New(String.Format(this.TheString, param0, param1, param2, param3, param4, param5, param6, param7, param8));
            else
                return this
                    .Replace("{0}", param0.As<string>())
                    .Replace("{1}", param1.As<string>())
                    .Replace("{2}", param2.As<string>())
                    .Replace("{3}", param3.As<string>())
                    .Replace("{4}", param4.As<string>())
                    .Replace("{5}", param5.As<string>())
                    .Replace("{6}", param6.As<string>())
                    .Replace("{7}", param7.As<string>())
                    .Replace("{8}", param8.As<string>());
        }


        //----------------------------------------------------------------------------------------------------
        public int IndexOf(string subString)
        {
            if (IsServerSide)
                return this.TheString.IndexOf(subString);
            else
                return this.AsJsString().indexOf(subString);
        }
        //----------------------------------------------------------------------------------------------------
        public string Substring(int startIndex, int length)
        {
            if (IsServerSide)
                return this.TheString.Substring(startIndex, length);
            else
                return this.AsJsString().substr(startIndex, length);
        }


        //----------------------------------------------------------------------------------------------------
        public bool StartsWith(string subString, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return (this.IndexOf(subString) == 0); //  (this.AsJsString().indexOf(subString) == 0);
            }
            else
            {
                //return (this.AsJsString().toLowerCase().indexOf(subString.As<JsString>().toLowerCase()) == 0);
                return this.ToLower().IndexOf(st.New(subString).ToLower().TheString) == 0;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public bool EndsWith(string subString, bool ignoreCase)
        {
            if (subString.Length > this.TheString.Length)
                return false;
            
            string rightPart = this.TheString.Substring(this.TheString.Length - subString.Length, subString.Length);

            if (!ignoreCase)
            {
                return (subString == rightPart);
            }
            else
            {
                return (st.New(subString).ToLower().TheString == st.New(rightPart).ToLower().TheString);
            }
        }

        //----------------------------------------------------------------------------------------------------
        public st TruncateLeft(int length)
        {
            var rtn = new st();
            if (length > this.TheString.Length)
                return st.New("");
            rtn.TheString = this.Substring(length, this.TheString.Length - length);
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public st TruncateRight(int length)
        {
            var rtn = new st();
            if (length > this.TheString.Length)
                return st.New("");
            rtn.TheString = this.Substring(0, this.TheString.Length - length);
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public st IfStartsWith_Truncate(string subString, bool ignoreCase)
        {
            if (this.StartsWith(subString, ignoreCase))
                return this.TruncateLeft(subString.Length);
            else
                return st.New(this.TheString);
        }
        //----------------------------------------------------------------------------------------------------
        public st IfEndsWith_Truncate(string subString, bool ignoreCase)
        {
            if (this.EndsWith(subString, ignoreCase))
                return this.TruncateRight(subString.Length);
            else
                return st.New(this.TheString);
        }


        //----------------------------------------------------------------------------------------------------
        public st[] Split(string delimiter, bool ignoreCase)
        {
            if(IsServerSide)
            {
                if (ignoreCase)
                {
                    var input = this.TheString;

                    var input_L = input.ToLower(); // input.toLowerCase();
                    var delimiter_L = delimiter.ToLower(); // delimiter.As<JsString>().toLowerCase();
                    var arr = input_L.Split(new string[] { delimiter_L }, StringSplitOptions.None);
                    var rtn = new st[arr.Length];

                    int start = 0;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        rtn[i] = new st();
                        rtn[i].TheString = input.Substring(start, arr[i].Length);
                        start += arr[i].Length + delimiter.Length;
                    }
                    return rtn;
                }
                else
                {
                    var arr = this.TheString.Split(new string[] { delimiter }, StringSplitOptions.None); // this.AsJsString().split(delimiter);
                    var rtn = new st[arr.Length];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        rtn[i] = new st();
                        rtn[i].TheString = arr[i];
                    }
                    return rtn;
                }
            }
            else // (!IsServerSide)
            {
                if (ignoreCase)
                {
                    var input = this.AsJsString();

                    var input_L = input.toLowerCase();
                    var delimiter_L = delimiter.As<JsString>().toLowerCase();
                    var arr = input_L.split(delimiter_L);
                    var rtn = new st[arr.length];

                    int start = 0;
                    for (int i = 0; i < arr.length; i++)
                    {
                        rtn[i] = new st();
                        rtn[i].TheString = input.substr(start, arr[i].length);
                        start += arr[i].length + delimiter.Length;
                    }
                    return rtn;
                }
                else
                {
                    var arr = this.AsJsString().split(delimiter);
                    var rtn = new st[arr.length];
                    for (int i = 0; i < arr.length; i++)
                    {
                        rtn[i] = new st();
                        rtn[i].TheString = arr[i];
                    }
                    return rtn;
                }
            }


            //if(ignoreCase)
            //{
            //    var input = this.AsJsString();

            //    var input_L = input.toLowerCase();
            //    var delimiter_L = delimiter.As<JsString>().toLowerCase();
            //    var arr = input_L.split(delimiter_L);
            //    var rtn = new st[arr.length];

            //    int start = 0;
            //    for (int i = 0; i < arr.length; i++)
            //    {
            //        rtn[i] = new st();
            //        rtn[i].TheString = input.substr(start, arr[i].length);
            //        start += arr[i].length + delimiter.Length;
            //    }
            //    return rtn;
            //}
            //else
            //{
            //    var arr = this.AsJsString().split(delimiter);
            //    var rtn = new st[arr.length];
            //    for (int i = 0; i < arr.length; i++)
            //    {
            //        rtn[i] = new st();
            //        rtn[i].TheString = arr[i];
            //    }
            //    return rtn;
            //}
        }
        //----------------------------------------------------------------------------------------------------
        public st Trim()
        {
            if (IsServerSide)
                return st.New(this.TheString.Trim());
            else
                return st.New(this.AsJsString().trim());
        }
        //----------------------------------------------------------------------------------------------------
        public st ToLower()
        {
            if (IsServerSide)
                return st.New(this.TheString.ToLower());
            else
                return st.New(this.AsJsString().toLowerCase());
        }
        //----------------------------------------------------------------------------------------------------
        public st ToUpper()
        {
            if (IsServerSide)
                return st.New(this.TheString.ToUpper());
            else
                return st.New(this.AsJsString().toUpperCase());
        }
        //----------------------------------------------------------------------------------------------------
        public st Append(string str)
        {
            if (this.TheString == null)
                this.TheString = "";
            this.TheString += str;
            return this;
        }
        //----------------------------------------------------------------------------------------------------
        public st AppendST(st str)
        {
            return this.Append(str.TheString);
        }
        //----------------------------------------------------------------------------------------------------
        public bool Contains(string subString, bool ignoreCase)
        {
            if(IsServerSide)
            {
                if (!ignoreCase)
                    return this.TheString.IndexOf(subString) >= 0;
                else
                    return this.TheString.ToLower().IndexOf(subString.As<JsString>().toLowerCase()) >= 0;
            }
            else
            {
                if (!ignoreCase)
                    return this.AsJsString().indexOf(subString) >= 0;
                else
                    return this.AsJsString().toLowerCase().indexOf(subString.As<JsString>().toLowerCase()) >= 0;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public int Length()
        {
            return this.TheString.Length;
        }



        //----------------------------------------------------------------------------------------------------
        public static string TestUnit()
        {
            string unitName = "ASPdb.FrameworkUI.st";
            string passFail = "Failed";
            string rtn = "";
            st s = null;
            string suffix = "";

            //---------- 01
            s = st.New("AAAaaa BBBbbb CCCccc" + suffix);
            s = s.Replace("BBbb", "--");
            if (s.TheString != "AAAaaa B--b CCCccc")
                rtn += "01) Replace() \n";

            //---------- 02
            s = st.New("aa {0} bb " + suffix);
            if (s.Format1("AA").TheString != "aa AA bb ")
                rtn += "02) Format1() \n";

            s = st.New("aa {0} bb {1} cc " + suffix);
            if (s.Format2("AA", "BB").TheString != "aa AA bb BB cc ")
                rtn += "02) Format2() \n";

            s = st.New("aa {0} bb {1} cc {2} dd " + suffix);
            if (s.Format3("AA", "BB", "CC").TheString != "aa AA bb BB cc CC dd ")
                rtn += "02) Format3() \n";

            s = st.New("aa {0} bb {1} cc {2} dd {3} ee " + suffix);
            if (s.Format4("AA", "BB", "CC", "DD").TheString != "aa AA bb BB cc CC dd DD ee ")
                rtn += "02) Format4() \n";

            s = st.New("aa {0} bb {1} cc {2} dd {3} ee {4} ff " + suffix);
            if (s.Format5("AA", "BB", "CC", "DD", "EE").TheString != "aa AA bb BB cc CC dd DD ee EE ff ")
                rtn += "02) Format5() \n";

            s = st.New("aa {0} bb {1} cc {2} dd {3} ee {4} ff {5} gg " + suffix);
            if (s.Format6("AA", "BB", "CC", "DD", "EE", "FF").TheString != "aa AA bb BB cc CC dd DD ee EE ff FF gg ")
                rtn += "02) Format6() \n";

            s = st.New("aa {0} bb {1} cc {2} dd {3} ee {4} ff {5} gg {6} hh " + suffix);
            if (s.Format7("AA", "BB", "CC", "DD", "EE", "FF", "GG").TheString != "aa AA bb BB cc CC dd DD ee EE ff FF gg GG hh ")
                rtn += "02) Format7() \n";

            s = st.New("aa {0} bb {1} cc {2} dd {3} ee {4} ff {5} gg {6} hh {7} ii " + suffix);
            if (s.Format8("AA", "BB", "CC", "DD", "EE", "FF", "GG", "HH").TheString != "aa AA bb BB cc CC dd DD ee EE ff FF gg GG hh HH ii ")
                rtn += "02) Format8() \n";

            s = st.New("aa {0} bb {1} cc {2} dd {3} ee {4} ff {5} gg {6} hh {7} ii {8} jj " + suffix);
            if (s.Format9("AA", "BB", "CC", "DD", "EE", "FF", "GG", "HH", "II").TheString != "aa AA bb BB cc CC dd DD ee EE ff FF gg GG hh HH ii II jj ")
                rtn += "02) Format9() \n";

            //---------- 03
            s = st.New("AAaa 1234567890 bbBB" + suffix);
            if (s.StartsWith("AAaa 12", false) != true) rtn += "03.a) StartsWith() \n";
            if (s.StartsWith("aaaa 12", false) != false) rtn += "03.b) StartsWith() \n";
            if (s.StartsWith("aaaa 12", true) != true) rtn += "03.c) StartsWith() \n";

            if (s.EndsWith("90 bbBB", false) != true) rtn += "03.d) EndsWith() \n";
            if (s.EndsWith("90 bbbb", false) != false) rtn += "03.e) EndsWith() \n";
            if (s.EndsWith("90 bbbb", true) != true) rtn += "03.f) EndsWith() \n";

            //---------- 04
            s = st.New("12345678901234567890" + suffix);
            if (s.TruncateLeft(0).TheString != "12345678901234567890") rtn += "04.a) TruncateLeft() \n";
            if (s.TruncateLeft(21).TheString != "") rtn += "04.b) TruncateLeft() \n";
            if (s.TruncateLeft(5).TheString != "678901234567890") rtn += "04.c) TruncateLeft() \n";

            if (s.TruncateRight(0).TheString != "12345678901234567890") rtn += "04.d) TruncateRight() \n";
            if (s.TruncateRight(21).TheString != "") rtn += "04.e) TruncateRight() \n";
            if (s.TruncateRight(5).TheString != "123456789012345") rtn += "04.f) TruncateRight() \n";

            //---------- 05
            s = st.New("a12345678901234567890z" + suffix);
            if (s.IfStartsWith_Truncate("A12345", true).TheString != "678901234567890z") rtn += "05.a) IfStartsWith_Truncate() \n";
            if (s.IfStartsWith_Truncate("A12345", false).TheString != "a12345678901234567890z") rtn += "05.b) IfStartsWith_Truncate() \n";
            if (s.IfStartsWith_Truncate("a12345", false).TheString != "678901234567890z") rtn += "05.c) IfStartsWith_Truncate() \n";

            if (s.IfEndsWith_Truncate("7890Z", true).TheString != "a1234567890123456") rtn += "05.d) IfEndsWith_Truncate() \n";
            if (s.IfEndsWith_Truncate("7890Z", false).TheString != "a12345678901234567890z") rtn += "05.d) IfEndsWith_Truncate() \n";
            if (s.IfEndsWith_Truncate("7890z", false).TheString != "a1234567890123456") rtn += "05.d) IfEndsWith_Truncate() \n";

            //---------- 06
            s = st.New("AAAxBBBxCCCXDDDXEEE" + suffix);
            var arr = s.Split("x", false);
            if (arr.Length != 3) rtn += "06.a) Split() \n";
            arr = s.Split("x", true);
            if (arr.Length != 5) rtn += "06.b) Split() \n";
            if (arr[0].TheString != "AAA") rtn += "06.c) Split() \n";
            if (arr[4].TheString != "EEE") rtn += "06.d) Split() \n";

            //---------- 07
            s = st.New("   Aa Aa   " + suffix);
            if (s.Trim().TheString != "Aa Aa") rtn += "07.a) Trim() \n";
            if (s.ToUpper().TheString != "   AA AA   ") rtn += "07.b) ToUpper() \n";
            if (s.ToLower().TheString != "   aa aa   ") rtn += "07.c) ToLower() \n";

            //---------- 08
            s = st.New("aaa aaa" + suffix);
            s.Append(" aaa");
            if (s.TheString != "aaa aaa aaa") rtn += "08.a) Append() \n";
            s.AppendST(st.New(" bbb"));
            if (s.TheString != "aaa aaa aaa bbb") rtn += "08.b) AppendST() \n";

            if (rtn == "") passFail = "Passed";
            return st.New("{0} : {1} \n{2}").Format3(unitName, passFail, rtn).TheString;
        }
    }
}